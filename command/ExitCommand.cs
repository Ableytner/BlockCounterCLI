using BlockCounterCLI.program;
using System;

namespace BlockCounterCLI.command
{
    internal class ExitCommand : BaseCommand
    {
        public static new string Prefix = "exit";
        public static new string Description = "stops the program";

        public ExitCommand(string[] args) { }

        public override void Execute()
        {
            CLI.AtExit();

            Environment.Exit(0);
        }
    }
}
