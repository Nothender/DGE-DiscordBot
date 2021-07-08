using DGE.Bot;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Discord
{
    public class DGECommandContext : SocketCommandContext
    {

        public IBot bot;

        public DGECommandContext(DiscordSocketClient client, SocketUserMessage msg, IBot bot) : base(client, msg)
        {
            this.bot = bot;
        }

    }
}
