using DGE.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;

namespace DGE.Console
{
    public static class UpdaterCommands
    {
        public static void Create()
        {
            Commands.AddCommand(new FrameworkCommand("au", (a) =>
            {
                string result = Execute(a, out bool interactive);

                if (interactive)
                {
                    if (a is null || a.Length == 0)
                    {
                        Commands.logger.Log("Running auto update procedure", EnderEngine.Logger.LogLevel.INFO);

                        Updater.UpdateManager.StartUpdater();

                        Commands.logger.Log("Fetching latest project versions", EnderEngine.Logger.LogLevel.INFO);

                        Updater.UpdateManager.Fetch("all");
                        if (Updater.UpdateManager.isUpdateAvailable)
                        {
                            Commands.logger.Log("Fetched (new version(s) avaliable) - Downloading updates", EnderEngine.Logger.LogLevel.INFO);
                            Updater.UpdateManager.Download("all");
                            if (Updater.UpdateManager.isUpdateDownloaded)
                            {
                                Commands.logger.Log("A new update was downloaded : Do you want to restart and update the application ? (y/n)", EnderEngine.Logger.LogLevel.INFO);
                                if (System.Console.ReadKey().Key == ConsoleKey.Y)
                                {
                                    System.Console.Write('\n');
                                    Updater.UpdateManager.StartUpdateScript();
                                    return null;
                                }
                            }
                        }
                        return "Canceled - No update available";

                    }
                    else if (a.Length > 0)
                    {
                        string action = a[0].ToLower().Trim();
                        if (action == "i" || action == "install")
                        {
                            Commands.logger.Log("Running interactive install procedure : Do you want to restart the application ? (y/n)", EnderEngine.Logger.LogLevel.WARN);
                            if (System.Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                System.Console.Write('\n');
                                Updater.UpdateManager.StartUpdateScript();
                            }
                            else
                                return "Canceled interactive procedure";
                        }
                    }
                }

                return result;
            }, "Starts the DGE Updater, runs interactive procedure if no args, otherwise :\n\ts/start, q/quit/exit, w/write - write command to updater, i/install - installs downloaded version,\n\tf/fetch - searches for latest update, d/download - downloads latest update"));
        }

        public static string Execute(string[] args, out bool interactive, Action<string, EnderEngine.Logger.LogLevel> logCallback = null)
        {
            // If only i knew how to write clean code (Command provider - Named arguments - Easier help command)
            interactive = false;
            if (args is null || args.Length == 0)
            {
                interactive = true;
                return "Base interactive procedure";
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
                else if (action == "i" || action == "install")
                {
                    interactive = true;
                    return "Install interactive procedure";
                }
                else if (action == "f" || action == "fetch")
                {
                    string fetchArgs = args.Length >= 2 ? string.Join(' ', args, 2, args.Length - 1) : "all";
                    Updater.UpdateManager.Fetch(fetchArgs);
                    return null;
                }
                else if (action == "d" || action == "download")
                {
                    string downloadArgs = args.Length >= 2 ? string.Join(' ', args, 2, args.Length - 1) : "all";
                    Updater.UpdateManager.Download(downloadArgs);
                    return null;
                }
            }
            return "A problem occured parsing arguments";
        }

    }
}
