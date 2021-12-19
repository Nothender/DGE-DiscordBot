using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DGE.Application;
using DGE.Core;
using DGE.Exceptions;
using System.Linq;
using EnderEngine;
using System.Diagnostics;
using DGE.Core.OperatingSystem;

namespace DGE.Console
{
    public static class Commands
    {
        internal static readonly Dictionary<string, Func<string[], string>> commands = new Dictionary<string, Func<string[], string>>();
        private static readonly Logger logger = new Logger("DGE-CC"); //DGE-CC for DGE ConsoleCommands

        internal static string exitCommand = "exit";

        static Commands()
        {
            CreateCommand(exitCommand, (a) =>
            {
                return "THIS STRING SHOULDNT SHOW #0"; //the #0 is the id of the string that shouldnt show, so if it ever shows ik from where it is
            });
        }

        public static Task ExecuteCommand(string commandName, params string[] arguments)
        {
            commandName = commandName.ToLower();
            if (commandName == exitCommand)
                return Task.CompletedTask;
            if (arguments is null)
                arguments = new string[0];
            try
            {
                if (!commands.TryGetValue(commandName, out Func<string[], string> commandFunction))
                {
                    logger.Log($"The command \"{commandName}\" does not exist", Logger.LogLevel.WARN);
                    return Task.CompletedTask;
                }
                string result = commandFunction(arguments);
                if (result is null)
                    return Task.CompletedTask;
                logger.Log(result, Logger.LogLevel.INFO);
            }
            catch (Exception e)
            {
                logger.Log(e.Message, Logger.LogLevel.ERROR);
            }
            return Task.CompletedTask;
        }

        public static void CreateCommand(string name, Func<string[], string> action)
        {
            if (commands.ContainsKey(name))
                return;
            commands.Add(name, action);
        }

        public static string[] GetCommands()
        {
            return commands.Keys.ToArray();
        }

    }
}
