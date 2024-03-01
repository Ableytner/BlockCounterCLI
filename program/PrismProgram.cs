using BlockCounterCLI.command;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.helpers
{
    internal class PrismProgram : BaseProgram
    {
        public PrismProgram() { }

        public override string Name => "Prism";

        public override bool IsSetup()
        {
            return false;
        }

        public override void Setup()
        {
            // download files
            string url = "https://github.com/PrismLauncher/PrismLauncher/releases/download/8.0/PrismLauncher-Windows-MSVC-Portable-8.0.zip";
            string prism_zip = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/accounts.json";
            string prism_accounts_dl = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/prismlauncher.cfg";
            string prism_config_dl = FileHelper.DownloadFile(url);

            // copy files
            string prism_dir = FileHelper.GetProgramsPath("prism");
            FileHelper.UnzipFile(prism_zip, prism_dir);

            string prism_accounts = Path.Combine(FileHelper.GetProgramsPath("prism"), "accounts.json");
            FileHelper.CopyFile(prism_accounts_dl, prism_accounts);

            string prism_config = Path.Combine(FileHelper.GetProgramsPath("prism"), "prismlauncher.cfg");
            string config_content = File.ReadAllText(prism_config_dl);
            JavaProgram javaProgram = ProgramRegistry.Instance.GetProgram("Java") as JavaProgram;
            Console.WriteLine(javaProgram);
            config_content = config_content.Replace("[java_path_hare]", javaProgram.java8_path.Replace("\\", "/"));
            config_content = config_content.Replace("[hostname_here]", Environment.MachineName);
            File.WriteAllText(prism_config_dl, config_content);
            FileHelper.CopyFile(prism_config_dl, prism_config);
        }
    }
}
