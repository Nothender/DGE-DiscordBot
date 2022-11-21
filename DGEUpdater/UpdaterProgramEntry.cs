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
using System.Threading.Tasks;

namespace DGE
{
    public static class UpdaterProgramEntry
    {

        internal static readonly int msStandbyMaxTime = 15000; // Automatically stops if more than 15s standby (doing this to prevent the application for just staying open in the system)
        internal static int msStandbyTimer;
        private static int currentlyRunningIds;   // If something is running/waiting, this integer will be > 0, if not that means we can resume the timer

        internal static void PauseStandbyTimer(int id)
        {
            currentlyRunningIds += 1 << id;
            msStandbyTimer = msStandbyMaxTime;
        }

        internal static void UnPauseStandbyTimer(int id)
        {
            currentlyRunningIds -= 1 << id;
        }

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

            _ = Timer(); // shutting down after 15 seconds of standby

            DGE.Main.Run(false).GetAwaiter().GetResult();

        }

        public static async Task Timer()
        {
            msStandbyTimer = msStandbyMaxTime;
            int msRefresh = 100;
            while (msStandbyTimer > 0)
            {
                await Task.Delay(msRefresh);
                if (currentlyRunningIds == 0)
                    msStandbyTimer -= msRefresh;
            }
            UpdaterLogging.WriteToMain($"Updater was on standby for longer than {msStandbyMaxTime / 1000}s automatic shutdown", Logger.LogLevel.WARN);
            DGE.Main.Stop();
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
