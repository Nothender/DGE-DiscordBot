using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using DGE.Core;
using DGE.Updater;
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
                        System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.INFO)}\n - Latest version from website : {latestVersion}\n - Local version calculated : {localVersion}");

                        if (true || latestVersion.IsNewer(localVersion)) //If the version on the internet is newer than the local version
                        //We try to download it or ask for download
                        {
                            try
                            {
                                System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.INFO)}Update available, Attempting download");
                                System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.UpdateAvailableTag);

                                //FetcherCollection.DownloadLatestVersion(info.ProjectDlLatest[i].Split(sep));
                                
                                //Download and app shutdown was successful : Extracting zip
                                foreach(string file in Directory.GetFiles(Paths.Get("Downloads")))
                                {
                                    if (file.EndsWith(".zip"))
                                        ZipFile.ExtractToDirectory(file, Paths.Get("Contents"), true);
                                }

                                System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.UpdateDownloadedTag);

                                //ScriptsManager.RunScript(launchPId);

                                //TODO: Fetch latest release, and put it in a seperate directory
                                //TODO: If download successful, shutdown launching process, move files to process folder, rerun launching process
                            }
                            catch (Exception e)
                            {
                                System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.ERROR)}Couldn't download or install latest version :\n{e.Message}");
                            }
                            //TODO: Try to download update/Ask if want to download
                            //If downloading new update -> shutdown application that ran the updater.
                        }
                        else System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.INFO)}No update required");
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.ERROR)}Couldn't fetch the latest version of your project, maybe check if your repository is public (autoupdater doens't support logins yet) :\n{e.Message}");
                    }
                }

            }
            catch(Exception e)
            {
                System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.FATAL)}Error loading ProjectUpdate info and config file (you should try running the application first) :\n{e.Message}");
            }

            System.Console.WriteLine(UpdaterTags.PassthroughInfo + UpdaterTags.Stopped);

            System.Console.WriteLine($"{UpdaterTags.GetLogTag(Logger.LogLevel.INFO)}Stopped Updater");

            //If there are no updates application shutdown

            //TODO: Split main in different functions
            //TODO: Add a simple way to load DLL and check if a new release is available, without having to reference this project
            //TODO: maybe use EnderEngine for Paths?, logging
        }

    }
}
