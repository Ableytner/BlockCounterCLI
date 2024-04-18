using AngleSharp;
using AngleSharp.Dom;
using AnvilParser;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using Newtonsoft.Json;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class CountCommand : BaseCommand
    {
        public static new string Prefix = "count";
        public static new string Description = "counts all blocks in a given minecraft world";

        public CountCommand(string[] args)
        {
            this.args = args;
        }

        private string[] args;

        public override void Execute()
        {
            if (args.Length > 1)
            {
                Errored = true;
                ResultMessage = "More than one argument provided\n"
                                + "Please verify that the path is under \"quotation marks\"";
                return;
            }

            if (args.Length == 0)
            {
                // TODO: file dialog

                Errored = true;
                ResultMessage = "No world path provided";
                return;
            }

            string[] regionFiles = GetRegionFiles(args[0]);

            // GetRegionFiles failed
            if (Errored == true
                && ResultMessage != string.Empty)
            {
                return;
            }

            // GetRegionFiles failed
            if (regionFiles.Length == 0)
            {
                // ensure that ResultMessage is set
                if (ResultMessage != string.Empty)
                {
                    Errored = true;
                    ResultMessage = "No valid region files found\n"
                                    + "Please verify that the path is under \"quotation marks\"";
                }
                return;
            }

            Console.WriteLine($"Found {regionFiles.Length} region files");

            Console.WriteLine("Starting to count, this may take a few minutes");
            var watch = Stopwatch.StartNew();
            Dictionary<string, long> blockCounts = CountBlocks(regionFiles);
            watch.Stop();
            Console.WriteLine($"Counting took {watch.ElapsedMilliseconds / 1000} s");

            var blockCountsFiltered = MapBlockNames(blockCounts, regionFiles[0]);

            var blockCountsSorted = blockCountsFiltered.ToList();
            blockCountsSorted.Sort((x, y) => y.Value.CompareTo(x.Value));

            var outputJsonFile = Path.Combine(FileHelper.GetPath(), "block_counts.json");
            string outputJsonText = "{" + string.Join(',', blockCountsSorted.Select(kvp => $"\"{kvp.Key}\": {kvp.Value}")) + "}";
            File.WriteAllText(outputJsonFile, outputJsonText);
            Console.WriteLine($"Block counts saved to '{outputJsonFile}'");

            ResultMessage = outputJsonText;
            Errored = false;
        }

        private Dictionary<string, long> MapBlockNames(Dictionary<string, long> blockCounts, string sampleRegionFile)
        {
            PrismProgram prismProgram = ProgramRegistry.Instance.GetProgram(typeof(PrismProgram));

            Dictionary<string, long> formattedBlockCounts = new Dictionary<string, long>();

            // map pre 17w47a ids to names
            if (int.TryParse(blockCounts.Keys.ToArray()[0].Split(":")[0], out _))
            {
                // check if mappings file is missing
                if (!File.Exists(prismProgram.mappingsFile))
                {
                    Console.WriteLine("Missing blockid_to_name.json to convert old IDs to names.");
                    Console.WriteLine("Run 'old-mappings' and then try again.");
                    return blockCounts;
                }

                string mappingsText = File.ReadAllText(prismProgram.mappingsFile);
                Dictionary<string, string> mappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(mappingsText);

                int unmappableIds = 0;
                foreach (var blockCount in blockCounts)
                {
                    if (mappings.TryGetValue(blockCount.Key, out string mappedName))
                    {
                        formattedBlockCounts[mappedName] = blockCount.Value;
                    }
                    else
                    {
                        unmappableIds++;
                        if (CLI.IsDebugMode)
                        {
                            Console.WriteLine($"Could not translate id {blockCount.Key}");
                        }
                        // fall back to the unmapped ID
                        formattedBlockCounts[blockCount.Key] = blockCount.Value;
                    }
                }

                if (unmappableIds > 0)
                {
                    Console.WriteLine($"Failed to translate {unmappableIds} IDs");
                }

                return formattedBlockCounts;
            }

            var langDict = GetLanguageDictionary(sampleRegionFile);

            int unmappableNames = 0;
            foreach (var blockCount in blockCounts)
            {
                string blockKey = $"block.minecraft.{blockCount.Key}";
                if (langDict.TryGetValue(blockKey, out string mappedName))
                {
                    formattedBlockCounts[mappedName] = blockCount.Value;
                }
                else
                {
                    unmappableNames++;
                    if (CLI.IsDebugMode)
                    {
                        Console.WriteLine($"Could not translate name {blockCount.Key}");
                    }
                    // fall back to the unmapped ID
                    formattedBlockCounts[blockCount.Key] = blockCount.Value;
                }
            }

            if (unmappableNames > 0)
            {
                Console.WriteLine($"Failed to translate {unmappableNames} names");
            }

            return formattedBlockCounts;
        }

        private Dictionary<string, string> GetLanguageDictionary(string sampleRegionFile)
        {
            Region region = Region.FromFile(sampleRegionFile);
            int versionId = 0;
            foreach (Chunk chunk in region.StreamChunks())
            {
                versionId = chunk.Version;
                break;
            }
            if (versionId == 0)
            {
                throw new Exception("Version could not be determined from chunk");
            }
            string versionName = VersionHelper.GetVersionName(versionId);

            string jarDownloadUrl = GetJarDownloadLink(versionName);

            string jarFile = FileHelper.DownloadFile(jarDownloadUrl, true);

            string langJson = null;
            using (ZipArchive serverJar = ZipFile.OpenRead(jarFile))
            {
                string innerServerFile = Path.Combine(FileHelper.GetDownloadPath(), $"server-{versionName}.jar");

                // for versions 1.17.1 and lower
                var entries = serverJar.Entries.Where(y => y.Name == $"en_us.json");
                if (entries.Count() == 1)
                {
                    langJson = Path.Combine(FileHelper.GetDownloadPath(), "en_us.json");
                    entries.ToArray()[0].ExtractToFile(langJson, true);
                }
                // for versions 1.18 and higher
                else
                {
                    entries = serverJar.Entries.Where(y => y.Name == $"server-{versionName}.jar");
                    if (entries.Count() != 1)
                    {
                        throw new Exception($"Could not find 'server-{versionName}.jar' for version {versionName}");
                    }
                    entries.ToArray()[0].ExtractToFile(innerServerFile, true);

                    using (ZipArchive innerServerJar = ZipFile.OpenRead(innerServerFile))
                    {
                        entries = innerServerJar.Entries.Where(y => y.Name == "en_us.json");
                        if (entries.Count() != 1)
                        {
                            throw new Exception("Could not pars inner server.jar");
                        }
                        langJson = Path.Combine(FileHelper.GetDownloadPath(), "en_us.json");
                        entries.ToArray()[0].ExtractToFile(langJson, true);
                    }
                }
            }

            if (langJson == null || !File.Exists(langJson))
            {
                throw new Exception();
            }
            string langJsonText = File.ReadAllText(langJson);
            var langDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(langJsonText);
            if (langDict == null)
            {
                throw new Exception("Json deserialize failed");
            }
            return langDict;
        }

        private string GetJarDownloadLink(string versionName)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            string address = $"https://mcversions.net/download/{versionName}";
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = Task.Run(async () => await context.OpenAsync(address)).Result;
            string cellSelector = "a.text-xs.whitespace-nowrap.py-3.px-8.bg-green-700.rounded.text-white.no-underline.font-bold.transition-colors.duration-200";
            IHtmlCollection<IElement> cells = document.QuerySelectorAll(cellSelector);
            
            if (cells.Length < 0)
            {
                throw new Exception($"Could not find download link on page {address}");
            }

            return cells[0].GetAttribute("href");
        }

        private Dictionary<string, long> CountBlocks(string[] regionFiles)
        {
            List<string> regionFilesList = new List<string>(regionFiles);
            Dictionary<string, long> blockCounts = new Dictionary<string, long>();
            int MAXTHREADS = 20;
            List<Tuple<Thread, Dictionary<string, long>>> threads = new List<Tuple<Thread, Dictionary<string, long>>>();

            ProgressBarOptions options = new ProgressBarOptions
            {
                ProgressCharacter = '-',
                ProgressBarOnBottom = true,
                ForegroundColor = ConsoleColor.White
            };
            using (ProgressBar pBar = new ProgressBar(regionFilesList.Count, "Reading region files", options))
            {
                do
                {
                    for (int i = 0; i < threads.Count; i++)
                    {
                        if (!threads[i].Item1.IsAlive)
                        {
                            MergeDictionaries(blockCounts, threads[i].Item2);
                            threads.RemoveAt(i);
                            i--;
                            pBar.Tick();
                        }
                    }

                    while (threads.Count < MAXTHREADS && regionFilesList.Count > 0)
                    {
                        string regionFile = regionFilesList[0];
                        regionFilesList.RemoveAt(0);

                        Dictionary<string, long> resultList = new Dictionary<string, long>();

                        Thread t = new Thread(() => CountBlocksInOneRegion(regionFile, resultList));
                        t.Start();

                        threads.Add(new Tuple<Thread, Dictionary<string, long>>(t, resultList));
                    }

                    Thread.Sleep(1000);
                }
                while (threads.Count > 0);
            }

            return blockCounts;
        }

        private void MergeDictionaries(Dictionary<string, long> copyTo, Dictionary<string, long> copyFrom)
        {
            foreach (var key in copyFrom.Keys)
            {
                if (copyTo.ContainsKey(key))
                {
                    copyTo[key] += copyFrom[key];
                }
                else
                {
                    copyTo[key] = copyFrom[key];
                }
            }
        }

        private void CountBlocksInOneRegion(string regionFile, Dictionary<string, long> blockCounts)
        {
            Region region = Region.FromFile(regionFile);
            string blockName;

            foreach (Chunk chunk in region.StreamChunks())
            {
                foreach (BaseBlock block in chunk.StreamBlocks())
                {
                    if (block.Id != "air" & block.Id != "0")
                    {
                        if (block is OldBlock)
                        {
                            blockName = $"{block.Id}:{((OldBlock)block).Data}";
                        }
                        else
                        {
                            blockName = block.Id;
                        }

                        if (!blockCounts.TryGetValue(blockName, out long value))
                        {
                            value = 0;
                            blockCounts[blockName] = value;
                        }

                        blockCounts[blockName] = ++value;
                    }
                }
            }
        }

        private string[] GetRegionFiles(string path)
        {
            List<string> regionFiles = new List<string>();
            // path is a single region file
            if (File.Exists(path))
            {
                if (Path.GetExtension(path) != ".mca")
                {
                    Console.WriteLine("Provided file does not have expected .mca extension, continuing anyways");
                }

                regionFiles.Add(path);
                return regionFiles.ToArray();
            }

            // ensure that path is a folder
            if (!Directory.Exists(path))
            {
                Errored = true;
                ResultMessage = "Directory does not exist\n"
                                + "Please verify that the path is under \"quotation marks\"";
                return Array.Empty<string>();
            }

            // Prism Launcher instances
            if (Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray().Contains(".minecraft"))
            {
                path = Path.Combine(path, ".minecraft");
            }

            // if path is the whole .minecraft folder, switch into the saves folder
            if (Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray().Contains("saves"))
            {
                path = Path.Combine(path, "saves");
            }

            // path is not a world folder, but the whole saves folder
            if (Path.GetFileName(path) == "saves")
            {
                string[] worlds = Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray();
                if (worlds.Length == 0)
                {
                    Errored = true;
                    ResultMessage = "No world is present in the selected Minecraft instance";
                    return Array.Empty<string>();
                }

                List<string> worldsList = worlds.ToList();
                foreach (string world in worlds)
                {
                    if (!Directory.Exists(Path.Combine(path, world, "region")))
                    {
                        worldsList.Remove(world);
                    }
                }
                worlds = worldsList.ToArray();

                bool complete = false;
                while (!complete)
                {
                    Console.WriteLine("No world specified, select one:");
                    for (int i = 1; i <= worlds.Length; i++)
                    {
                        Console.WriteLine($"{i, 3}: {worlds[i - 1]}");
                    }
                    Console.Write("> ");
                    string choice = Console.ReadLine();
                    if (worlds.Contains(choice))
                    {
                        path = Path.Combine(path, choice);
                        complete = true;
                    }
                    else if (int.TryParse(choice, out int choiceInt))
                    {
                        if (choiceInt != 0
                            && choiceInt <= worlds.Length)
                        {
                            path = Path.Combine(path, worlds[choiceInt - 1]);
                            complete = true;
                        }
                    }
                    
                    if (!complete)
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
            }

            // path is a server folder
            if (Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray().Contains("world"))
            {
                path = Path.Combine(path, "world");
            }

            // path is a world folder
            if (Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray().Contains("region"))
            {
                regionFiles.AddRange(GetRegionFiles(Path.Combine(path, "region")));

                string[] subDirs = Directory.GetDirectories(path).Select(t => Path.GetFileName(t)).ToArray();
                Regex dimRegex = new Regex(@"^DIM.+$");
                foreach (string subDir in subDirs)
                {
                    if (dimRegex.IsMatch(subDir)
                        && Directory.Exists(Path.Combine(path, subDir, "region")))
                    {
                        regionFiles.AddRange(GetRegionFiles(Path.Combine(path, subDir, "region")));
                    }
                }

                return regionFiles.ToArray();
            }

            // add region files from current directory
            if (Path.GetFileName(path) == "region")
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".mca")
                    {
                        regionFiles.Add(file);
                    }
                }
                return regionFiles.ToArray();
            }

            Errored = true;
            ResultMessage = "No world found in provided path";
            return Array.Empty<string>();
        }
    }
}
