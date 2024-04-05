using BlockCounterCLI.command;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using Python.Runtime;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BlockCounterCLI.program
{
    internal class McServerWrapperProgram : BaseProgram
    {
        public string serverJar;
        public PyModule session;

        public McServerWrapperProgram()
        {
            serverJar = Path.Combine(FileHelper.GetProgramsPath(), Name, "server", "server-forge.jar");

            if (IsSetup())
            {
                SetupPythonSession();
            }
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
            SetupAndExtract();
            InstallWithPip();
            SetupMcServer();
            SetupPythonSession();
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

        public void Stop()
        {
            session.TryGet("wrapper", out PyObject wrapper);
            if (wrapper != null)
            {
                session.Exec("child_status = wrapper.server.get_child_status(1)");
                PyObject childStatus = session.Get("child_status");
                // if childStatus is None, the server has not yet exited
                if (childStatus.IsNone())
                {
                    if (CLI.IsDebugMode)
                    {
                        Console.WriteLine("Integrated server is running, stopping it...");
                    }

                    session.Exec("wrapper.stop()");
                }
            }
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
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            // install from local files
            string args = "-m pip install .";
            string wrapperPath = FileHelper.GetProgramsPath(Name);

            // install from github
            // doesn't work, because git is not installed
            //string args = "-m pip install git+https://github.com/mcserver-tools/mcserverwrapper.git";

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

            foreach(var file in Directory.GetFiles(serverPath))
            {
                if (Regex.IsMatch(file, @"forge-.*\.jar"))
                {
                    FileHelper.RenameFile(file, serverJar);
                }
            }

            // add bidtoname mod
            /*url = "https://github.com/Ableytner/blockid-to-name/releases/download/1.5.0/bidtoname-1.5.0.jar";
            string bidtoname = FileHelper.DownloadFile(url);

            string modsPath = Path.Combine(serverPath, "mods");
            Directory.CreateDirectory(modsPath);
            FileHelper.CopyFile(bidtoname, Path.Combine(modsPath, Path.GetFileName(bidtoname)));*/
        }

        private void SetupPythonSession()
        {
            if (ProgramRegistry.Instance.GetProgram(typeof(PythonProgram)) == null)
            {
                throw new Exception("PythonProgram needs to be registered first");
            }

            using (Py.GIL())
            {
                session = Py.CreateScope();
            }
        }
    }
}
