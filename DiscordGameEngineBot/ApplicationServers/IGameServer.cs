using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public interface IGameServer : IApplicationServer
    {
        int usersConnected { get; }
        string[] usersConnectedUsernames { get; }
    }
}
