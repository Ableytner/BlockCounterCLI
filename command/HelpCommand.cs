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

        public HelpCommand(CommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        protected CommandRegistry commandRegistry;

        public override void Execute()
        {
            resultMessage = "Showing help for " + commandRegistry.GetCommandTypes().Length.ToString() + " commands:\n";

            List<string> helpMessages = new List<string>();
            foreach (var command in commandRegistry.GetCommandTypes())
            {
                helpMessages.Add(commandRegistry.GetPrefixFromCommandType(command) + ": " + commandRegistry.GetDescriptionFromCommandType(command));
            }
            resultMessage += string.Join("\n", helpMessages);
        }
    }
}
