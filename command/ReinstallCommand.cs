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
    internal class ReinstallCommand : BaseCommand
    {
        public static new string prefix = "reinstall";
        public static new string description = "deletes and reinstalls all necessary programs";

        public ReinstallCommand(string[] args)
        {
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;

        public override void Execute()
        {
            if (targets.Length == 0)
            {
                SetupHelper.RemoveAll();
                SetupHelper.SetupAll();
            }
            else
            {
                SetupHelper.Remove(targets);
                SetupHelper.Setup(targets);
            }
        }
    }
}
