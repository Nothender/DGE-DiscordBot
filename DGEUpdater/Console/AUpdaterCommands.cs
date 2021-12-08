using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;
using DGE;
using DGE.Exceptions;
using System.Linq;
using EnderEngine;

namespace DGE.Console
{
    public static class AUpdaterCommands
    {

        public static void CreateCommands()
        {
            Commands.CreateCommand("help", (a) =>
            {
                if (a.Length != 0) throw new InvalidArgumentCountException("help", 0, a.Length);

                string helpMessage = "Commands that exist :";
                helpMessage += "\n- " + string.Join("\n- ", Commands.GetCommands());
                return helpMessage; // Shows every existing commands, doesn't detail anything
            });
            Commands.CreateCommand("fetch", (a) => // Fetches the latest releases for the corresponding projects/modules
            {
                if (a.Length < 1 || a[0] == "all") // Fetch all the versions by default
                    UpdaterProgram.FetchVersions(-1);
                else if (int.TryParse(a[0], out int i))
                    UpdaterProgram.FetchVersions(i);
                else if (Program.ProjectInfos.projectInfos.Any(p => p.Version.name.ToLower() == a[0]))
                    UpdaterProgram.FetchVersions(Array.FindIndex(Program.ProjectInfos.projectInfos, p => p.Version.name == a[0]));
                else
                    throw new ArgumentException("The argument does not fit any use");

                return "Fetched versions";
            });
        }

    }
}
