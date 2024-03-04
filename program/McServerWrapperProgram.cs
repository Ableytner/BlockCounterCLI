using BlockCounterCLI.command;
using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.program
{
    internal class McServerWrapperProgram : BaseProgram
    {
        public McServerWrapperProgram() { }

        public override string Name => "McServerWrapper";

        public override bool IsSetup()
        {
            return false;
        }

        public override void Setup()
        {
            SetupAndExtract();
            InstallWithPip();
        }

        private void SetupAndExtract()
        {
            // download file
            string url = "https://github.com/mcserver-tools/mcserverwrapper/archive/refs/heads/main.zip";
            string mcsw_zip = FileHelper.DownloadFile(url);

            // extract
            string mcsw_extract_path = FileHelper.GetProgramsPath();
            FileHelper.UnzipFile(mcsw_zip, mcsw_extract_path);


            // copy all files
            mcsw_extract_path = Path.Combine(mcsw_extract_path, "mcserverwrapper-main");
            string mcsw_path = FileHelper.GetProgramsPath(Name);
            FileHelper.MoveAllFolderContents(mcsw_extract_path, mcsw_path);

            // delete old folder
            FileHelper.DeleteFolder(mcsw_extract_path);
        }

        private void InstallWithPip()
        {
            // variables
            string args = "-m pip install .";
            string wrapperPath = FileHelper.GetProgramsPath(Name);
            PythonProgram pythonProgram = ProgramRegistry.Instance.GetProgram(typeof(PythonProgram));

            Console.WriteLine(pythonProgram.python_executable);
            Console.WriteLine(args);
            Console.WriteLine(wrapperPath);
            // install
            ProcessHelper.RunCommand(pythonProgram.python_executable, args, wrapperPath);
        }
    }
}
