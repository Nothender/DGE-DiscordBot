using DGE.Core;
using DGE.Core.OperatingSystem;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DGE.Updater
{
    public static class UpdateManager
    {
        private static Logger logger = new Logger("DGE-Updater");

        private static bool isUpdateAndRestartRequired = false;

        public static void StartUpdater()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "DGEUpdater.exe",
                Arguments = Process.GetCurrentProcess().Id.ToString(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process updaterProcess = new Process
            {
                StartInfo = startInfo
            };
            updaterProcess.OutputDataReceived += OutputRecievedHandler;
            updaterProcess.ErrorDataReceived += OutputRecievedHandler;
            updaterProcess.Start();
            updaterProcess.BeginOutputReadLine();
            updaterProcess.BeginErrorReadLine();
        }

        private static void OutputRecievedHandler(object sender, DataReceivedEventArgs line)
        {
            if (line is null || line.Data is null) return;
            System.Console.WriteLine(line.Data);//TODO: To remove, proper logging, remove logging from DGEUpdater program
            if (line.Data.Contains(UpdaterTags.log)) System.Console.WriteLine("Imagine Logging : ");
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
