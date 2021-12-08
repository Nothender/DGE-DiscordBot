using DGE.Console;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE
{
    public static class UpdaterProgram
    {

        public static void FetchVersions(int index)
        {
            if (index < 0) // If there are no specific indexes, we fetch all versions of all projects
            {
                foreach (ProjectInfo info in Program.ProjectInfos.projectInfos)
                    FetchVersion(info);
            }
            else if (index >= Program.ProjectInfos.projectInfos.Length)
                UpdaterLogging.WriteToMain("No project of that index/name exists or was loaded. Couldn't fetch version", Logger.LogLevel.ERROR);
            else
            {
                FetchVersion(Program.ProjectInfos.projectInfos[index]);
            }
        }

        private static void FetchVersion(ProjectInfo info)
        {
            FetcherCollection.InitFetcher(info.FetcherOptions);
            DGEVersion version = DGEVersion.FromString(FetcherCollection.FetchLatestVersion(info.VersionLatestGet));
            if (version.IsNewer(info.Version))
                UpdaterLogging.WriteToMain($"A new version ({version.version}) is available for the project {info}", Logger.LogLevel.INFO);
            else
                UpdaterLogging.WriteToMain($"No new version exist for the project {info}", Logger.LogLevel.INFO);

        }

    }
}
