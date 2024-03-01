using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BlockCounterCLI.command
{
    internal class ExitCommand : BaseCommand
    {
        public static new string prefix = "exit";
        public static new string description = "stops the program";

        public ExitCommand(string[] args) { }

        public override void Execute()
        {
            Environment.Exit(0);
        }
    }
}
