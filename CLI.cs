using BlockCounterCLI.command;
using BlockCounterCLI.helpers;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI
{
    internal class CLI
    {
        public void Setup()
        {
            // create singletons
            if (DataStore.Instance == null || CommandRegistry.Instance == null || ProgramRegistry.Instance == null)
            {
                throw new Exception("Could not create singletons");
            }

            CommandRegistry.Instance.RegisterCommand(typeof(HelpCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(StatusCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(SetupCommand));
            CommandRegistry.Instance.RegisterCommand(typeof(ReinstallCommand));
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
                    if (Program.DEBUG_MODE)
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
                prefix = rawCommand.Split(new char[1] { ' ' }, 1)[0];
                args = rawCommand.Split(new char[1] { ' ' }).Skip(1).ToArray();
            }

            Type commandType = CommandRegistry.Instance.GetCommandType(prefix);
            if (commandType == null)
            {
                return "Command " + rawCommand + " not found";
            }

            BaseCommand cmd = Activator.CreateInstance(commandType, new object[1] { args }) as BaseCommand;

            cmd.Execute();

            if (cmd.HasErrored())
            {
                // return error
                return "Command errored: " + cmd.GetResultMessage();
            }
            return cmd.GetResultMessage();
        }
    }
}
