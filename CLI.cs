using BlockCounterCLI.command;
using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BlockCounterCLI
{
    internal class CLI
    {
        public static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsDebugMode = false;

        public void Setup()
        {
            // check OS
            if (!IsWindows && !IsLinux)
            {
                throw new Exception("Current OS '" + RuntimeInformation.OSDescription + "' is not supported");
            }

            // create singletons
            if (DataStore.Instance == null || CommandRegistry.Instance == null || ProgramRegistry.Instance == null)
            {
                throw new Exception("Could not create singletons");
            }

            CommandRegistry.Instance.RegisterCommand(typeof(HelpCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(StatusCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(SetupCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(ReinstallCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(CleanCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(GetOldMappings));
            CommandRegistry.Instance.RegisterCommand(typeof(CountCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(ClearCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(ExitCommand));

            ProgramRegistry.Instance.RegisterProgram(new JavaProgram());
            ProgramRegistry.Instance.RegisterProgram(new PrismProgram());
            ProgramRegistry.Instance.RegisterProgram(new PythonProgram());
            ProgramRegistry.Instance.RegisterProgram(new McServerWrapperProgram());
            ProgramRegistry.Instance.RegisterProgram(new MinecraftBlockcounterProgram());

            AppDomain.CurrentDomain.ProcessExit +=
                (sender, eventArgs) => AtExit();
        }

        public void RunLoop()
        {
            while (true)
            {
                Console.WriteLine(string.Concat(Enumerable.Repeat("-", 80)));
                Console.Write("> ");
                string userIn = Console.ReadLine();
                Console.WriteLine(string.Concat(Enumerable.Repeat("-", 80)));
                try
                {
                    string result = HandleCommand(userIn);
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Exception during command handling: " + ex.Message);
                    if (IsDebugMode)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        public static void AtExit()
        {
            McServerWrapperProgram mcServerWrapper = ProgramRegistry.Instance.GetProgram(typeof(McServerWrapperProgram));
            mcServerWrapper?.Stop();

            foreach (BaseProgram program in ProgramRegistry.Instance.GetPrograms())
            {
                program.AtExit();
            }
        }

        private string HandleCommand(string rawCommand)
        {
            string prefix = rawCommand;
            string[] args = Array.Empty<string>();

            if (rawCommand.Contains(' '))
            {
                // code from https://stackoverflow.com/a/4780801/15436169
                string[] parts = Regex.Split(rawCommand, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                parts = parts.Select(f => f.Trim('\"')).ToArray();
                prefix = parts[0];
                args = parts.Skip(1).ToArray();
            }

            Type commandType = CommandRegistry.Instance.GetCommandType(prefix);
            if (commandType == null)
            {
                return "Command '" + rawCommand + "' not found";
            }

            BaseCommand cmd = Activator.CreateInstance(commandType, [args]) as BaseCommand;

            foreach (Type dep in cmd.DependsOn)
            {
                BaseProgram program = ProgramRegistry.Instance.GetProgram(dep);
                if (program == null || !program.IsSetup())
                {
                    return "Command depends on missing program " + program.Name;
                }
            }

            cmd.Execute();

            if (cmd.HasErrored())
            {
                // return error message
                // this is currently stored the same way as a normal result message
                return cmd.GetResultMessage();
            }
            return cmd.GetResultMessage();
        }
    }
}
