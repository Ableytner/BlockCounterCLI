using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
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

        public SetupCommand(string[] args)
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

            Console.WriteLine("Removing download folder");
            string downloadFolder = FileHelper.GetDownloadPath();
            if (Directory.Exists(downloadFolder))
            {
                Directory.Delete(downloadFolder, true);
            }

            foreach (var program in ProgramRegistry.Instance.GetPrograms())
            {
                if (targets.Length == 0 || targets.Contains(program.Name.ToLower()))
                {
                    SetupProgram(program);
                }
            }
        }

        private void SetupProgram(BaseProgram program)
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
