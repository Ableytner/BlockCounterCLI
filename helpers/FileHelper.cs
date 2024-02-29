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
        public static void DownloadFile(string url, string destination_file)
        {
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

        public static string DownloadFile(string url)
        {
            string base_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "download");
            Directory.CreateDirectory(base_path);

            Uri uri = new Uri(url);
            string full_path = Path.Combine(base_path, Path.GetFileName(uri.LocalPath));

            DownloadFile(url, full_path);

            return full_path;
        }

        public static void UnzipFile(string filename, string destination_path) 
        {
            using (ZipArchive archive = ZipFile.OpenRead(filename))
            {
                foreach (var entry in archive.Entries)
                {
                    Console.WriteLine(entry.FullName);
                    if (Path.GetFileName(entry.FullName).Length == 0)
                    {
                        Directory.CreateDirectory(Path.Combine(destination_path, entry.FullName));
                    }
                    else
                    {
                        entry.ExtractToFile(Path.Combine(destination_path, entry.FullName));
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
    }
}
