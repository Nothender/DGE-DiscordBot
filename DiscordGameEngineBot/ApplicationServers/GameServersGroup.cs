using DiscordGameEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public class GameServerGroup
    {
        public List<IGameServer> servers { get; private set; } = new List<IGameServer>();
        private int currentOnlineServerIndex = -1;

        public string address;
        public string displayName;
        
        public GameServerGroup(string address, string displayName)
        {
            this.address = address;
            if (displayName is null)
                displayName = address;
            this.displayName = displayName;
        }

        public IGameServer GetCurrentRunningServer()
        {
            return servers[currentOnlineServerIndex];
        }

        public int AddApplicationServer(IGameServer server)
        {
            if (servers.Contains(server))
                throw new ApplicationServerException("Cannot add the server, the server is already in the group");

            int index = servers.Count;
            servers.Add(server);
            if (server.status == ApplicationServerState.ONLINE && currentOnlineServerIndex >= 0)
                server.Stop();
            else if (server.status == ApplicationServerState.ONLINE)
                currentOnlineServerIndex = index;
            return index;
        }

        public void StartServer(int index, bool forceStart = false)
        {
            if (index == currentOnlineServerIndex)
                throw new ServerAlreadyStartedException("The server in this group is already started");
            if (currentOnlineServerIndex >= 0 && (forceStart || servers[currentOnlineServerIndex].usersConnected < 1))
            {
                servers[currentOnlineServerIndex].Stop();
                currentOnlineServerIndex = index;
                servers[index].Start();
            }
            else if (currentOnlineServerIndex < 0)
            {
                currentOnlineServerIndex = index;
                servers[index].Start();
            }
            else
                throw new ServerAlreadyStartedException("Couldn't start the server, because another server in the group is online and cannot be shutdown");
        }

        public void StopCurrentServer()
        {
            if (currentOnlineServerIndex >= 0)
                servers[currentOnlineServerIndex].Stop();
            currentOnlineServerIndex = -1;
        }

    }
}