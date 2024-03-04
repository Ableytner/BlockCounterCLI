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

        public ReinstallCommand(string[] args)
        {
            if (args.Length == 0)
            {
                targets = new string[0];
            }
            else
            {
                foreach (string arg in args)
                {
                    if (ProgramRegistry.Instance.GetProgram(arg) == null)
                    {
                        resultMessage = "Could not find program " + arg;
                        errored = true;
                        return;
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = args[i].ToLower();
                }

                targets = args;
            }
        }

        private readonly string[] targets;

        public override void Execute()
        {
            if (errored)
            {
                return;
            }

            if (targets.Length == 0)
            {
                DeleteData();

                foreach (var program in ProgramRegistry.Instance.GetPrograms())
                {
                    Console.WriteLine("Setting up " + program.Name);
                    program.Setup();
                }

            }
            else
            {
                foreach (var program in ProgramRegistry.Instance.GetPrograms())
                {
                    if (targets.Contains(program.Name.ToLower()))
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
                }
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
