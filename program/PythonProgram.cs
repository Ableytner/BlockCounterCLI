using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.program
{
    internal class PythonProgram : BaseProgram
    {
        public string python_executable;

        public PythonProgram()
        {
            python_executable = Path.Combine(FileHelper.GetProgramsPath(), Name, "python.exe");
        }

        public override string Name => "Python";

        public override Type[] DependsOn => new Type[0];

        public override bool IsSetup()
        {
            if (!File.Exists(python_executable))
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(python_executable, "--version");
            if (!output.Contains("Python 3.12.2"))
            {
                return false;
            }

            output = ProcessHelper.RunCommandWithOutput(python_executable, "-m pip list");
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
            string python_zip = FileHelper.DownloadFile(url);

            // extract
            string python_path = FileHelper.GetProgramsPath(Name);
            FileHelper.UnzipFile(python_zip, python_path);
        }

        // pip setup from https://stackoverflow.com/a/55271031/15436169
        private void SetupPip()
        {
            // download files
            string url = "https://bootstrap.pypa.io/get-pip.py";
            string get_pip_dl = FileHelper.DownloadFile(url);

            // copy
            string python_path = FileHelper.GetProgramsPath(Name);
            string get_pip = Path.Combine(python_path, Path.GetFileName(get_pip_dl));
            FileHelper.CopyFile(get_pip_dl, get_pip);

            // run
            ProcessHelper.RunCommand(python_executable, get_pip, python_path);

            // add paths to _pth
            string pth_file = Path.Combine(python_path, "python312._pth");
            string pth_content = File.ReadAllText(pth_file);
            pth_content += "\n";
            pth_content += python_path + "\n";
            pth_content += Path.Combine(python_path, "DLLs") + "\n";
            pth_content += Path.Combine(python_path, "lib") + "\n";
            pth_content += Path.Combine(python_path, "lib", "plat-win") + "\n";
            pth_content += Path.Combine(python_path, "lib", "site-packages") + "\n";
            File.WriteAllText(pth_file, pth_content);
        }
    }
}
