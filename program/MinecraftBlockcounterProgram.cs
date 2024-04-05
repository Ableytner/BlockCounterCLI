using BlockCounterCLI.command;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.IO;

namespace BlockCounterCLI.program
{
    internal class MinecraftBlockcounterProgram : BaseProgram
    {
        public string mainFile;

        public MinecraftBlockcounterProgram()
        {
            mainFile = Path.Combine(FileHelper.GetProgramsPath(Name), "main.py");
        }

        public override string Name => "MinecraftBlockcounter";

        public override Type[] DependsOn => [typeof(PythonProgram)];

        public override bool IsSetup()
        {
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            if (!pythonProgram.IsSetup())
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(pythonProgram.pythonExecutable, "-m pip list");
            if (!output.Contains("anvil-parser"))
            {
                return false;
            }

            if (mainFile == null || !File.Exists(mainFile))
            {
                return false;
            }

            return true;
        }

        public override void Setup()
        {
            SetupAndExtract();
            SetupRequirementsWithPip();
        }

        private void SetupAndExtract()
        {
            // download file
            string url = "https://github.com/Ableytner/Minecraft-blockcounter/archive/main.zip";
            string zipFile = FileHelper.DownloadFile(url, "Minecraft-blockcounter-main.zip");

            // extract
            string extractPath = FileHelper.GetProgramsPath();
            FileHelper.UnzipFile(zipFile, extractPath);

            // copy all files
            extractPath = Path.Combine(extractPath, Path.GetFileNameWithoutExtension(zipFile));
            FileHelper.MoveAllFolderContents(extractPath, FileHelper.GetProgramsPath(Name));

            // delete old folder
            FileHelper.DeleteFolder(extractPath);
        }

        private void SetupRequirementsWithPip()
        {
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            // install requirements with pip
            pythonProgram.InstallWithPip("-r requirements.txt", FileHelper.GetProgramsPath(Name));
        }
    }
}
