using BlockCounterCLI.helper;
using BlockCounterCLI.program;

namespace BlockCounterCLI.command
{
    internal class ReinstallCommand : BaseCommand
    {
        public static new string Prefix = "reinstall";
        public static new string Description = "deletes and reinstalls all necessary programs";

        public ReinstallCommand(string[] args)
        {
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;

        public override void Execute()
        {
            if (targets.Length == 0)
            {
                SetupHelper.RemoveAll();
                SetupHelper.SetupAll();
            }
            else
            {
                SetupHelper.Remove(targets);
                SetupHelper.Setup(targets);
            }
        }
    }
}
