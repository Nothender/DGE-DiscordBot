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
using Discord.Addons.Interactive;
using Discord.Commands;

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

        public DiscordSocketClient client { get; protected set; }
        public IServiceProvider services { get; protected set; }
        public CommandService commandsService { get; protected set; }
        public InteractiveService interactiveServices { get; protected set; }

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

        private ulong feedbackChannelId;

        public DiscordBot(string token, string commandPrefix, ulong feedbackChannelId)
        {
            appCount++;

            OnStarting += (s, e) => status = ApplicationStatus.STARTING;
            OnStarted += (s, e) => status = ApplicationStatus.ON;
            OnShutdown += (s, e) => status = ApplicationStatus.STOPPING;
            OnStopped += (s, e) => status = ApplicationStatus.OFF;

            this.commandPrefix = commandPrefix;
            this.feedbackChannelId = feedbackChannelId;

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true
            });

            commandsService = new CommandService();

            interactiveServices = new InteractiveService(client, new InteractiveServiceConfig{ DefaultTimeout = new TimeSpan(0, 0, interactiveTimeoutSeconds) });

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commandsService)
                .AddSingleton(interactiveServices)
                .BuildServiceProvider();

            logger = new Logger($"DGE-Bot:{appCount}");

            client.Log += (m) => DiscordNETLogger.Log(m, logger);
            client.MessageReceived += (m) => MessageHandler.HandleMessageAsync(m, this);

            client.LoginAsync(TokenType.Bot, token);
        }

        public override void Dispose()
        {
            Stop();
            client.LogoutAsync().Wait(); //When quitting the application the application may not have finished disposing, so we wait for it as to not cause memory leaks or anything
            client.Dispose();
        }

        public override void Start()
        {
            if ((int)status > 1) //App already on or starting
                return;
            OnStarting?.Invoke(this, EventArgs.Empty);

            client.Ready += StartedBotReady;
            client.StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartedBotReady()
        {
            client.Ready -= StartedBotReady;

            if (feedbackChannel is null)
                feedbackChannel = client.GetChannel(feedbackChannelId) as IMessageChannel;
            await client.SetGameAsync(activity);

            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public override void Stop()
        {
            OnShutdown?.Invoke(this, EventArgs.Empty);

            client.StopAsync().GetAwaiter().GetResult();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        public void RegisterCommandModule(Type module)
        {
            commandsService.AddModuleAsync(module, services);
            logger.Log($"Loaded command module [{module.Name}]", Logger.LogLevel.INFO);
        }

    }
}