using BlockCounterCLI.program;
using System;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using BlockCounterCLI.helper;

namespace BlockCounterCLI.helpers
{
    internal class JavaProgram : BaseProgram
    {
        public string java8_executable;
        public string java17_executable;

        public JavaProgram()
        {
            // setup paths
            java8_executable = Path.Combine(FileHelper.GetProgramsPath(Name), "jdk8u402-b06-jre", "bin", "java.exe");
            java17_executable = Path.Combine(FileHelper.GetProgramsPath(Name), "jdk-17.0.10+7-jre", "bin", "java.exe");
        }

        public override string Name => "Java";

        public override bool IsSetup()
        {
            if (!CheckJavaInstallation(java8_executable, "-version", "openjdk version \"1.8.0_402\""))
            {
                return false;
            }
            if (!CheckJavaInstallation(java17_executable, "-version", "openjdk version \"17.0.10\""))
            {
                return false;
            }
            return true;
        }

        public override void Setup()
        {
            DownloadAndExtract();
        }

        private bool CheckJavaInstallation(string jarPath, string parameters, string expected_version)
        {
            if (!File.Exists(jarPath))
            {
                return false;
            }

            string output = ProcessHelper.RunCommandWithOutput(jarPath, parameters);

            return output.Contains(expected_version);
        }

        private void DownloadAndExtract()
        {
            // download files
            string url = "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u402-b06/OpenJDK8U-jre_x64_windows_hotspot_8u402b06.zip";
            string java8_zip = FileHelper.DownloadFile(url);

            url = "https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.10%2B7/OpenJDK17U-jre_x86-32_windows_hotspot_17.0.10_7.zip";
            string java17_zip = FileHelper.DownloadFile(url);

            // extract files
            FileHelper.UnzipFile(java8_zip, FileHelper.GetProgramsPath(Name));
            FileHelper.UnzipFile(java17_zip, FileHelper.GetProgramsPath(Name));
        }
    }
}
