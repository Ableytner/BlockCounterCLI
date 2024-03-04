using BlockCounterCLI.helper;
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
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;

        public override void Execute()
        {
            if (targets.Length == 0)
            {
                SetupHelper.SetupAll();
            }
            else
            {
                SetupHelper.Setup(targets);
            }
        }
    }
}
