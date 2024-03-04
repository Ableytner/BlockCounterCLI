using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.IO;

namespace BlockCounterCLI.program
{
    internal class PythonProgram : BaseProgram
    {
        public string pythonExecutable;

        public PythonProgram()
        {
            pythonExecutable = Path.Combine(FileHelper.GetProgramsPath(), Name, "python.exe");
        }

        public override string Name => "Python";

        public override Type[] DependsOn => new Type[0];

        public override bool IsSetup()
        {
            if (!File.Exists(pythonExecutable))
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(pythonExecutable, "--version");
            if (!output.Contains("Python 3.12.2"))
            {
                return false;
            }

            output = ProcessHelper.RunCommandWithOutput(pythonExecutable, "-m pip list");
            if (!output.Contains("setuptools"))
            {
                return false;
            }

            return true;
        }

        public override void Setup()
        {
            SetupAndExtract();
            SetupPip();
        }

        private void SetupAndExtract()
        {
            // download files
            string url = "https://www.python.org/ftp/python/3.12.2/python-3.12.2-embed-amd64.zip";
            string pythonZip = FileHelper.DownloadFile(url);

            // extract
            string pythonPath = FileHelper.GetProgramsPath(Name);
            FileHelper.UnzipFile(pythonZip, pythonPath);
        }

        // pip setup from https://stackoverflow.com/a/55271031/15436169
        private void SetupPip()
        {
            // download files
            string url = "https://bootstrap.pypa.io/get-pip.py";
            string getPipDl = FileHelper.DownloadFile(url);

            // copy
            string pythonPath = FileHelper.GetProgramsPath(Name);
            string getPip = Path.Combine(pythonPath, Path.GetFileName(getPipDl));
            FileHelper.CopyFile(getPipDl, getPip);

            // run
            ProcessHelper.RunCommand(pythonExecutable, getPip, pythonPath);

            // add paths to _pth
            string pthFile = Path.Combine(pythonPath, "python312._pth");
            string pthContent = File.ReadAllText(pthFile);
            pthContent += "\n";
            pthContent += pythonPath + "\n";
            pthContent += Path.Combine(pythonPath, "DLLs") + "\n";
            pthContent += Path.Combine(pythonPath, "lib") + "\n";
            pthContent += Path.Combine(pythonPath, "lib", "plat-win") + "\n";
            pthContent += Path.Combine(pythonPath, "lib", "site-packages") + "\n";
            File.WriteAllText(pthFile, pthContent);
        }
    }
}
