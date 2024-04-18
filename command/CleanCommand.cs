using BlockCounterCLI.helper;
using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
using System.IO;

namespace BlockCounterCLI.command
{
    internal class CleanCommand : BaseCommand
    {
        public static new string Prefix = "clean";
        public static new string Description = "delete all downloaded files and programs, thus resetting this application";

        public CleanCommand(string[] args)
        {
            targets = ArgsHelper.ParsePrograms(args);
        }

        private readonly BaseProgram[] targets;


        public override void Execute()
        {
            if (targets.Length == 0)
            {
                SetupHelper.RemoveAll();
            }
            else
            {
                SetupHelper.Remove(targets);
            }

            Directory.Delete(FileHelper.GetDownloadPath(), true);
            Directory.Delete(FileHelper.GetProgramsPath(), true);
        }
    }
}
