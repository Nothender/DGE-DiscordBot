using DGE.Application;
using DGE.Core;
using DGE.Discord;
using DGE.Discord.Handlers;
using Discord;
using Discord.WebSocket;
using EnderEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Linq;
using DGE.Exceptions;
using Discord.Webhook;
using Discord.Commands;
using DGE.Bot.Config;
using Discord.Interactions;
using System.Reflection;

namespace DGE.Bot
{
    public class DiscordBot : ApplicationBase, IBot
    {
        #region IApplication

        private static int appCount = 0;

        public override event EventHandler OnStarting;
        public override event EventHandler OnStarted;
        public override event EventHandler OnShutdown;
        public override event EventHandler OnStopped;

        #endregion IApplication

        public DiscordSocketClient client { get; set; }
        public IServiceProvider services { get; set; }
        public InteractionService interactionService { get; set; }
        //public InteractiveService interactiveServices { get; protected set; }

        public const int interactiveTimeoutSeconds = 21;

        public string commandPrefix { get; set; }

        public string activity
        {
            get { return activityStatus; }
            set
            {
                activityStatus = value;
                client.SetGameAsync(activity);
            }
        }

#if DEBUG
        private string activityStatus = $"{AssemblyFramework.module} Experimental";
#else
        private string activityStatus = $"{AssemblyBot.module} BetaTesting";
#endif

        public IMessageChannel feedbackChannel { get; set; }
        
        protected ulong? DebugGuildId;
        private ulong feedbackChannelId;

        public DiscordBot(string name) : base(name)
        {
            appCount++;
            logger = new Logger($"DGE-Bot:{name}");
        }

        public void Load(IBotConfig config, DiscordSocketConfig socketConfig) => Load(config.Token, config.DebugGuildId, config.FeedbackChannelId, config.Modules, socketConfig);

        public void Load(string token, ulong? debugGuildId, ulong feedbackChannelId, ICommandModuleConfig[] commandModules, DiscordSocketConfig socketConfig)
        {
            DebugGuildId = debugGuildId;
            client = new DiscordSocketClient(socketConfig);

            interactionService = new InteractionService(client, new InteractionServiceConfig { AutoServiceScopes = true });
            //interactiveServices = new InteractiveService(client, new InteractiveServiceConfig{ DefaultTimeout = new TimeSpan(0, 0, interactiveTimeoutSeconds) });

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(interactionService)
                .AddSingleton(socketConfig)
                .BuildServiceProvider();

            client.Log += (m) => DiscordNETLogger.Log(m, logger);
            client.MessageReceived += (m) => MessageHandler.HandleMessageAsync(m, this);
            
            // Overriding by using DGEInteractionContext, attempt at making it work
            client.InteractionCreated += async interaction =>
            {
                try
                {
                    DGEInteractionContext ctx = new DGEInteractionContext(this, interaction, interaction.Channel);
                    var res = await interactionService.ExecuteCommandAsync(ctx, services);
                }
                catch (Exception ex)
                {
                    logger.Log(ex.Message, Logger.LogLevel.ERROR);
                }
            };

            LoadCommandModules(commandModules);

            client.LoginAsync(TokenType.Bot, token);
        }


        /// <summary>
        /// Automatically registers commands according to config preferences (debug: guild, release: global)
        /// </summary>
        public async Task RegisterCommands(bool deleteMissing = true)
        {

            bool global = true;
#if DEBUG
            global = DebugGuildId is null;
#endif
            string registerScope = global ? "globally" : "to debug guild";
            try
            {
                if (global)
                    await interactionService.RegisterCommandsGloballyAsync(deleteMissing);
                else
                    await interactionService.RegisterCommandsToGuildAsync((ulong) DebugGuildId, deleteMissing);
                logger.Log($"Succesfully registered interaction commands {registerScope}", Logger.LogLevel.INFO);
            }
            catch(Exception e)
            {
                logger.Log($"Error registering interaction commands {registerScope}: {e.Message}", Logger.LogLevel.ERROR);
            }
        }

        public override void Dispose()
        {
            Stop();
            client.LogoutAsync().Wait(); //When quitting the application the application may not have finished disposing, so we wait for it as to not cause memory leaks or anything
            client.Dispose();
            appCount--;
        }

        public override void Start()
        {
            if ((int)status > 1) //App already on or starting
                return;
            OnStarting?.Invoke(this, EventArgs.Empty);

            client.Ready += StartedBotReady;
            client.StartAsync().GetAwaiter().GetResult();
        }

        private async Task StartedBotReady()
        {
            client.Ready -= StartedBotReady;

            if (feedbackChannel is null)
                feedbackChannel = client.GetChannel(feedbackChannelId) as IMessageChannel;
            await client.SetGameAsync(activity);
            //await RegisterCommands();
            
            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public override void Stop()
        {
            OnShutdown?.Invoke(this, EventArgs.Empty);
            client.StopAsync().GetAwaiter().GetResult();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        public void LoadCommandModules(ICommandModuleConfig[] modules)
        {
            Type module;
            foreach (ICommandModuleConfig mcfg in modules)
            {
#if RELEASE
                if (mcfg.DebugOnly)
                    continue;
#endif
                module = Type.GetType(mcfg.AssemblyQualifiedName);
                if (module is null)
                {
                    logger.Log($"Couldn't find module {mcfg.ModuleName} (AssemblyQualifiedName: {mcfg.AssemblyQualifiedName}) in current program", Logger.LogLevel.ERROR);
                    continue;
                }
                LoadCommandModule(module);
            }
        }

        public void LoadCommandModule(Type module)
        {
            interactionService.AddModuleAsync(module, services).GetAwaiter().GetResult();
            
            logger.Log($"Loaded command module [{module.Name}]", Logger.LogLevel.INFO);
        }

    }
}