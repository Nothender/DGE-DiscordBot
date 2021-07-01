using DiscordGameEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DiscordGameEngine.ApplicationServers
{
    public abstract class CmdGameServer : GameServer
    {
        protected Process process;

        private string startFilePath;
        private string workingDirectoryPath;

        public CmdGameServer(string startFilePath, string IPAdress, string displayName = null) : base(IPAdress, displayName)
        {
            this.startFilePath = startFilePath;
            string temp = startFilePath.Replace('\\', '/');
            string[] folders = temp.Split('/');
            workingDirectoryPath = string.Join('/', folders.Take(folders.Length - 1));
        }

        public override void Start()
        {
            if (status == ApplicationServerState.ONLINE || status == ApplicationServerState.STARTING)
                throw new ServerAlreadyStartedException();

            status = ApplicationServerState.STARTING;

            ProcessStartInfo processInfo = new ProcessStartInfo(startFilePath);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.WorkingDirectory = workingDirectoryPath;

            process = Process.Start(processInfo);

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
            });

        }

        public override void Stop()
        {
            if (!isOnline)
                throw new ServerAlreadyOfflineException();
            StopServer();
        }
        
        public void WriteToConsole(string message, bool asFullLine = true)
        {
            if (asFullLine)
                process.StandardInput.WriteLine(message);
            else
                process.StandardInput.Write(message);
        }

        protected abstract void StopServer(); //ways to stop a command line application are various, so it has to be different for every app

        protected void AddUser(string name = null)
        {
            usersConnected++;
            usersConnectedUsernames = usersConnectedUsernames.Append(name).ToArray();
        }

        protected void RemoveUser(string name = null)
        {
            usersConnected--;
            if (name is null)
            {
                string[] tempArray = new string[usersConnectedUsernames.Length - 1];
                bool nullFound = false;
                int tempArrayPtr = 0;
                for (int i = 0; i < usersConnectedUsernames.Length; i++)
                {
                    if ((!nullFound) && (usersConnectedUsernames[i] is null))
                        nullFound = true;
                    else
                    {
                        tempArray[tempArrayPtr] = usersConnectedUsernames[i];
                        tempArrayPtr++;
                    }
                }
            }
            else
                usersConnectedUsernames = usersConnectedUsernames.Where(s => s != name).ToArray();
        }

        private void Log(string logMessage, bool error)
        {

            //TODO: Write log in a file //Custom EE logger ?

            if (logMessage is null)
                return;

            DiscordGameEngineBot.DGELogger.Log(logMessage, EnderEngine.Logger.LogLevel.DEBUG);

            if (error)
                ReadErrorMessage(logMessage);
            else
                ReadLogMessage(logMessage);
        }

        protected abstract void ReadLogMessage(string logMessage);

        protected abstract void ReadErrorMessage(string logMessage);

    }
}
