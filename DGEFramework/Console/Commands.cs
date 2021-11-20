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

        public static Task ExecuteCommand(string commandName, params string[] arguments)
        {
            commandName = commandName.ToLower();
            if (commandName == "exit")
                return Task.CompletedTask;
            if (arguments is null)
                arguments = new string[0];
            try
            {
                logger.Log(commands[commandName](arguments), Logger.LogLevel.INFO);
            }
            catch (Exception e)
            {
                if (e is KeyNotFoundException) //Can cause problems if the command has a KeyNotFoundException
                    logger.Log($"The command \"{commandName}\" does not exist", Logger.LogLevel.WARN);
                else
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
