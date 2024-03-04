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
            Environment.Exit(0);
        }
    }
}
