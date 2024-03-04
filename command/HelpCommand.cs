using System.Collections.Generic;

namespace BlockCounterCLI.command
{
    internal class HelpCommand : BaseCommand
    {
        public static new string Prefix = "help";
        public static new string Description = "Prints out an overview over all available commands";

        public HelpCommand(string[] args) { }

        public override void Execute()
        {
            ResultMessage = "Showing help for " + CommandRegistry.Instance.GetCommandTypes().Length.ToString() + " commands:\n";

            List<string> helpMessages = new List<string>();
            foreach (var command in CommandRegistry.Instance.GetCommandTypes())
            {
                helpMessages.Add(CommandRegistry.Instance.GetPrefixFromCommandType(command)
                                 + ": "
                                 + CommandRegistry.Instance.GetDescriptionFromCommandType(command));
            }
            ResultMessage += string.Join("\n", helpMessages);
        }
    }
}
