﻿using DGE.Core;
using DGE.Core.OperatingSystem;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using DGE.Processes;
using DGE.Application;

namespace DGE.Updater
{
    public static class UpdateManager
    {
        private static Logger logger = new Logger("DGE-Updater");

        public static bool isUpdateDownloaded = false;
        public static bool isUpdateAvailable = false;
        public static bool fetched = false;
        public static bool downloaded = false;
        public static bool loaded = false;

        public static int RequestTimeoutMilliseconds = 2000;
        private static int RequestPollrateMilliseconds = 10;

        private static Action<string, Logger.LogLevel> LogCallback;

        private static ApplicationProcess process;

        private static void ResetBools()
        {
            isUpdateDownloaded = false;
            isUpdateAvailable = false;
            fetched = false;
            downloaded = false;
            loaded = false;
        }

        static UpdateManager()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = $"DGEUpdater{OS.GetDotnetExtension()}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            process = new ApplicationProcess("UpdateProcess", startInfo, SafeProcessStop);

            process.OnStarting += (s, e) =>
            {
                process.process.OutputDataReceived += OutputRecievedHandler;
                process.process.ErrorDataReceived += OutputRecievedHandler;
            };
            process.OnStarted += (s, e) =>
            {
                try
                {
                    process.process.BeginOutputReadLine();
                    process.process.BeginErrorReadLine();
                }
                catch(Exception ex)
                {
                    logger.Log("Error trying to start reading process Output" + ex.Message, Logger.LogLevel.WARN);
                }
            };
        }

        private static void SafeProcessStop(ApplicationProcess ap)
        {
            if (ap.process is null)
                return;
            if (ap.process.StandardInput is null)
                ap.process.Kill();
            else
            {
                ap.process.StandardInput.WriteLine("exit");
                ap.process.WaitForExit(100); // Arbitrary timeout for application process clean up
            }
            ResetBools();
        }

        /// <summary>
        /// Starts the updater process
        /// </summary>
        /// <param name="LogCallback"> Action triggered when recieving Logs from the application, the input string is the message </param>
        public static void StartUpdater(Action<string, Logger.LogLevel> logCallback = null)
        {

            LogCallback = logCallback;

            if (process.status >= ApplicationStatus.ON) // If the application is ON or STARTING
            {
                logger.Log($"An ApplicationProcess (id: {process.Id}) is already running", Logger.LogLevel.INFO);
                return;
            }

            ResetBools();

            process.Start();

            logger.Log($"Starting updater as ApplicationProcess (id: {process.Id})", Logger.LogLevel.INFO);

            WaitForRequest(ref loaded, "starting updater", 4242);
        }

        public static void Download(string args)
        {
            if (process.status <= ApplicationStatus.STOPPING)
                throw new Exception("Updater is not started");
            downloaded = false;
            isUpdateDownloaded = false;
            WriteToUpdater($"download {args}");
            WaitForRequest(ref downloaded, "downloading");
        }

        /// <summary>
        /// Fetches the latest version
        /// </summary>
        public static void Fetch(string args)
        {
            if (process.status <= ApplicationStatus.STOPPING)
                throw new Exception("Updater is not started");
            fetched = false;
            isUpdateAvailable = false;
            WriteToUpdater($"fetch {args}");
            WaitForRequest(ref fetched, "fetching");
        }

        /// <summary>
        /// Blocks till the request was answered. If the timeout is exceeded it throws a TimeoutException
        /// </summary>
        /// <param name="boolean">The boolean we are waiting for (from false to true)</param>
        /// <param name="action">Action name for error reporting</param>
        private static void WaitForRequest(ref bool boolean, string action = "unknown", int customMillisecondTimeout = -1)
        {
            int msTotal = 0;
            int msTimeout = customMillisecondTimeout > 0 ? customMillisecondTimeout : RequestTimeoutMilliseconds;
            while (!boolean)
            {
                Thread.Sleep(RequestPollrateMilliseconds);
                msTotal += RequestPollrateMilliseconds;
                if (msTotal > msTimeout)
                    throw new TimeoutException($"AutoUpdater ({action}) - Request timeout of {customMillisecondTimeout}ms exceeded");
            }
        }

        /// <summary>
        /// Stops the updater process
        /// </summary>
        public static void StopUpdater()
        {
            process.Stop();
        }

        private static void OutputRecievedHandler(object sender, DataReceivedEventArgs line)
        {
            if (line is null || line.Data is null) return;
            if (line.Data.Contains(UpdaterTags.log))
            {
                int st = UpdaterTags.log.Length + 1;
                string message = line.Data.Substring(st).Replace("\\n", "\n");
                Logger.LogLevel level = UpdaterTags.GetLogLevel(line.Data.Substring(0, st));
                logger.Log(message, level);
                try { LogCallback?.Invoke(message, level); } 
                catch (Exception e) { logger.Log("Error at log callback : " + e.Message, Logger.LogLevel.WARN); }
            }
            if (line.Data.Contains(UpdaterTags.PassthroughInfo))
                ProcessData(line.Data);
        }

        private static void ProcessData(string res)
        {
            if (res.Contains(UpdaterTags.UpdateDownloadedTag)) isUpdateDownloaded = true;
            if (res.Contains(UpdaterTags.UpdateAvailableTag)) isUpdateAvailable = true;
            if (res.Contains(UpdaterTags.Stopped)) UpdaterProgramStopped();
            if (res.Contains(UpdaterTags.FetchedTag)) fetched = true;
            if (res.Contains(UpdaterTags.AttemptedDownloadTag)) downloaded = true;
            if (res.Contains(UpdaterTags.LoadedTag)) loaded = true;
        }

        private static void UpdaterProgramStopped()
        {
        }

        /// <summary>
        /// Installs newly downloaded content then restarts the application
        /// </summary>
        public static void StartUpdateScript()
        {
            Scripts.UpdateRestartApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);

            Main.OnStopped += (s, e) => Scripts.UpdateRestartApp.Run();
            Main.Stop(); //TODO: PDB Files from DGEExampleProgram and deps are not moved (access denied) maybe ensure the app is running by detaching the process ?
            //Seems to be a UB, as PDB files are sometimes moved
        }

        public static string WriteToUpdater(string command)
        {
            if (process.status <= ApplicationStatus.STOPPING) // Application process is OFF or STOPPING
                return "Cannot write to the AutoUpdater as it is not running";

            process.process.StandardInput.WriteLine(command);

            return $"Wrote command `{command}` to the AutoUpdater process";
        }

    }
}
