using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class ReinstallCommand : BaseCommand
    {
        public static new string prefix = "reinstall";
        public static new string description = "deletes and reinstalls all necessary programs";

        public ReinstallCommand(string[] args) { }

        public override void Execute()
        {
            DeleteData();

            foreach (var program in ProgramRegistry.Instance.GetPrograms())
            {
                Console.WriteLine("Setting up " + program.Name);
                program.Setup();
            }
        }

        private void DeleteData()
        {
            Console.WriteLine("Removing download folder");
            string downloadFolder = FileHelper.GetDownloadPath();
            if (Directory.Exists(downloadFolder))
            {
                Directory.Delete(downloadFolder, true);
            }
            Console.WriteLine("Removing programs folder");
            string programsFolder = FileHelper.GetProgramsPath();
            if (Directory.Exists(programsFolder))
            {
                Directory.Delete(programsFolder, true);
            }
        }
    }
}
