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

                System.Console.WriteLine($"{UpdaterTags.PassthroughInfo}{UpdaterTags.LoadedTag}");
                return;
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
