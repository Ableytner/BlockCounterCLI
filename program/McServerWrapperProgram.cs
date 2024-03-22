using BlockCounterCLI.command;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.IO;

namespace BlockCounterCLI.program
{
    internal class McServerWrapperProgram : BaseProgram
    {
        public string serverJar;

        public McServerWrapperProgram()
        {
            serverJar = Path.Combine(FileHelper.GetProgramsPath(), Name, "server", "server-forge.jar");
        }

        public override string Name => "McServerWrapper";

        public override Type[] DependsOn => [typeof(PythonProgram), typeof(JavaProgram)];

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
            //SetupAndExtract();
            InstallWithPip();
            SetupMcServer();
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
            //string args = "-m pip install .";
            string args = "-m pip install git+https://github.com/mcserver-tools/mcserverwrapper.git";
            string wrapperPath = FileHelper.GetProgramsPath(Name);
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            // install
            ProcessHelper.RunCommand(pythonProgram.pythonExecutable, args, wrapperPath);
        }

        private void SetupMcServer()
        {
            // download forge installer
            string url = "https://maven.minecraftforge.net/net/minecraftforge/forge/1.7.10-10.13.4.1614-1.7.10/forge-1.7.10-10.13.4.1614-1.7.10-installer.jar";
            string installerDownloadPath = FileHelper.DownloadFile(url);

            // create target directory
            string serverPath = Path.Combine(FileHelper.GetProgramsPath(Name), "server");
            Directory.CreateDirectory(serverPath);

            // copy installer file
            string installerJar = Path.Combine(serverPath, Path.GetFileName(installerDownloadPath));
            FileHelper.CopyFile(installerDownloadPath, installerJar);

            // install server
            JavaProgram javaProgram = ProgramRegistry.Instance.GetProgram(typeof(JavaProgram));
            ProcessHelper.RunCommandWithShellExecute(javaProgram.java8Executable, "-jar " + Path.GetFileName(installerJar) + " --installServer", serverPath);

            // delete installer file
            FileHelper.DeleteFile(installerJar);
            FileHelper.DeleteFile(installerJar + ".log");
        }
    }
}
