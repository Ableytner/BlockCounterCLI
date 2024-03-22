using System;
using System.Diagnostics;

namespace BlockCounterCLI.helper
{
    internal class ProcessHelper
    {
        public static void RunCommand(string command, string parameters, string workingDirectory = null, int timeoutMillis = -1)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Running " + command + " " + parameters);
            }
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            if (workingDirectory != null)
            {
                pProcess.StartInfo.WorkingDirectory = workingDirectory;
            }
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.Start();
            pProcess.WaitForExit(timeoutMillis);
        }

        public static void RunCommandWithShellExecute(string command, string parameters, string workingDirectory, int timeoutMillis = -1)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Running " + command + " " + parameters);
            }
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            pProcess.StartInfo.WorkingDirectory = workingDirectory;
            pProcess.StartInfo.UseShellExecute = true;
            pProcess.Start();
            pProcess.WaitForExit(timeoutMillis);
        }

        public static string RunCommandWithOutput(string command, string parameters, string workingDirectory = null)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Running " + command + " " + parameters);
            }
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            if (workingDirectory != null)
            {
                pProcess.StartInfo.WorkingDirectory = workingDirectory;
            }
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.Start();
            string stdout = pProcess.StandardOutput.ReadToEnd();
            string stderr = pProcess.StandardError.ReadToEnd();
            pProcess.WaitForExit();

            return stdout + "\n" + stderr;
        }

        public static Process StartProcess(string command, string parameters, string workingDirectory = null)
        {
            if (CLI.IsDebugMode)
            {
                Console.WriteLine("Starting " + command + " " + parameters);
            }
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            if (workingDirectory != null)
            {
                pProcess.StartInfo.WorkingDirectory = workingDirectory;
            }
            pProcess.StartInfo.UseShellExecute = true;
            pProcess.Start();

            return pProcess;
        }
    }
}
