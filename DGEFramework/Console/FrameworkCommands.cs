using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DGE.Application;
using DGE.Console;
using DGE.Core;
using DGE.Core.OperatingSystem;
using DGE.Exceptions;

namespace DGE.Console
{
    public static class FrameworkCommands
    {

        public static void CreateHelpCommand()
        {
            Commands.AddCommand(new FrameworkCommand("help", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("help", 0, a.Length);

                StringBuilder helpMessage = new StringBuilder("Commands that exist :");
                string[,] commandHelps = new string[Commands.commands.Count(), 2];
                string[][] commandArgsDescription = new string[Commands.commands.Count()][];
                int i = 0;
                int max = 0;
                foreach (ICommand command in Commands.commands.Values)
                {
                    commandHelps[i, 0] = command.Name;
                    commandHelps[i, 1] = command.Description;
                    if (command.Name.Length > max)
                        max = command.Name.Length;
                    commandArgsDescription[i] = command.ArgumentNameDescriptions;
                    i++;
                }
                for (i = 0; i < Commands.commands.Count(); i++)
                {
                    helpMessage.Append("\n   ");
                    helpMessage.Append(commandHelps[i, 0]);
                    helpMessage.Append(' ', max - commandHelps[i, 0].Length);
                    helpMessage.Append(" : ");
                    helpMessage.Append(commandHelps[i, 1]);
                    foreach (string arg in commandArgsDescription[i])
                    {
                        helpMessage.Append("\n\t* ");
                        helpMessage.Append(arg);
                    }
                }

                return helpMessage.ToString();
            }, "Shows every command name and their description"));
        }

        public static void Create()
        {
            //TODO: Commands may need to be improved (with structs or classes or attributes, with automatic TypeCasting and arguments passing
            CreateHelpCommand();

            Commands.AddCommand(new FrameworkCommand("startapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("startapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(id).Start();
                return $"Application of id {a[0]} was started";
            }, "Starts the application of specified `index`", new string[] {"index > the app to start"}));

            Commands.AddCommand(new FrameworkCommand("stopapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("stopapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(id).Stop();
                return $"Application of id {a[0]} was stopped";
            }, "Stops app of specified `index`", new string[] {"index > the app to stop"}));

            Commands.AddCommand(new FrameworkCommand("showapps", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("showapps", 0, a.Length);
                string res = "Instanced applications :";
                foreach (IApplication app in ApplicationManager.GetAll()) res += $"\n - {app.GetType().Name} application of id {app.Id}, currently {app.status}";
                return res;
            }, "Shows every instanced application"));

            Commands.AddCommand(new FrameworkCommand("fgc", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("FGC", 0, a.Length);

                GC.Collect();
                return "Forced a garbage collection";
            }, "Forces a garbage collection"));

            Commands.AddCommand(new FrameworkCommand("showmodules", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("showmodules", 0, a.Length);

                return $"Loaded assembly DGE Modules :\n - {string.Join("\n - ", DGEModules.modules)}";
            }, "Shows every loaded module in the framework"));

            Commands.AddCommand(new FrameworkCommand("restart", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("restart", 0, a.Length);

                AssemblyFramework.logger.Log($"Current executable path is : \"{Process.GetCurrentProcess().MainModule.FileName}\"", EnderEngine.Logger.LogLevel.INFO);

                //string executableFileName; // TODO: 

                Scripts.RunApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);
                Main.OnStopped += (s, e) => Scripts.RunApp.Run();
                Main.Stop();

                return "Restarting";
            }, "Quite explicit, restarts the application"));

            Commands.AddCommand(new FrameworkCommand("pack", (a) =>
            {
                bool fullPack = a.Length > 0 ? a[0] == "-f" || a[0] == "-full" : false;

                string fileName = fullPack ? "DGE-FullPack" : "DGE-Latest";

                if (a.Length > 0)
                {
                    if (!fullPack) fileName = string.Join('-', a);
                    else if (a.Length > 1 || !fullPack) // If the user wants to define his own file name
                        fileName = string.Join('-', a, fullPack ? 1 : 0, a.Length - 1);
                }

                string filePath = Updater.ProjectPacker.Pack(fileName, fullPack);

                return $"Packed application Full={fullPack} (including settings and configs) in `{filePath}`";
            }, "Packs the application in a .zip (-f/-full argument has to be before filename)", 
                new string[] { 
                    "-f/-full (optional) > whether the pack should include configs (default false)", 
                    "FileName (optional) > The remaining characters (spaces included)" 
                }
            ));
        }
    }
}
