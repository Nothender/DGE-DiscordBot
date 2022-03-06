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

        private static int IndexFromArgument(string arg)
        {
            if (arg == "all") // Fetch all the versions by default
                return -1;
            else if (int.TryParse(arg, out int i))
                return i;
            else
            {
                int index = Array.FindIndex(UpdaterProgramEntry.ProjectInfos.projectInfos, p => p.Version.name.ToLower() == arg);
                if (index == -1)
                    throw new ArgumentException($"The argument `{arg}` does not fit any use");
                return index;
            }
        }

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
                if (a.Length != 1) throw new InvalidArgumentCountException("fetch", 1, a.Length);

                UpdaterProgram.FetchVersions(IndexFromArgument(a[0]));

                return "Successfully fetched versions";
            });
            Commands.CreateCommand("download", (a) => // Downloads the latest version of the project and extracts it
            {
                if (a.Length != 1) throw new InvalidArgumentCountException("fetch", 1, a.Length);

                UpdaterProgram.DownloadVersions(IndexFromArgument(a[0]));

                return "Succesfully (or at least attempted) downloaded project";
            });
        }

    }
}
