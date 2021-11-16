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

        public static string PassthroughInfoTag = "DGEUT";

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
            if (line.Data.Contains(PassthroughInfoTag))
                ProcessData(line.Data);
        }

        private static void ProcessData(string res)
        {
            //TODO: If update required, shutdown process and run UScript (.bat or .sh depending on OS)
            if (res.Contains("URR")) //TODO: Handle tags correctly
                StartUpdateScript();
        }

        private static void StartUpdateScript()
        {
            Scripts.UpdateRestartApp.CreateProcess(Process.GetCurrentProcess().MainModule.FileName);
            Main.OnStopped += (s, e) => Scripts.UpdateRestartApp.Run();
            Main.Stop(); //TODO: PDB Files from DGEExampleProgram and deps are not moved (access denied) maybe ensure the app is running by detaching the process ?
        }

    }
}
