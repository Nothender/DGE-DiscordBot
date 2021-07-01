using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.ApplicationServers
{
    public static class GameServersManager
    {

        public static List<GameServerGroup> applicationServerGroups { get; private set; } = new List<GameServerGroup>();

        public static void StartGameServer(int groupIndex, int serverIndex, bool forceStart = false)
        {
            applicationServerGroups[groupIndex].StartServer(serverIndex, forceStart);
        }

        public static void StopGameServer(int groupIndex)
        {
            applicationServerGroups[groupIndex].StopCurrentServer();
        }

        public static int AddGameServerGroup(GameServerGroup serverGroup)
        {
            if (applicationServerGroups.Contains(serverGroup))
                throw new Exception("The server group was already added");
            applicationServerGroups.Add(serverGroup);
            return applicationServerGroups.Count - 1;
        }

    }
}
