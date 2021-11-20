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
            Commands.CreateCommand("sau", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("sau", 0, a.Length);

                Updater.UpdateManager.StartUpdater();

                return "Started auto-updater";
            });
            Commands.CreateCommand("wau", (a) => //Write to auto updater
            {
                if (a.Length < 1) throw new InvalidArgumentCountException("wau", 1, a.Length, true);

                if (Updater.UpdateManager.updaterInput is null)
                    return "Cannot write to the AutoUpdater as it is not running";

                string command = string.Join(' ', a);

                Updater.UpdateManager.updaterInput.WriteLine(command);

                return $"Wrote command `{string.Join(' ', a)}` to the AutoUpdater process";
            });
        }
    }
}
