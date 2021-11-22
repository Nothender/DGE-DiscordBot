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
        public static void Create()
        {
            //TODO: Commands may need to be improved (with structs or classes or attributes, with automatic TypeCasting and arguments passing)
            Commands.CreateCommand("help", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("help", 0, a.Length);

                string helpMessage = "Commands that exist :";
                helpMessage += "\n- " + string.Join("\n- ", Commands.commands.Keys.ToArray());
                return helpMessage;
            });
            Commands.CreateCommand("startapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("startapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(id).Start();
                return $"Application of id {a[0]} was started";
            });
            Commands.CreateCommand("stopapp", (a) =>
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("stopapp", 1, a.Length);
                if (!int.TryParse(a[0], out int id)) throw new InvalidArgumentTypeException(0, typeof(int));

                ApplicationManager.Get(id).Stop();
                return $"Application of id {a[0]} was stopped";
            });
            Commands.CreateCommand("showapps", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("showapps", 0, a.Length);
                string res = "Instanced applications :";
                foreach (IApplication app in ApplicationManager.GetAll()) res += $"\n - {app.GetType().Name} application of id {app.Id}, currently {app.status}";
                return res;
            });
            Commands.CreateCommand("fgc", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("FGC", 0, a.Length);

                GC.Collect();
                return "Forced a garbage collection";
            });
            Commands.CreateCommand("showmodules", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("showmodules", 0, a.Length);

                return $"Loaded assembly DGE Modules :\n - {string.Join("\n - ", DGEModules.modules)}";
            });
            Commands.CreateCommand("restart", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("restart", 0, a.Length);

                Scripts.RunApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);
                Main.OnStopped += (s, e) => Scripts.RunApp.Run();
                Main.Stop();

                return "Restarting";
            });
        }
    }
}
