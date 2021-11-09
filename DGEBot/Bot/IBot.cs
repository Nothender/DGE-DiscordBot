using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;

namespace DGE.Bot
{
    public interface IBot : Application.IApplication
    {
        public IServiceProvider services { get; }
        public CommandService commandsService { get; }
        public DiscordSocketClient client { get; }

        public IMessageChannel feedbackChannel { get; set; }

        public string commandPrefix { get; set; }

        public void RegisterCommandModule(Type module);

    }
}