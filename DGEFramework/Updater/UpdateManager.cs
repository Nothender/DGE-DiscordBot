using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DGE.Updater
{
    public static class UpdateManager
    {

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
            //Main.Stop();
        }

        private static void OutputRecievedHandler(object sender, DataReceivedEventArgs line)
        {
            //TODO: If logs ignore, otherwise send to process results
        }

        private static void ProcessResults(string res)
        {
            //TODO: If update required, shutdown process and run UScript (.bat or .sh depending on OS)
        }

    }
}
