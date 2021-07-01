using DiscordGameEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{

    //Not very clean implementation : use a better way to deal with processes (FW U V1)
    public class ValheimServer : GameServer
    {

        public string serverFilePath;
        private string workingDirectoryPath;

        public ValheimServer(string serverFilePath, string IPAdress, string displayName = null) : base(IPAdress, displayName)
        {
            typeName = "Valheim";
            this.serverFilePath = serverFilePath;
        }

        public override void Start()
        {

            //Not working

            if (status == ApplicationServerState.ONLINE || status == ApplicationServerState.STARTING)
                throw new ServerAlreadyStartedException();

            status = ApplicationServerState.STARTING;

            ProcessStartInfo processInfo = new ProcessStartInfo(serverFilePath);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.WorkingDirectory = serverFilePath;

            /*
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                Log(e.Data, false);

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                Log(e.Data, true);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Task.Run(() =>
            {
                process.WaitForExit();
                process.Close();
                Stopped();
            });*/
        }

        public override void Stop()
        {

        }



        /*
        protected override void StopServer()
        {
            process.Kill(true);
        }

        protected override void ReadLogMessage(string logMessage)
        {
            if (logMessage.Contains("Got connection SteamID")) //ex of a valheim log (user logging in) : 06/25/2021 13:05:47: Got connection SteamID 42
            {
                AddUser(logMessage.Split(' ')[5]); //The 6th word is the userid
            }
            else if (logMessage.Contains("Closing socket"))
            {
                RemoveUser(logMessage.Split(' ')[4]);
            }
            else if (logMessage.Contains("Game server connected"))
            {
                Started();
            }
        }

        protected override void ReadErrorMessage(string logMessage)
        {
        }
        */

    }
}
