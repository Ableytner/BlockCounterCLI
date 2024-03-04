using System;

namespace BlockCounterCLI.command
{
    internal class ClearCommand : BaseCommand
    {
        public static new string Prefix = "clear";
        public static new string Description = "clear the terminal screen";

        public ClearCommand(string[] args) { }

        public override void Execute()
        {
            Console.Clear();
        }
    }
}
