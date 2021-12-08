using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using DGE.Core;
using EnderEngine;
using System.Collections;
using System.Collections.Generic;
using DGE.Console;
using DGE.Updater;

namespace DGE
{
    public static class Program
    {

        static void Main(string[] args)
        {
            DGE.Main.Init(false); //We dont want to create commands for the updater, if this is the updater
            DGE.Main.OnStopped += (s, e) =>
            {
                System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.Stopped);
                Console.UpdaterLogging.WriteToMain("Stopped Updater", Logger.LogLevel.INFO);
            };
            DGE.Main.OnStarted += (s, e) =>
            {
                AUpdaterCommands.CreateCommands();
                StartUpdater();
            };
            DGEModules.RegisterModule(AssemblyUpdater.module);

            //TODO: Cannot get command feedback because ender engine is not well made enough, need interface so we can create our own logger to replace the command one (so it can send back the command execution result to DGE)

            DGE.Main.Run(false).GetAwaiter().GetResult();

        }

        public static ProjectInfosManager ProjectInfos;

        /// <summary>
        /// Starts the updater - loads projects into memory
        /// </summary>
        public static void StartUpdater()
        {

            try
            {
                ProjectInfos = new ProjectInfosManager(Paths.Get("Application") + "ProjectUpdateInfo.xml", Paths.Get("Application") + "ProjectInfoConfig.xml"); //Version info from each repository used in the project (to know if an update is needed)

                UpdaterLogging.WriteToMain($"Discovered {ProjectInfos.projectInfos.Length} project(s) :\n - {string.Join<ProjectInfo>("\n - ", ProjectInfos.projectInfos)}", Logger.LogLevel.INFO);

                return;
                foreach (ProjectInfo info in ProjectInfos.projectInfos) //TODO: way to enumerate over ProjectInfos (IProjectInfo to create) for cleaner code instead of keeping track of the index for the version and url
                {
                    try
                    {
                        char sep = '>';

                        FetcherCollection.InitFetcher(info.FetcherOptions);

                        IVersion latestVersion = DGEVersion.FromString(FetcherCollection.FetchLatestVersion(info.VersionLatestGet)); //Latest version from website
                        IVersion localVersion = info.Version; //Local version calculated
                        UpdaterLogging.WriteToMain($"\nProject {info.Version.name} :\n -> Latest version from website : {latestVersion}\n -> Local version calculated : {localVersion}", Logger.LogLevel.INFO);

                        if (true || latestVersion.IsNewer(localVersion)) //If the version on the internet is newer than the local version
                        //We try to download it or ask for download
                        {

                            try
                            {
                                UpdaterLogging.WriteToMain("Update available, Attempting download", Logger.LogLevel.INFO);
                                System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.UpdateAvailableTag);

                                //FetcherCollection.DownloadLatestVersion(info.ProjectDlLatest[i].Split(sep));

                                //TODO: Move down, so it extracts the files, once every project is done downloading | Extract with priorities : the first project downloaded should overwrite others if they share files

                                //Download and app shutdown was successful : Extracting zip

                                //System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.UpdateDownloadedTag);
                                UpdaterLogging.WriteToMain("Update downloaded succesfully", Logger.LogLevel.INFO);

                                //TODO: Fetch latest release, and put it in a seperate directory
                                //TODO: If download successful, shutdown launching process, move files to process folder, rerun launching process


                                string file = Directory.GetFiles(Paths.Get("Downloads"))[0];
                                if (file.EndsWith(".zip"))
                                    ZipFile.ExtractToDirectory(file, Paths.Get("Contents"), true);

                            }
                            catch (Exception e)
                            {
                                UpdaterLogging.WriteToMain($"Couldn't download or install latest version :\n{e.Message}", Logger.LogLevel.ERROR);
                            }
                            //TODO: Try to download update/Ask if want to download
                            //If downloading new update -> shutdown application that ran the updater.
                        }
                        else UpdaterLogging.WriteToMain("No update required", Logger.LogLevel.INFO);
                    }
                    catch (Exception e)
                    {
                        UpdaterLogging.WriteToMain($"Couldn't fetch the latest version of your project, maybe check if your repository is public (autoupdater doens't support logins yet) :\n{e.Message}", Logger.LogLevel.ERROR);
                    }
                }


            }
            catch (Exception e)
            {
                UpdaterLogging.WriteToMain($"Error loading ProjectUpdate info and config file (you should try running the application first) :\n{e.Message}", Logger.LogLevel.FATAL);
                DGE.Main.Stop();
            }
            //If there are no updates application shutdown

            //TODO: Split main in different functions
            //TODO: Add a simple way to load DLL and check if a new release is available, without having to reference this project
            //TODO: maybe use EnderEngine for Paths?, logging
        }
    }
}
