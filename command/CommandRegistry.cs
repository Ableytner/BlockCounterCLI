﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class CommandRegistry
    {
        private Dictionary<string,Type> commands;

        public CommandRegistry() {
            commands = new Dictionary<string, Type>();
        }

        public void RegisterCommand(Type command) {
            string prefix = null;
            try
            {
                prefix = GetPrefixFromCommandType(command);
            }
            catch { }
            prefix = GetPrefixFromCommandType(command);

            if (prefix == null || prefix == string.Empty)
            {
                throw new ArgumentException("Tried to register command with invalid prefix " + prefix);
            }

            if (commands.ContainsKey(prefix))
            {
                throw new ArgumentException("Tried to register command with already existing prefix " + prefix);
            }
            if (command == null || !command.IsSubclassOf(typeof(BaseCommand)))
            {
                throw new ArgumentException("Tried to register command with invalid type " +  command.ToString());
            }

            string description = null;
            try
            {
                description = GetDescriptionFromCommandType(command);
            }
            catch { }
            if (description == null || description == string.Empty)
            {
                throw new ArgumentException("Tried to register command " + prefix + " without description");
            }

            commands.Add(prefix, command);
        }

        public Type GetCommandType(string prefix)
        {
            if (prefix == null || prefix == string.Empty)
            {
                return null;
            }
            if (!commands.ContainsKey(prefix))
            {
                return null;
            }
            return commands[prefix];
        }

        public Type[] GetCommandTypes()
        {
            return commands.Values.ToArray();
        }

        public string GetPrefixFromCommandType(Type command)
        {
            return (string) command.GetField("prefix", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }

        public string GetDescriptionFromCommandType(Type command)
        {
            return (string)command.GetField("description", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }
    }
}
