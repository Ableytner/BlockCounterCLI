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
            List<string> lines = new List<string>();

            lines.Add("Showing help for " + CommandRegistry.Instance.GetCommandTypes().Length.ToString() + " commands:\n");
            foreach (var command in CommandRegistry.Instance.GetCommandTypes())
            {
                lines.Add(CommandRegistry.Instance.GetPrefixFromCommandType(command)
                                 + ": "
                                 + CommandRegistry.Instance.GetDescriptionFromCommandType(command));
            }

            lines.Add("");

            lines.Add("Showing " + ProgramRegistry.Instance.GetPrograms().Length.ToString() + " available programs:\n");
            foreach (var program in ProgramRegistry.Instance.GetPrograms())
            {
                lines.Add(program.Name);
            }
            ResultMessage += string.Join("\n", lines);
        }
    }
}
