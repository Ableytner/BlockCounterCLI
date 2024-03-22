using BlockCounterCLI.command;
using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
            CommandRegistry.Instance.RegisterCommand(typeof(ClearCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(ExitCommand));

            ProgramRegistry.Instance.RegisterProgram(new JavaProgram());
            ProgramRegistry.Instance.RegisterProgram(new PrismProgram());
            ProgramRegistry.Instance.RegisterProgram(new PythonProgram());
            ProgramRegistry.Instance.RegisterProgram(new McServerWrapperProgram());
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

        private string HandleCommand(string rawCommand)
        {
            string prefix = rawCommand;
            string[] args = new string[0];

            if (rawCommand.Contains(' '))
            {
                prefix = rawCommand.Split(' ')[0];
                args = rawCommand.Split(' ').Skip(1).ToArray();
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

            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                // return error
                if (cmd.GetResultMessage() != "")
                {
                    return "Command errored: " + cmd.GetResultMessage();
                }
                else
                {
                    return "Command errored: " + e.Message;
                }
            }

            if (cmd.HasErrored())
            {
                // return error
                return "Command errored: " + cmd.GetResultMessage();
            }
            return cmd.GetResultMessage();
        }
    }
}
