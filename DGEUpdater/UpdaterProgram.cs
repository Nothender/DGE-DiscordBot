using DGE.Console;
using DGE.Core;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DGE
{
    public static class UpdaterProgram
    {

        private static ProjectInfo[] GetProjectsByIndex(int index)
        {
            if (index < 0) // If there are no specific indexes, we fetch all versions of all projects
                return Program.ProjectInfos.projectInfos;
            else if (index >= Program.ProjectInfos.projectInfos.Length)
            {
                UpdaterLogging.WriteToMain("No project of that index/name exists or was loaded. Couldn't fetch version", Logger.LogLevel.ERROR);
                return new ProjectInfo[0]; 
            }
            else
                return new ProjectInfo[1] { Program.ProjectInfos.projectInfos[index] };
        }

        public static void FetchVersions(int index)
        {
            try
            {
                foreach (ProjectInfo info in GetProjectsByIndex(index))
                {
                    FetcherCollection.InitFetcher(info.FetcherOptions);
                    DGEVersion version = DGEVersion.FromString(FetcherCollection.FetchLatestVersion(info.VersionLatestGet));
                    if (version.IsNewer(info.Version))
                    {
                        UpdaterLogging.WriteToMain($"A new version ({version.version}) is available for the project {info}", Logger.LogLevel.INFO);
                        System.Console.WriteLine($"{Updater.UpdaterTags.PassthroughInfo}{Updater.UpdaterTags.UpdateAvailableTag}");
                    }
                    else
                        UpdaterLogging.WriteToMain($"No new version exist for the project {info}", Logger.LogLevel.INFO);
                }
            }
            finally
            {
                System.Console.WriteLine($"{Updater.UpdaterTags.PassthroughInfo}{Updater.UpdaterTags.FetchedTag}");
            }
        }

        public static void DownloadVersions(int index)
        {
            //WIP
            //Paths.ClearPath("Downloads");
            try
            {
                foreach (ProjectInfo info in GetProjectsByIndex(index))
                {
                    FetcherCollection.InitFetcher(info.FetcherOptions);
                    //FetcherCollection.DownloadLatestVersion(info.FetcherOptions);
                    
                    DirectoryInfo di = new DirectoryInfo(Paths.Get("Downloads"));
                    FileInfo file = di.GetFiles().OrderBy(p => p.CreationTime).ToArray().Last();

                    if (file.Extension == ".zip")
                        ZipFile.ExtractToDirectory(file.FullName, Paths.Get("Contents"), true);
                    else
                        UpdaterLogging.WriteToMain($"Failed to download project {info}, file was not a compressed object", Logger.LogLevel.WARN);
                    
                    UpdaterLogging.WriteToMain($"Downloaded and extracted project {info}", Logger.LogLevel.INFO);
                    System.Console.WriteLine($"{Updater.UpdaterTags.PassthroughInfo}{Updater.UpdaterTags.UpdateDownloadedTag}");

                }
            }
            finally
            {
                //Paths.ClearPath("Downloads");
                System.Console.WriteLine($"{Updater.UpdaterTags.PassthroughInfo}{Updater.UpdaterTags.AttemptedDownloadTag}");
            }

        }

    }
}