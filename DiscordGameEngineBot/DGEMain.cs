using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using DiscordGameEngine.Misc;
using DiscordGameEngine.UI.Commands;
using EnderEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordGameEngine
{
    public static class DGEMain
    {
        private static bool isShutDown = false;

        internal static DiscordSocketClient _client;
        private static CommandService _commands;
        private static IServiceProvider _services;

        private static string commandPrefix = "//";

        internal static Logger DGELogger = new Logger("DGE");
        public static Logger DGELoggerProgram = new Logger("DGEProgram");

        public static EventHandler OnShutdown;

        /// <summary>
        /// Main function executed
        /// </summary>
        /// <returns></returns>
        public static async Task MainAsync()
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

            RegisterCommandModules();
            await RegisterMethodsAsync();

            //Gets all the infos for the bot to run
            //index { 0 : Token }
            string[] infos = File.ReadAllLines("../../../infos.txt"); // CHANGE ACCESS PATH FOR RELEASE

            await _client.LoginAsync(TokenType.Bot, infos[0]);
            await _client.StartAsync();
            await Startups();

            while (!isShutDown)
                await Task.Delay(100);
        }

        /// <summary>
        /// Make sure to use this method to ensure the engine is shutting down properly
        /// </summary>
        public static void Shutdown()
        {
            OnShutdown?.Invoke(null, null);
            _client.LogoutAsync();
            isShutDown = true;
        }

        private static async Task RegisterMethodsAsync()
        {
            //Client method registry
            _client.Log += LogManager.LogDebug;
            //Commands method registry
            _client.MessageReceived += HandleMessagesAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private static async Task HandleMessagesAsync(SocketMessage message)
        {
            SocketUserMessage uMessage = message as SocketUserMessage;
            int argPos = 0;
            if (uMessage.HasStringPrefix(commandPrefix, ref argPos) && !uMessage.Author.IsBot)
            {
                SocketCommandContext context = new SocketCommandContext(_client, uMessage);

                IResult execution = await _commands.ExecuteAsync(context, argPos, _services);
                if (!execution.IsSuccess)
                {
                    await LogManager.LogDebug(new LogMessage(LogSeverity.Warning, "Commands", execution.ErrorReason));
                    await context.Channel.SendMessageAsync(LogManager.DGE_ERROR + "Command execution failed : " + execution.ErrorReason);
                }
            }
            else
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        if (!uMessage.HasStringPrefix(commandPrefix, ref argPos) && uMessage.Author.Id != _client.CurrentUser.Id)
                        {
                            if (ChannelListener.IsChannelListened(uMessage.Channel.Id))
                                ChannelListener.MessageRecieved(uMessage.Channel.Id, uMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        message.Channel.SendMessageAsync(LogManager.DGE_ERROR + "Failed reading/executing message : " + e.Message);
                    }
                });
            }
        }

        private static Task Startups()
        {
            // Note : An event handler is not used here, every class is started individually, allowing them to subscribe to other events
            Console.WriteLine("Starting up...");

            Logger.SetDefaultLoggingMethod(Logger.LogMethod.TO_CONSOLE);
            ConsoleCommands().ConfigureAwait(false);

            Core.Core.Start();

            Console.WriteLine("Startup complete");
            return Task.CompletedTask;
        }

        public static void AddCommandModule(Type type)
        {
            _commands.AddModuleAsync(type, _services);
        }

        private static void RegisterCommandModules()
        {
            AddCommandModule(typeof(Commands));
            AddCommandModule(typeof(FrameBufferCommands));
            AddCommandModule(typeof(FunCommands));
        }

        /// <summary>
        /// Method that runs the management of the console interactions
        /// </summary>
        private async static Task ConsoleCommands()
        {
            await Task.Run(() =>
            {
                while (Console.ReadLine() != "stop") ;
                Shutdown();
            });
        }

    }
}