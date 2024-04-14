using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

            Console.WriteLine($"Found {regionFiles.Length} region files:");
            ResultMessage = string.Join("\n", regionFiles);

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

            // TODO: Prism instances

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
            ResultMessage = "Unknown processing error";
            return Array.Empty<string>();
        }
    }
}
