using BlockCounterCLI.helper;
using BlockCounterCLI.program;
using System;

namespace BlockCounterCLI.command
{
    internal class SetupCommand : BaseCommand
    {
        public static new string Prefix = "setup";
        public static new string Description = "downloads/installs all unavailable necessary programs";

        public SetupCommand(string[] args)
        {
            this.args = args;
        }

        private readonly string[] args;

        public override void Execute()
        {
            BaseProgram[] targets = ArgsHelper.ParsePrograms(args);
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
