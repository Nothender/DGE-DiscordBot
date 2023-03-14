using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;

namespace DGE.Bot
{
    public interface IBot : Application.IApplication
    {
        public IServiceProvider services { get; }
        public InteractionService interactionService { get; }
        public DiscordSocketClient client { get; }

        public IMessageChannel feedbackChannel { get; set; }

        public void LoadCommandModule(Type module);

    }
}