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
            McServerWrapperProgram mcServerWrapper = ProgramRegistry.Instance.GetProgram(typeof(McServerWrapperProgram));
            mcServerWrapper?.Stop();

            foreach (BaseProgram program in ProgramRegistry.Instance.GetPrograms())
            {
                program.AtExit();
            }

            Environment.Exit(0);
        }
    }
}
