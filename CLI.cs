using BlockCounterCLI.command;
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
        private CommandRegistry commandRegistry;

        public void Setup()
        {
            commandRegistry = new CommandRegistry();
            commandRegistry.RegisterCommand(typeof (HelpCommand));
            commandRegistry.RegisterCommand(typeof (SetupCommand));
            commandRegistry.RegisterCommand(typeof(ExitCommand));
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
                    Console.WriteLine(result);
                }
                catch (Exception ex) {
                    Console.WriteLine("Exception during command handling: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
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

            Type commandType = commandRegistry.GetCommandType(prefix);
            if (commandType == null)
            {
                return "Command " + rawCommand + " not found";
            }

            BaseCommand cmd = Activator.CreateInstance(commandType, new object[1] { commandRegistry }) as BaseCommand;

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
