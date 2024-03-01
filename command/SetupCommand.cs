using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class SetupCommand : BaseCommand
    {
        public static new string prefix = "setup";
        public static new string description = "downloads/installs all unavailable necessary programs";

        public SetupCommand(string[] args) { }

        public override void Execute()
        {
            Console.WriteLine("Removing download folder");
            string downloadFolder = FileHelper.GetDownloadPath();
            if (Directory.Exists(downloadFolder))
            {
                Directory.Delete(downloadFolder, true);
            }

            foreach (var program in ProgramRegistry.Instance.GetPrograms())
            {
                if (!program.IsSetup())
                {
                    string programFolder = FileHelper.GetProgramsPath(program.Name);
                    if (Directory.Exists(programFolder))
                    {
                        Console.WriteLine("Deleting " + program.Name);
                        Directory.Delete(programFolder, true);
                    }

                    Console.WriteLine("Setting up " + program.Name);
                    program.Setup();
                }
                else
                {
                    Console.WriteLine("Skipping already installed program " + program.Name);
                }
            }
        }
    }
}
