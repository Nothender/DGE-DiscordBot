using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordGameEngine.ApplicationServers;
using DiscordGameEngine.Exceptions;
using System.Linq;

namespace DiscordGameEngine.UI.Commands
{
    [Summary("Commands to interact with the Application Servers (like minecraft or valheim game servers)")]
    public class ApplicationServerCommands : ModuleBase<SocketCommandContext>
    {

        static ApplicationServerCommands()
        {
            IApplicationServer.OnStarted += OnServerStarted;
            IApplicationServer.OnStopped += OnServerStopped;
        }

        private static string GetServerStatusEmote(ApplicationServerState status)
        {
            switch (status)
            {
                case ApplicationServerState.OFFLINE:
                    return "🔴";
                case ApplicationServerState.ONLINE:
                    return "🟢";
                case ApplicationServerState.STARTING:
                    return "🟡";
                default:
                    return "⚫";
            }
        }

        [Command("ShowServers")]
        [Summary("Shows a list of servers, online or not with their IP address and name")]
        public async Task CommandShowServers()
        {
            EmbedBuilder embed = new EmbedBuilder();
            int i = 1, j = 1;
            embed.WithDescription($"The groups and servers are shown this way :\n{{index}} {{GroupName}}\n{{{GetServerStatusEmote(ApplicationServerState.OFFLINE)} offline, {GetServerStatusEmote(ApplicationServerState.STARTING)} starting or {GetServerStatusEmote(ApplicationServerState.ONLINE)} online}} {{index}} {{Game}} - {{ServerName}} - {{IPAdress}}\n\nTo start a server enter the following command and arguments :\n `>StartServer `*`{{GroupIndex}} {{ServerIndex}}`*");
            GameServersManager.applicationServerGroups.ForEach(sg =>
            {
                string servers = "";
                j = 1;
                sg.servers.ForEach(s =>
                {
                    servers += $"{GetServerStatusEmote(s.status)} [{j}] {s.typeName} - {s.displayName} - {s.address}\n";
                    j++;
                });
                embed.AddField($"[{i}] {sg.displayName}", servers);
                i++;
            });
            embed.WithTitle("Servers :");
            embed.WithColor(Color.DarkGreen);
            await ReplyAsync(null, false, embed.Build());
        }

        private static List<(IMessageChannel, IGameServer)> channelServerStarted = new List<(IMessageChannel, IGameServer)>(4); //Keeps track of the channel which server was started

        [Command("StartServer")]
        [Summary("Tries to start a server using a ServerGroup Index and Server Index")]
        public async Task CommandStartServer(int groupIndex, int serverIndex)
        {

            groupIndex--;
            serverIndex--;

            if (groupIndex < 0 || groupIndex >= GameServersManager.applicationServerGroups.Count)
                throw new CommandExecutionException($"The group [{groupIndex + 1}] doesn't exist, group indexes are in the range of 1 to {GameServersManager.applicationServerGroups.Count}");
            if (serverIndex < 0 || serverIndex >= GameServersManager.applicationServerGroups[groupIndex].servers.Count)
                throw new CommandExecutionException($"The server [{groupIndex + 1}][{serverIndex + 1}] doesn't exist, server indexes for the group [{groupIndex + 1}] are in the range of 1 to {GameServersManager.applicationServerGroups[groupIndex].servers.Count}");

            try
            {
                GameServersManager.StartGameServer(groupIndex, serverIndex);
            }
            catch (Exception e)
            {
                if (!(e is ApplicationServerException))
                    throw e;
                throw new CommandExecutionException(e);
            }
            IGameServer server = GameServersManager.applicationServerGroups[groupIndex].GetCurrentRunningServer();
            channelServerStarted.Add((Context.Channel, server));
            await ReplyAsync($"{GetServerStatusEmote(ApplicationServerState.STARTING)} Starting the **{server.displayName}** {server.typeName} server");
        }
        
        private static List<(IMessageChannel, IGameServer)> channelServerStopped = new List<(IMessageChannel, IGameServer)>(4); //Keeps track of the channel which server was started

        [Command("StopServer")]
        [Summary("Stops the currently running server in the specified group using a ServerGroup Index")]
        [RequireOwner()]
        public async Task CommandStopServer(int groupIndex)
        {
            groupIndex--;

            if (groupIndex < 0 || groupIndex >= GameServersManager.applicationServerGroups.Count)
                throw new CommandExecutionException($"The group [{groupIndex + 1}] doesn't exist, group indexs are in the range of 1 to {GameServersManager.applicationServerGroups.Count}");

            IGameServer server = GameServersManager.applicationServerGroups[groupIndex].GetCurrentRunningServer();
            channelServerStopped.Add((Context.Channel, server));
            try
            {
                GameServersManager.StopGameServer(groupIndex);
            }
            catch (Exception e)
            {
                channelServerStopped.Remove((Context.Channel, server));
                if (!(e is ApplicationServerException))
                    throw e;
                throw new CommandExecutionException(e);
            }
            await ReplyAsync($"{GetServerStatusEmote(ApplicationServerState.OFFLINE)} Shutting the **{server.displayName}** {server.typeName} server down");
        }

        public static void OnServerStarted(object sender, EventArgs e)
        {
            int index = channelServerStarted.FindIndex(s => s.Item2 == sender);
            (IMessageChannel channel, IGameServer server) = channelServerStarted[index];
            if (index == -1)
                return;
            channel.SendMessageAsync($"{GetServerStatusEmote(ApplicationServerState.ONLINE)} The **{server.displayName}** {server.typeName} is now online");
            channelServerStarted.RemoveAt(index);
        }

        public static void OnServerStopped(object sender, EventArgs e)
        {
            int index = channelServerStopped.FindIndex(s => s.Item2 == sender);
            if (index == -1)
                return;
            (IMessageChannel channel, IGameServer server) = channelServerStopped[index];
            channel.SendMessageAsync($"{GetServerStatusEmote(ApplicationServerState.OFFLINE)} The **{server.displayName}** {server.typeName} is now offline");
            channelServerStopped.RemoveAt(index);
        }

        [Command("WriteToServer")]
        [Summary("Attempts to write a string to the currently running server in the specified group using a ServerGroup Index")]
        [RequireOwner()]
        public async Task CommandWriteToServerServer(int groupIndex, [Remainder] string message)
        {
            groupIndex--;

            if (groupIndex < 0 || groupIndex >= GameServersManager.applicationServerGroups.Count)
                throw new CommandExecutionException($"The group [{groupIndex + 1}] doesn't exist, group indexs are in the range of 1 to {GameServersManager.applicationServerGroups.Count}");

            IGameServer server = GameServersManager.applicationServerGroups[groupIndex].GetCurrentRunningServer();
            if (server is CmdGameServer)
                (server as CmdGameServer).WriteToConsole(message);
            else
                throw new CommandExecutionException("Couldn't send the message to the server as it isn't a Console type GameServer");
            await ReplyAsync($"Succesfully wrote \"{message}\" to the **{server.displayName}** {server.typeName} server");
        }

    }

}
