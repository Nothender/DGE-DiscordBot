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

namespace DGE.Bot
{
    public class DiscordBot : IBot
    {
        #region IApplication

        public ApplicationStatus status { get; protected set; }

        public Logger logger { get; protected set; }
        private static int appCount = 0;

        public event EventHandler OnStarting;

        public event EventHandler OnStarted;

        public event EventHandler OnShutdown;

        public event EventHandler OnStopped;

        #endregion IApplication

        public DiscordSocketClient client { get; protected set; }
        public IServiceProvider services { get; protected set; }

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
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(DiscordCommandManager.commands)
                .BuildServiceProvider();

            logger = new Logger($"DGE-Bot:{appCount}");

            client.Log += (m) => DiscordNETLogger.Log(m, logger);
            client.MessageReceived += (m) => MessageHandler.HandleMessageAsync(m, this);

            client.LoginAsync(TokenType.Bot, token);
        }

        public void Dispose()
        {
            Stop();
            client.LogoutAsync();
            client.Dispose();
        }

        public void Start()
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

        public void Stop()
        {
            OnShutdown?.Invoke(this, EventArgs.Empty);

            client.StopAsync().GetAwaiter().GetResult();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}