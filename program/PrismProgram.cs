using BlockCounterCLI.command;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
            SetupLauncher();
            SetupLauncherConfig();
            SetupInstance();
        }

        private void SetupLauncher()
        {
            // download files
            string url = "https://github.com/PrismLauncher/PrismLauncher/releases/download/8.0/PrismLauncher-Windows-MSVC-Portable-8.0.zip";
            string prism_zip = FileHelper.DownloadFile(url);

            // copy files
            string prism_dir = FileHelper.GetProgramsPath("prism");
            FileHelper.UnzipFile(prism_zip, prism_dir);
        }

        private void SetupLauncherConfig()
        {
            string url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/accounts.json";
            string prism_accounts_dl = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/prismlauncher.cfg";
            string prism_config_dl = FileHelper.DownloadFile(url);

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

        private void SetupInstance()
        {
            string url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/Forge1.7.10Template/instance.cfg";
            string instance_config = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/Forge1.7.10Template/mmc-pack.json";
            string mmc_pack = FileHelper.DownloadFile(url);

            string instance_dir = Path.Combine(FileHelper.GetProgramsPath("prism"), "instances", "Forge1.7.10template");
            Directory.CreateDirectory(instance_dir);

            FileHelper.CopyFile(instance_config, Path.Combine(instance_dir, "instance.cfg"));
            FileHelper.CopyFile(mmc_pack, Path.Combine(instance_dir, "mmc-pack.json"));
        }
    }
}
