using System;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;

namespace BlockCounterCLI.helpers
{
    internal class FileHelper
    {
        public static void CopyFile(string source, string destination)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Copying " + Path.GetFileName(destination));
            }
            File.Copy(source, destination, true);
        }

        public static string DecompressGz(string compressedFilePath)
        {
            using (FileStream originalFileStream = new FileInfo(compressedFilePath).OpenRead())
            {
                string newFileName = compressedFilePath.Remove(compressedFilePath.Length - 3);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        return newFileName;
                    }
                }
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Deleting file " + filePath);
            }
            File.Delete(filePath);
        }

        public static void DeleteFolder(string path)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Deleting folder " + path);
            }
            Directory.Delete(path, true);
        }

        public static string DownloadFile(string url)
        {
            return DownloadFile(url, false);
        }

        public static string DownloadFile(string url, bool force)
        {
            string basePath = GetDownloadPath();
            Directory.CreateDirectory(basePath);

            Uri uri = new Uri(url);
            string fullPath = Path.Combine(basePath, Path.GetFileName(uri.LocalPath));

            DownloadFile(url, fullPath, force);

            return fullPath;
        }

        public static string DownloadFile(string url, string destinationFile)
        {
            return DownloadFile(url, destinationFile, false);
        }

        public static string DownloadFile(string url, string destinationFile, bool force)
        {
            if (File.Exists(destinationFile))
            {
                if (force)
                {
                    DeleteFile(destinationFile);
                }
                else
                {
                    Console.WriteLine($"Skipping download for {Path.GetFileName(destinationFile)}");
                    return destinationFile;
                }
            }

            Console.WriteLine("Downloading " + Path.GetFileName(destinationFile));

            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync(url).Result)
                {
                    using (var fs = new FileStream(destinationFile, FileMode.OpenOrCreate))
                    {
                        s.CopyTo(fs);
                    }
                }
            }

            return Path.GetFullPath(destinationFile);
        }

        public static string ExtractTar(string tarFilePath)
        {
            string outputDirectory = Path.Combine(Path.GetDirectoryName(tarFilePath), Path.GetFileNameWithoutExtension(tarFilePath));
            Directory.CreateDirectory(outputDirectory);
            ExtractTar(tarFilePath, outputDirectory);
            return outputDirectory;
        }

        public static void ExtractTar(string tarFilePath, string outputDirectory)
        {
            using (var tarStream = new FileStream(tarFilePath, new FileStreamOptions { Mode = FileMode.Open, Access = FileAccess.Read }))
            {
                using (var tarReader = new TarReader(tarStream))
                {
                    TarEntry entry;
                    while ((entry = tarReader.GetNextEntry()) != null)
                    {
                        if (entry.EntryType is TarEntryType.SymbolicLink or TarEntryType.HardLink or TarEntryType.GlobalExtendedAttributes)
                        {
                            continue;
                        }
                        if (entry.EntryType is TarEntryType.Directory)
                        {
                            Directory.CreateDirectory(Path.Combine(outputDirectory, entry.Name));
                        }
                        else
                        {
                            entry.ExtractToFile(Path.Combine(outputDirectory, entry.Name), true);
                        }
                    }
                }
            }
        }

        public static string ExtractTarGz(string tarGzFilePath)
        {
            string outputDirectory = Path.Combine(Path.GetDirectoryName(tarGzFilePath), Path.GetFileNameWithoutExtension(tarGzFilePath));
            Directory.CreateDirectory(outputDirectory);
            ExtractTarGz(tarGzFilePath, outputDirectory);
            return outputDirectory;
        }

        public static void ExtractTarGz(string tarGzFilePath, string outputDirectory)
        {
            string tarFilePath = DecompressGz(tarGzFilePath);
            ExtractTar(tarFilePath, outputDirectory);
        }

        public static string GetPath()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static string GetDownloadPath()
        {
            string path = Path.Combine(GetPath(), "download");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static string GetProgramsPath()
        {
            string path = Path.Combine(GetPath(), "programs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static string GetProgramsPath(string programName)
        {
            string path = Path.Combine(GetProgramsPath(), programName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void MoveAllFolderContents(string source, string destination)
        {
            foreach(var file in Directory.EnumerateFiles(source, "*", SearchOption.TopDirectoryOnly))
            {
                if (CLI.IsDebugMode)
                {
                    Console.WriteLine("Moving " + file);
                }
                File.Move(file, Path.Combine(destination, Path.GetFileName(file)));
            }
            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.TopDirectoryOnly))
            {
                if (CLI.IsDebugMode)
                {
                    Console.WriteLine("Moving " + dir);
                }
                Directory.Move(dir, Path.Combine(destination, Path.GetFileName(dir)));
            }
        }

        public static void RenameFile(string filename, string newFilename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            if (File.Exists(newFilename))
            {
                DeleteFile(newFilename);
            }

            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Renaming " + Path.GetFileName(filename) + " to " + Path.GetFileName(newFilename));
            }
            File.Move(filename, newFilename);
        }

        public static string UnzipFile(string filename)
        {
            string destinationPath = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
            UnzipFile(filename, destinationPath);
            return destinationPath;
        }

        public static void UnzipFile(string filename, string destinationPath) 
        {
            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                foreach (var entry in archive.Entries)
                {
                    if (Path.GetFileName(entry.FullName).Length == 0)
                    {
                        Directory.CreateDirectory(Path.Combine(destinationPath, entry.FullName));
                    }
                    else
                    {
                        if (CLI.IsDebugMode)
                        {
                            Console.WriteLine("Extracting " + entry.FullName);
                        }
                        Directory.CreateDirectory(Path.Combine(destinationPath, Path.GetDirectoryName(entry.FullName)));
                        entry.ExtractToFile(Path.Combine(destinationPath, entry.FullName), true);
                    }
                }
            }
        }
    }
}
