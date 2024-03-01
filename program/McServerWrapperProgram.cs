using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
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
            SetupVenv();
        }

        private void SetupAndExtract()
        {
            // download file
            string url = "https://github.com/mcserver-tools/mcserverwrapper/archive/refs/heads/main.zip";
            string mcsw_zip = FileHelper.DownloadFile(url);

            // extract
            string mcsw_path = FileHelper.GetProgramsPath(Name);
            FileHelper.UnzipFile(mcsw_zip, mcsw_path);
        }

        private void SetupVenv()
        {

        }
    }
}
