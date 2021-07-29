using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DGE.Application;
using DGE.Core;
using DGE.Exceptions;
using System.Linq;
using EnderEngine;

namespace DGE.Console
{
    public static class Commands
    {
        private static Dictionary<string, Func<string[], string>> commands = new Dictionary<string, Func<string[], string>>();
        private static Logger logger = new Logger("DGE-CC"); //DGE-CC for DGE ConsoleCommands

        static Commands()
        {
            CreateCommands();
        }

        public static Task ExecuteCommand(string commandName, params string[] arguments)
        {
            commandName = commandName.ToLower();
            if (commandName == "stop")
                return Task.CompletedTask;
            if (arguments is null)
                arguments = new string[0];
            try
            {
                logger.Log(commands[commandName](arguments), EnderEngine.Logger.LogLevel.INFO);
            }
            catch (Exception e)
            {
                if (e is KeyNotFoundException)
                    logger.Log($"The command {commandName} does not exist", EnderEngine.Logger.LogLevel.WARN);
                else
                    logger.Log(e.Message, EnderEngine.Logger.LogLevel.WARN);
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

        private static void CreateCommands()
        {
            //TODO: Commands may need to be improved (with structs or classes or attributes, with automatic TypeCasting and arguments passing)
            CreateCommand("help", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("help", 0, a.Length);

                string helpMessage = "Commands that exist :";
                helpMessage += "\n- " + string.Join("\n- ", commands.Keys.ToArray());
                return helpMessage;
            });
            CreateCommand("startapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("startapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(int.Parse(a[0])).Start();
                return $"Application of id {id} was started";
            });
            CreateCommand("stopapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("stopapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(int.Parse(a[0])).Stop();
                return $"Application of id {id} was stopped";
            });
            CreateCommand("showapps", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("showapps", 0, a.Length);
                string res = "Loaded apps :";
                IApplication[] apps = ApplicationManager.GetAll();
                for (int i = 0; i < apps.Length; i++)
                    res += $"\n{apps[i].GetType().Name} application of id {i}, currently {apps[i].status}";
                return res;
            });
            CreateCommand("stop", (a) =>
            {
                return "THIS STRING SHOULDNT SHOW #0"; //the #0 is the id of the string that shouldnt show, so if it ever shows ik from where it is
            });
        }

    }
}
