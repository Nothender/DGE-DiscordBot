using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using DGE.Core;
using EnderEngine;

namespace DGE
{
    public static class Program
    {

        static void Main(string[] args)
        {

            DGE.Main.Init();
            DGEModules.RegisterModule(AssemblyUpdater.module);

            try
            {
                ProjectUpdateInfo info = new ProjectUpdateInfo(Paths.Get("Application") + "ProjectUpdateInfo.xml", Paths.Get("Application") + "ProjectInfoConfig.xml"); //Version info from each repository used in the project (to know if an update is needed)

                string launchPId = args.Length > 0 ? args[0] : null;

                for (int i = 0; i < info.ProjectVersions.Length; i++) //TODO: way to enumerate over ProjectInfos (IProjectInfo to create) for cleaner code instead of keeping track of the index for the version and url
                {
                    try
                    {
                        char sep = '>';

                        FetcherCollection.InitFetcher(info.FetcherOptions[i].Split(sep));

                        IVersion latestVersion = DGEVersion.FromString(FetcherCollection.FetchLatestVersion(info.ProjectVLatest[i].Split(sep))); //Latest version from website
                        IVersion localVersion = info.ProjectVersions[i]; //Local version calculated
                        AssemblyUpdater.logger.Log($"\n - Latest version from website : {latestVersion}\n - Local version calculated : {localVersion}", Logger.LogLevel.INFO);

                        if (true || latestVersion.IsNewer(localVersion)) //If the version on the internet is newer than the local version
                        //We try to download it or ask for download
                        {
                            try
                            {
                                AssemblyUpdater.logger.Log($"Update available, Attempting download", Logger.LogLevel.INFO);

                                //FetcherCollection.DownloadLatestVersion(info.ProjectDlLatest[i].Split(sep));
                                
                                //Download and app shutdown was successful : Extracting zip
                                foreach(string file in Directory.GetFiles(Paths.Get("Downloads")))
                                {
                                    if (file.EndsWith(".zip"))
                                        ZipFile.ExtractToDirectory(file, Paths.Get("Contents"), true);
                                }

                                //ScriptsManager.RunScript(launchPId);

                                //TODO: Fetch latest release, and put it in a seperate directory
                                //TODO: If download successful, shutdown launching process, move files to process folder, rerun launching process
                            }
                            catch (Exception e)
                            {
                                AssemblyUpdater.logger.Log("Couldn't download or install latest version :\n" + e.Message, Logger.LogLevel.ERROR);
                            }
                            //TODO: Try to download update/Ask if want to download
                            //If downloading new update -> shutdown application that ran the updater.
                        }
                        else AssemblyUpdater.logger.Log("No update required", Logger.LogLevel.INFO);
                    }
                    catch (Exception e)
                    {
                        AssemblyUpdater.logger.Log("Couldn't fetch the latest version of your project, maybe check if your repository is public (autoupdater doens't support logins yet) :\n" + e.Message, Logger.LogLevel.ERROR);
                    }
                }

            }
            catch(Exception e)
            {
                AssemblyUpdater.logger.Log("Error loading ProjectUpdate info and config file : " + e.Message, Logger.LogLevel.FATAL);
                AssemblyUpdater.logger.Log("Maybe try to run the application first (before the updater)", Logger.LogLevel.INFO);
            }
            AssemblyUpdater.logger.Log("Stopped", Logger.LogLevel.INFO);

            //If there are no updates application shutdown

            //TODO: Split main in different functions
            //TODO: Add a simple way to load DLL and check if a new release is available, without having to reference this project
            //TODO: maybe use EnderEngine for Paths?, logging
        }

    }
}
