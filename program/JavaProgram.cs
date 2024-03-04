using BlockCounterCLI.helper;
using BlockCounterCLI.program;
using System;
using System.IO;

namespace BlockCounterCLI.helpers
{
    internal class JavaProgram : BaseProgram
    {
        public string java8Executable;
        public string java17Executable;

        public JavaProgram()
        {
            // setup paths
            java8Executable = Path.Combine(FileHelper.GetProgramsPath(), Name, "jdk8u402-b06-jre", "bin", "java.exe");
            java17Executable = Path.Combine(FileHelper.GetProgramsPath(), Name, "jdk-17.0.10+7-jre", "bin", "java.exe");
        }

        public override string Name => "Java";

        public override Type[] DependsOn => new Type[0];

        public override bool IsSetup()
        {
            if (!CheckJavaInstallation(java8Executable, "-version", "openjdk version \"1.8.0_402\""))
            {
                return false;
            }
            if (!CheckJavaInstallation(java17Executable, "-version", "openjdk version \"17.0.10\""))
            {
                return false;
            }
            return true;
        }

        public override void Setup()
        {
            DownloadAndExtract();
        }

        private bool CheckJavaInstallation(string jarPath, string parameters, string expectedVersion)
        {
            if (!File.Exists(jarPath))
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(jarPath, parameters);

            return output.Contains(expectedVersion);
        }

        private void DownloadAndExtract()
        {
            // download files
            string url = "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u402-b06/OpenJDK8U-jre_x64_windows_hotspot_8u402b06.zip";
            string java8Zip = FileHelper.DownloadFile(url);

            url = "https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.10%2B7/OpenJDK17U-jre_x86-32_windows_hotspot_17.0.10_7.zip";
            string java17Zip = FileHelper.DownloadFile(url);

            // extract files
            FileHelper.UnzipFile(java8Zip, FileHelper.GetProgramsPath(Name));
            FileHelper.UnzipFile(java17Zip, FileHelper.GetProgramsPath(Name));
        }
    }
}
