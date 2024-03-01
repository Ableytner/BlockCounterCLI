using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.helper
{
    internal class ProcessHelper
    {
        public static void RunCommand(string command, string parameters)
        {
            Console.WriteLine("Running " + command + " " + parameters);
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.Start();
            pProcess.WaitForExit();
        }

        public static void RunCommand(string command, string parameters, string working_directory)
        {
            Console.WriteLine("Running " + command + " " + parameters);
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
            pProcess.StartInfo.WorkingDirectory = working_directory;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.Start();
            pProcess.WaitForExit();
        }

        public static string RunCommandWithOutput(string command, string parameters)
        {
            Console.WriteLine("Running " + command + " " + parameters);
            Process pProcess = new Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = parameters;
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
    }
}
