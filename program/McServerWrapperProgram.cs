using BlockCounterCLI.command;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.IO;

namespace BlockCounterCLI.program
{
    internal class McServerWrapperProgram : BaseProgram
    {
        public McServerWrapperProgram() { }

        public override string Name => "McServerWrapper";

        public override Type[] DependsOn => new Type[1] { typeof(PythonProgram) };

        public override bool IsSetup()
        {
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            if (!pythonProgram.IsSetup())
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(pythonProgram.pythonExecutable, "-m pip list");
            if (!output.Contains("mcserverwrapper"))
            {
                return false;
            }

            return true;
        }

        public override void Setup()
        {
            SetupAndExtract();
            InstallWithPip();
        }

        public override void Remove()
        {
            if (IsSetup())
            {
                PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));
                ProcessHelper.RunCommand(pythonProgram.pythonExecutable, "-m pip uninstall mcserverwrapper -y");
            }
            base.Remove();
        }

        private void SetupAndExtract()
        {
            // download file
            string url = "https://github.com/mcserver-tools/mcserverwrapper/archive/refs/heads/main.zip";
            string mcswZip = Path.Combine(FileHelper.GetDownloadPath(), "mcserverwrapper-main.zip");
            FileHelper.DownloadFile(url, mcswZip);

            // extract
            string mcswExtractPath = FileHelper.GetProgramsPath();
            FileHelper.UnzipFile(mcswZip, mcswExtractPath);

            // copy all files
            mcswExtractPath = Path.Combine(mcswExtractPath, Path.GetFileNameWithoutExtension(mcswZip));
            string mcswPath = FileHelper.GetProgramsPath(Name);
            FileHelper.MoveAllFolderContents(mcswExtractPath, mcswPath);

            // delete old folder
            FileHelper.DeleteFolder(mcswExtractPath);
        }

        private void InstallWithPip()
        {
            // variables
            string args = "-m pip install .";
            string wrapperPath = FileHelper.GetProgramsPath(Name);
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            // install
            ProcessHelper.RunCommand(pythonProgram.pythonExecutable, args, wrapperPath);
        }
    }
}
