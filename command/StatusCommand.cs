using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class StatusCommand : BaseCommand
    {
        public static new string prefix = "status";
        public static new string description = "prints the current install status of all needed programs";

        public StatusCommand(string[] args) { }

        public override void Execute()
        {
            PrintAllStatuses();
        }

        private void PrintAllStatuses()
        {
            foreach (var program in ProgramRegistry.Instance.GetPrograms())
            {
                Console.Write(program.Name + " - ");
                if (program.IsSetup())
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("setup");
                }
                else
                {
                    string path = Path.Combine(FileHelper.GetProgramsPath(), program.Name);
                    if (Directory.Exists(path))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("corrupted");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("not installed");
                    }
                }
                Console.ResetColor();
            }
        }
    }
}
