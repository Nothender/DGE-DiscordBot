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
        internal static readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        public static readonly Logger logger = new Logger("DGE-CC"); //DGE-CC for DGE ConsoleCommands

        internal static string exitCommand = "exit";

        static Commands()
        {
            AddCommand(new FrameworkCommand("exitCommand", (a) =>
            {
                return "THIS STRING SHOULDNT SHOW #0"; //the #0 is the id of the string that shouldnt show, so if it ever shows ik from where it is
            }, "Shuts down the application"));
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
                if (!commands.TryGetValue(commandName, out ICommand command))
                {
                    logger.Log($"The command \"{commandName}\" does not exist", Logger.LogLevel.WARN);
                    return Task.CompletedTask;
                }
                string result = command.Execute(arguments);
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

        public static void AddCommand(ICommand command)
        {
            if (commands.ContainsKey(command.Name))
            {
                AssemblyFramework.logger.Log(
                    $"Couldn't add command {command.Name} because a command with that name already exists :\n> {commands[command.Name].Name} - {commands[command.Name].Description}",
                    Logger.LogLevel.ERROR
                );
                return;
            }
            commands.Add(command.Name, command);
        }

        public static string[] GetCommandNames()
        {
            return commands.Keys.ToArray();
        }

        public static ICommand[] GetCommands()
        {
            return commands.Values.ToArray();
        }

    }
}
