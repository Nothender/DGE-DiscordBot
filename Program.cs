using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordGameEngine
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private static string commandPrefix = "//";

        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

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
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await RegisterMethodsAsync();

            //Gets all the infos for the bot to run
            //index { 0 : Token }
            string[] infos = File.ReadAllLines("../../../infos.txt"); // CHANGE ACCESS PATH FOR RELEASE

            await _client.LoginAsync(TokenType.Bot, infos[0]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task RegisterMethodsAsync()
        {
            //Client method registry
            _client.Log += LogDebug;
            //Commands method registry
            _client.MessageReceived += HandleMessagesAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleMessagesAsync(SocketMessage message)
        {
            SocketUserMessage uMessage = message as SocketUserMessage;
            int argPos = 0;
            if (!uMessage.HasStringPrefix(commandPrefix, ref argPos) || uMessage.Author.IsBot)
                return;
            SocketCommandContext context = new SocketCommandContext(_client, uMessage);

            var execution = await _commands.ExecuteAsync(context, argPos, _services);
            if (!execution.IsSuccess)
                await LogDebug(new LogMessage(LogSeverity.Warning, "Commands", execution.ErrorReason));
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