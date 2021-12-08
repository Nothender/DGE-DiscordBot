using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;
using DGE;
using DGE.Updater;
using DGE.Exceptions;

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
                {
                    foreach(ProjectInfo info in Program.ProjectInfos.projectInfos)
                    {

                        FetcherCollection.InitFetcher(info.FetcherOption.Split());
                    }
                }
                return null;
            });
        }

    }
}
