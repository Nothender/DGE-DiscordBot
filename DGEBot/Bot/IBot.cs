using Discord;
using Discord.WebSocket;
using System;

namespace DGE.Bot
{
    public interface IBot : Application.IApplication
    {
        public IServiceProvider services { get; }
        public DiscordSocketClient client { get; }

        public IMessageChannel feedbackChannel { get; set; }

        public string commandPrefix { get; set; }
    }
}