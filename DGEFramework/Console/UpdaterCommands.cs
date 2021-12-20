using DGE.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Console
{
    public static class UpdaterCommands
    {
        public static void Create()
        {
            Commands.CreateCommand("au", (a) =>
            {
                return Execute(a);
            });
        }

        public static string Execute(string[] args, Action<string, EnderEngine.Logger.LogLevel> logCallback = null)
        {
            if (args is null || args.Length == 0)
            {
                return "No args were entered, action not implemented";
            }
            else if (args.Length > 0)
            {
                string action = args[0].ToLower().Trim();
                if (action == "s" || action == "start")
                {
                    Updater.UpdateManager.StartUpdater(logCallback);
                    return null;
                }
                else if (action == "w" || action == "write")
                {
                    return Updater.UpdateManager.WriteToUpdater(string.Join(' ', args, 1, args.Length - 1));
                    
                }
                else if (action == "q" || action == "quit" || action == "exit")
                {
                    Updater.UpdateManager.StopUpdater();
                    return null;
                }
            }
            return "A problem occured parsing arguments";
        }

    }
}
