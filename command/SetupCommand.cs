using BlockCounterCLI.helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class SetupCommand : BaseCommand
    {
        public static new string prefix = "setup";
        public static new string description = "downloads/installs up all needed programs";

        public SetupCommand(CommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        protected CommandRegistry commandRegistry;

        public override void Execute()
        {
            SetupPrism();
        }

        private void SetupPrism()
        {
            string url = "https://github.com/PrismLauncher/PrismLauncher/releases/download/8.0/PrismLauncher-Windows-MSVC-Portable-8.0.zip";
            string prism_zip = FileHelper.DownloadFile(url);

            string prism_dir = Path.Combine(Directory.GetParent(Path.GetDirectoryName(prism_zip)).FullName, "programs", "prism");
            if (Directory.Exists(prism_dir))
            {
                Directory.Delete(prism_dir, true);
            }
            Directory.CreateDirectory(prism_dir);
            FileHelper.UnzipFile(prism_zip, prism_dir);

            Console.WriteLine(prism_dir);
        }
    }
}
