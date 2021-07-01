using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscordGameEngine.ApplicationServers
{
    public class MinecraftServer : CmdGameServer
    {


        public MinecraftServer(string startFilePath, string IPAdress, string displayName = null) : base(startFilePath, IPAdress, displayName)
        {
            typeName = "Minecraft";
        }

        protected override void StopServer()
        {
            process.StandardInput.WriteLine("/stop");
        }

        protected override void ReadLogMessage(string logMessage)
        {
            if (logMessage.Contains("UUID of player") && logMessage.Contains("User Authenticator")) //ex of a minecraft log (user logging in) : [16:48:12] [User Authenticator #3/INFO]: UUID of player Tristonks is 42
            {
                AddUser(logMessage.Split(' ')[7]); //The 8th word is the username
            }
            else if (logMessage.Contains("lost connection: Disconnected") && usersConnectedUsernames.Contains(logMessage.Split(' ')[3]))
            {
                RemoveUser(logMessage.Split(' ')[3]);
            }
            else if (logMessage.Contains("Done (") && logMessage.Contains(")! For help, type \"help\""))
            {
                Started();
            }
        }

        protected override void ReadErrorMessage(string logMessage)
        {
        }


    }
}
