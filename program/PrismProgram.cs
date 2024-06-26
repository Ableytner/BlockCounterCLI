﻿using BlockCounterCLI.command;
using BlockCounterCLI.program;
using System;
using System.IO;

namespace BlockCounterCLI.helpers
{
    internal class PrismProgram : BaseProgram
    {
        public string prismExecutable;
        public string mappingsFile;

        public PrismProgram()
        {
            if (CLI.IsWindows)
            {
                prismExecutable = Path.Combine(FileHelper.GetProgramsPath(Name), "prismlauncher.exe");
            }
            else
            {
                throw new NotImplementedException();
            }

            mappingsFile = Path.Combine(FileHelper.GetPath(), "blockid_to_name.json");
        }

        public override string Name => "Prism";

        public override Type[] DependsOn => [typeof(JavaProgram)];

        public override bool IsSetup()
        {
            return File.Exists(prismExecutable);
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
            string prismZip = FileHelper.DownloadFile(url);

            // copy files
            string prismDir = FileHelper.GetProgramsPath(Name);
            FileHelper.UnzipFile(prismZip, prismDir);
        }

        private void SetupLauncherConfig()
        {
            string url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/accounts.json";
            string prismAccountsDl = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/prismlauncher.cfg";
            string prismConfigDl = FileHelper.DownloadFile(url);

            string prismAccounts = Path.Combine(FileHelper.GetProgramsPath(Name), "accounts.json");
            FileHelper.CopyFile(prismAccountsDl, prismAccounts);

            string prismConfig = Path.Combine(FileHelper.GetProgramsPath(Name), "prismlauncher.cfg");
            string configContent = File.ReadAllText(prismConfigDl);
            JavaProgram javaProgram = ProgramRegistry.Instance.GetProgram("Java") as JavaProgram;
            configContent = configContent.Replace("[java_path_hare]", javaProgram.java8Executable.Replace("\\", "/"));
            configContent = configContent.Replace("[hostname_here]", Environment.MachineName);
            File.WriteAllText(prismConfigDl, configContent);
            FileHelper.CopyFile(prismConfigDl, prismConfig);
        }

        private void SetupInstance()
        {
            string url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/Forge1.7.10Template/instance.cfg";
            string instanceConfig = FileHelper.DownloadFile(url);

            url = "https://raw.githubusercontent.com/Ableytner/BlockCounterCLI/main/data/Forge1.7.10Template/mmc-pack.json";
            string mmcPack = FileHelper.DownloadFile(url);

            url = "https://github.com/Ableytner/blockid-to-name/releases/download/1.6.0/bidtoname-1.6.0.jar";
            string bidtoname = FileHelper.DownloadFile(url);

            string instanceDir = Path.Combine(FileHelper.GetProgramsPath(Name), "instances", "Forge1.7.10template");
            Directory.CreateDirectory(instanceDir);

            FileHelper.CopyFile(instanceConfig, Path.Combine(instanceDir, "instance.cfg"));
            FileHelper.CopyFile(mmcPack, Path.Combine(instanceDir, "mmc-pack.json"));

            string mods_dir = Path.Combine(instanceDir, ".minecraft", "mods");
            Directory.CreateDirectory(mods_dir);
            FileHelper.CopyFile(bidtoname, Path.Combine(mods_dir, Path.GetFileName(bidtoname)));
        }
    }
}
