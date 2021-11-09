using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DGE
{
    public static class Program
    {

        static void Main(string[] args)
        {

            SimpleLogger.Init(Paths.AUPath + "Logs");

            try
            {
                ProjectUpdateInfo info = new ProjectUpdateInfo(Paths.ProgramPath + "ProjectUpdateInfo.xml", Paths.ProgramPath + "ProjectInfoConfig.xml"); //Version info from each repository used in the project (to know if an update is needed)

                string launchPId = args.Length > 0 ? args[0] : null;

                for (int i = 0; i < info.ProjectVersions.Length; i++) //TODO: way to enumerate over ProjectInfos (IProjectInfo to create) for cleaner code instead of keeping track of the index for the version and url
                {
                    try
                    {
                        char sep = '>';

                        FetcherCollection.InitFetcher(info.FetcherOptions[i].Split(sep));

                        IVersion latestVersion = DGEVersion.FromString(FetcherCollection.FetchLatestVersion(info.ProjectVLatest[i].Split(sep))); //Latest version from website
                        IVersion localVersion = info.ProjectVersions[i]; //Local version calculated
                        SimpleLogger.Log($"\n - Latest version from website : {latestVersion}\n - Local version calculated : {localVersion}");

                        FetcherCollection.DownloadLatestVersion(info.ProjectDlLatest[i].Split(sep));

                        if (latestVersion.IsNewer(localVersion)) //If the version on the internet is newer than the local version
                        //We try to download it or ask for download
                        {
                            try
                            {
                                SimpleLogger.Log($"Update available, Attempting download");
                                //TODO: Fetch latest release, and put it in a seperate directory
                                //TODO: If download successful, shutdown launching process, move files to process folder, rerun launching process
                            }
                            catch (Exception e)
                            {
                                SimpleLogger.Log("Couldn't download or install latest version :\n" + e.Message);
                            }
                            //TODO: Try to download update/Ask if want to download
                            //If downloading new update -> shutdown application that ran the updater.
                        }
                        SimpleLogger.Log("No update required");
                    }
                    catch (Exception e)
                    {
                        SimpleLogger.Log("Couldn't fetch the latest version of your project, maybe check if your repository is public (autoupdater doens't support logins yet) :\n" + e.Message, 2);
                    }
                }


            }
            catch(Exception e)
            {
                SimpleLogger.Log("Error loading ProjectUpdate info and config file : " + e.Message, 2);
                SimpleLogger.Log("Maybe try to run the application first (before the updater)", 0);
            }
            
            //If there are no updates application shutdown
            
            //TODO: Split main in different functions
            //TODO: Add a simple way to load DLL and check if a new release is available, without having to reference this project
            //TODO: maybe use EnderEngine for Paths?, logging

        }

    }
}
