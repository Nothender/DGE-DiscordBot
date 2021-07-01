using Discord.Commands;
using DiscordGameEngine.ApplicationServers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordGameEngine.UI.Commands
{
    public class AppServerCommands : ModuleBase<SocketCommandContext>
    {

        private static MinecraftServer server = new MinecraftServer("C:\\Users\\trist\\OneDrive\\Documents\\Work\\Nothender\\Projects\\DiscordGameEngine\\TestMinecraftServer\\StartServer.bat", "localhost");

        [Command("ASValue")]
        public async Task CommandInteractValue()
        {
            await ReplyAsync("Bagouette : " + server.isOnline);
        }

        [Command("ASStart")]
        public async Task CommandStartValue()
        {
            server.Start();
            await ReplyAsync("Started");
        }

        [Command("ASStop")]
        public async Task CommandStopValue()
        {
            server.Stop();
            await ReplyAsync("Stopped");
        }

        [Command("ASUsers")]
        public async Task CommandUsersValue()
        {
            await ReplyAsync("Users :" + string.Join('\n', server.usersConnectedUsernames));
        }
    }
}
