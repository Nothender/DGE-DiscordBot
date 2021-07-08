using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DGE.Core;
using DGE.UI.Feedback;
using EnderEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using DGE.Discord.Handlers;
using DGE;
using DGE.Application;
using DGE.Discord;

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

        public DiscordBot(string token, string commandPrefix)
        {
            appCount++;

            OnStarting += (s, e) => status = ApplicationStatus.STARTING;
            OnStarted += (s, e) => status = ApplicationStatus.ON;
            OnShutdown += (s, e) => status = ApplicationStatus.STOPPING;
            OnStopped += (s, e) => status = ApplicationStatus.OFF;

            this.commandPrefix = commandPrefix;

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true
            });
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(DiscordCommandManager.commands)
                .BuildServiceProvider();

            logger = new Logger($"DGE-Bot:{appCount}");

            client.Log += LogDebug;
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

            client.StartAsync().GetAwaiter().GetResult();
            client.SetGameAsync($"{AssemblyFramework.module} Experimental");

            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            OnShutdown?.Invoke(this, EventArgs.Empty);

            client.StopAsync().GetAwaiter().GetResult();

            OnStopped?.Invoke(this, EventArgs.Empty);
        }

        private Task LogDebug(LogMessage msg)
        {
            Logger.LogLevel logLevel = msg.Severity switch
            {
                LogSeverity.Info => Logger.LogLevel.INFO,
                LogSeverity.Verbose => Logger.LogLevel.DEBUG,
                LogSeverity.Debug => Logger.LogLevel.DEBUG,
                LogSeverity.Error => Logger.LogLevel.ERROR,
                LogSeverity.Critical => Logger.LogLevel.FATAL,
                LogSeverity.Warning => Logger.LogLevel.WARN,
                _ => Logger.LogLevel.DEBUG,
            };
            logger.Log($"({msg.Source}) {msg.Message}", logLevel);
            return Task.CompletedTask;
        }

    }
}
