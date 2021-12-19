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

                return null;
            });
            Commands.CreateCommand("wau", (a) => //Write to auto updater
            {
                if (a.Length < 1) throw new InvalidArgumentCountException("wau", 1, a.Length, true);

                return Updater.UpdateManager.WriteToUpdater(string.Join(' ', a));
            });
        }
    }
}
