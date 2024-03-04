using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlockCounterCLI.command
{
    internal class CleanCommand : BaseCommand
    {
        public static new string prefix = "clean";
        public static new string description = "delete all downloaded files and programs, thus resetting this application";

        public CleanCommand(string[] args)
        {
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;


        public override void Execute()
        {
            if (targets.Length == 0)
            {
                SetupHelper.RemoveAll();
            }
            else
            {
                SetupHelper.Remove(targets);
            }
        }
    }
}
