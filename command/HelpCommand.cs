using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BlockCounterCLI.command
{
    internal class HelpCommand : BaseCommand
    {
        public static new string prefix = "help";
        public static new string description = "Prints out an overview over all available commands";

        public HelpCommand(string[] args) { }

        public override void Execute()
        {
            resultMessage = "Showing help for " + CommandRegistry.Instance.GetCommandTypes().Length.ToString() + " commands:\n";

            List<string> helpMessages = new List<string>();
            foreach (var command in CommandRegistry.Instance.GetCommandTypes())
            {
                helpMessages.Add(CommandRegistry.Instance.GetPrefixFromCommandType(command) + ": "
                                 + CommandRegistry.Instance.GetDescriptionFromCommandType(command));
            }
            resultMessage += string.Join("\n", helpMessages);
        }
    }
}
