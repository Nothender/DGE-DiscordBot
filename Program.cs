using Discord;
using Discord.API;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot207
{
    public class Program
    {

        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public static SocketGuild server;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Main function executed
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true
            });
            _services = new ServiceCollection().AddSingleton(_client).BuildServiceProvider();

            await RegisterMethodsAsync();
            
            //Gets all the infos for the bot to run
            //index { 0 : Token }
            string[] infos = File.ReadAllLines("../../../infos.txt"); // CHANGE ACCESS PATH FOR RELEASE

            await _client.LoginAsync(TokenType.Bot, infos[0]);
            await _client.StartAsync();

            //await server.GetUser(638408938727014458).AddRoleAsync(server.GetRole(585555863373217930));
            //await (server.GetUser(638408938727014458) as IGuildUser).AddRoleAsync(server.GetRole(585555863373217930));

            await Task.Delay(-1);
        }

        private Task RegisterMethodsAsync()
        {
            _client.Log += LogDebug;
            _client.UserVoiceStateUpdated += VoiceChannelsManager.UserVoiceStateChange;
            return Task.CompletedTask;
        }

        private Task AssignServer(SocketGuild guild)
        {
            server = guild;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called at each log event from discord : used to debug
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Task LogDebug(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}
