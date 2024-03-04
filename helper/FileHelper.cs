using System;
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
            if (Program.DEBUG_MODE)
            {
                Console.WriteLine("Copying " + Path.GetFileName(destination));
            }
            File.Copy(source, destination, true);
        }

        public static void DeleteFolder(string path)
        {
            if (Program.DEBUG_MODE)
            {
                Console.WriteLine("Deleting folder " + path);
            }
            Directory.Delete(path, true);
        }

        public static string DownloadFile(string url)
        {
            string basePath = GetDownloadPath();
            Directory.CreateDirectory(basePath);

            Uri uri = new Uri(url);
            string fullPath = Path.Combine(basePath, Path.GetFileName(uri.LocalPath));

            DownloadFile(url, fullPath);

            return fullPath;
        }

        public static void DownloadFile(string url, string destinationFile)
        {
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
                if (Program.DEBUG_MODE)
                {
                    Console.WriteLine("Moving " + file);
                }
                File.Move(file, Path.Combine(destination, Path.GetFileName(file)));
            }
            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.TopDirectoryOnly))
            {
                if (Program.DEBUG_MODE)
                {
                    Console.WriteLine("Moving " + dir);
                }
                Directory.Move(dir, Path.Combine(destination, Path.GetFileName(dir)));
            }
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
                        if (Program.DEBUG_MODE)
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
