using DGE.Application;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Bot
{
    public interface IBot : IApplication
    {
        public IServiceProvider services { get; }
        public DiscordSocketClient client { get; }

        public string commandPrefix { get; set; }

    }
}
