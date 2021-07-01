using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public abstract class GameServer : ApplicationServer, IGameServer
    {

        public int usersConnected { get; protected set; } = 0;
        public string[] usersConnectedUsernames { get; protected set; } = new string[0];

        public GameServer(string IPAdress, string displayName = null) : base(IPAdress, displayName)
        {
            ResetUsers();
        }

        protected void ResetUsers()
        {
            usersConnected = 0;
            usersConnectedUsernames = new string[0];
        }

    }
}
