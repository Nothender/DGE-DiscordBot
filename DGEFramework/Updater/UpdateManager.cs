using DGE.Core;
using DGE.Core.OperatingSystem;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;

namespace DGE.Updater
{
    public static class UpdateManager
    {
        private static Logger logger = new Logger("DGE-Updater");

        private static bool isUpdateAndRestartRequired = false;

        private static Action<string, Logger.LogLevel> LogCallback;

        public static StreamWriter updaterInput { get; private set; }

        /// <summary>
        /// Starts the updater process - Restarts it if it already exist
        /// </summary>
        /// <param name="LogCallback"> Action triggered when recieving Logs from the application, the input string is the message </param>
        public static void StartUpdater(Action<string, Logger.LogLevel> logCallback = null)
        {

            if (!(updaterInput is null))
            {
                updaterInput.WriteLine("exit");
                Thread.Sleep(1000); // Sleeping arbitrary value to 'ensure' the process is shutdown
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "DGEUpdater.exe",
                Arguments = Process.GetCurrentProcess().Id.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            };

            Process updaterProcess = new Process
            {
                StartInfo = startInfo
            };
            updaterProcess.OutputDataReceived += OutputRecievedHandler;
            updaterProcess.ErrorDataReceived += OutputRecievedHandler;

            Main.OnShutdown += (s, e) =>
            {
                if (updaterInput is null) return;
                updaterInput.WriteLine("exit");
            };

            LogCallback = logCallback;

            updaterProcess.Start();
            updaterProcess.BeginOutputReadLine();
            updaterProcess.BeginErrorReadLine();
            updaterInput = updaterProcess.StandardInput;
        }

        private static void OutputRecievedHandler(object sender, DataReceivedEventArgs line)
        {
            if (line is null || line.Data is null) return;
            //System.Console.WriteLine(line.Data);//TODO: To remove, proper logging, remove logging from DGEUpdater program
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
            if (res.Contains(UpdaterTags.UpdateDownloadedTag)) isUpdateAndRestartRequired = true;
            if (res.Contains(UpdaterTags.Stopped)) UpdaterProgramStopped();
        }

        private static void UpdaterProgramStopped()
        {
            if (isUpdateAndRestartRequired) StartUpdateScript();
            //Resetting input media
            updaterInput?.Close();
            updaterInput?.Dispose();
            updaterInput = null;
        }

        private static void StartUpdateScript()
        {
            Scripts.UpdateRestartApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);
            Main.OnStopped += (s, e) => Scripts.UpdateRestartApp.Run();
            Main.Stop(); //TODO: PDB Files from DGEExampleProgram and deps are not moved (access denied) maybe ensure the app is running by detaching the process ?
            //Seems to be a UB, as PDB files are sometimes moved
        }

    }
}
