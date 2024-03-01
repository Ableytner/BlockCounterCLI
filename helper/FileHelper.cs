using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.helpers
{
    internal class FileHelper
    {
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
        public static string GetProgramsPath(string program_name)
        {
            string path = Path.Combine(GetProgramsPath(), program_name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void CopyFile(string source, string destination)
        {
            Console.WriteLine("Copying " + Path.GetFileName(destination));
            File.Copy(source, destination, true);
        }

        public static string DownloadFile(string url)
        {
            string base_path = GetDownloadPath();
            Directory.CreateDirectory(base_path);

            Uri uri = new Uri(url);
            string full_path = Path.Combine(base_path, Path.GetFileName(uri.LocalPath));

            DownloadFile(url, full_path);

            return full_path;
        }

        public static void DownloadFile(string url, string destination_file)
        {
            Console.WriteLine("Downloading " + Path.GetFileName(destination_file));

            using (var client = new HttpClient())
            {
                using (var s = client.GetStreamAsync(url).Result)
                {
                    using (var fs = new FileStream(destination_file, FileMode.OpenOrCreate))
                    {
                        s.CopyTo(fs);
                    }
                }
            }
        }

        public static string UnzipFile(string filename)
        {
            string destination_path = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename));
            UnzipFile(filename, destination_path);
            return destination_path;
        }

        public static void UnzipFile(string filename, string destination_path) 
        {
            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                foreach (var entry in archive.Entries)
                {
                    if (Path.GetFileName(entry.FullName).Length == 0)
                    {
                        Directory.CreateDirectory(Path.Combine(destination_path, entry.FullName));
                    }
                    else
                    {
                        Console.WriteLine("Extracting " + entry.FullName);
                        Directory.CreateDirectory(Path.Combine(destination_path, Path.GetDirectoryName(entry.FullName)));
                        entry.ExtractToFile(Path.Combine(destination_path, entry.FullName), true);
                    }
                }
            }
        }
    }
}
