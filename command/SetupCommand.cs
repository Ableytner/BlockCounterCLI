using BlockCounterCLI.helper;
using BlockCounterCLI.program;

namespace BlockCounterCLI.command
{
    internal class SetupCommand : BaseCommand
    {
        public static new string Prefix = "setup";
        public static new string Description = "downloads/installs all unavailable necessary programs";

        public SetupCommand(string[] args)
        {
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;

        public override void Execute()
        {
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
