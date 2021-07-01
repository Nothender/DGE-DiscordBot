using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using DiscordGameEngine.ProgramModules;
using DiscordGameEngine.Services;
using DiscordGameEngine.UI.Commands;
using DiscordGameEngine.UI.Feedback;
using EnderEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace DiscordGameEngine
{
    public static class DiscordGameEngineBot
    {
        /// <summary>
        /// The name of the Engine/Assembly
        /// </summary>
        public const string NAME = "DiscordGameEngine";
        /// <summary>
        /// The current version of the Engine in that format : Major.Minor.Fix/Small.Revision/SmallExtra
        /// </summary>
        public const string VERSION = "0.20.4.0"; //The last number can be ignored as it is for minor minor changes

        private static bool isShutDown = false;

        public static DiscordSocketClient _client; //See for usability and accessibility in FW V1 and V2
        internal static CommandService _commands;
        internal static IServiceProvider _services;

        public static readonly string commandPrefix = ">"; //To be changed to per guild or per user prefix

        internal static Logger DGELogger = new Logger("DGE");
        public static Logger DGELoggerProgram = new Logger("DGEProgram");

        public static event EventHandler OnShutdown;
        /// <summary>
        /// This EventHandler is called after the bot was started and ready
        /// </summary>
        public static event EventHandler OnStarted;

        /// <summary>
        /// Main function executed
        /// </summary>
        /// <returns></returns>
        public static async Task StartAsync()
        {
            Engine.Init();

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

            /* Gets all the infos for the bot to run
             * index {
             *  0 : Bot token (string),
             *  1 : Feedback/bug reporting DiscordMessageChannel Id (ulong)
             * } */
            string[] infos = File.ReadAllLines("infos.txt"); // CHANGE ACCESS PATH FOR RELEASE

            UserFeedbackHandler.feedbackChannelId = ulong.Parse(infos[1]);

            _client.Ready += Startups;

            await _client.LoginAsync(TokenType.Bot, infos[0]);
            await _client.StartAsync();

            while (!isShutDown)
                await Task.Delay(100);
        }

        /// <summary>
        /// Make sure to use this method to ensure the engine is shutting down properly
        /// </summary>
        public static void Shutdown()
        {
            DGELogger.Log("Shutting down...", Logger.LogLevel.INFO);
            try
            {
                OnShutdown?.Invoke(null, null);
            }
            catch (Exception e)
            {
                DGELogger.Log(e.Message, Logger.LogLevel.ERROR);
            }
            _client.LogoutAsync();
            isShutDown = true;
        }

        private static async Task RegisterMethodsAsync()
        {
            //Client method registry
            _client.Log += LogManager.LogDebug;
            //Commands method registry
            _client.MessageReceived += MessageHandler.HandleMessageAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private static Task Startups()
        {
            // Note : An event handler is not used here, every class is started individually, allowing them to subscribe to other events
            DGELogger.Log("Starting up...", Logger.LogLevel.INFO);

            ConsoleCommands().ConfigureAwait(false);

            try
            {
                ProgramModule.RestoreSavedPrograms();
            }
            catch (Exception e)
            {
                DGELogger.Log(e.Message, Logger.LogLevel.ERROR);
            }
            UserFeedbackHandler.feedbackChannel = _client.GetChannel(UserFeedbackHandler.feedbackChannelId) as ISocketMessageChannel;
            //_client.SetGameAsync($"V{string.Join(".", VERSION.Split('.', 4).Take(3))} BetaTesting", type: ActivityType.Playing); //Setting the current version (excluding the last version number)
            _client.SetGameAsync($"V{string.Join(".", VERSION.Split('.', 4).Take(3))} Experimental", type: ActivityType.Playing); //Setting the current version (excluding the last version number)

            try
            {
                OnStarted?.Invoke(null, null); //Running client start methods
            }
            catch (Exception e)
            {
                DGELogger.Log(e.Message, Logger.LogLevel.ERROR);
            }
            DGELogger.Log("Startup complete", Logger.LogLevel.INFO);
            return Task.CompletedTask;
        }

        public static void RegisterCommandModule(Type type)
        {
            _commands.AddModuleAsync(type, _services);
            DGELogger.Log($"Registered command module {type.Name}", Logger.LogLevel.INFO);
        }

        private static void RegisterCommandModules()
        {
            RegisterCommandModule(typeof(Commands));
            RegisterCommandModule(typeof(FrameBufferCommands));
            RegisterCommandModule(typeof(FunCommands));
            RegisterCommandModule(typeof(ModerationCommands));
            RegisterCommandModule(typeof(DebugCommands));
            RegisterCommandModule(typeof(DevCommands));
            RegisterCommandModule(typeof(BetaTestingCommands));
            //RegisterCommandModule(typeof(ApplicationServerCommands));
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