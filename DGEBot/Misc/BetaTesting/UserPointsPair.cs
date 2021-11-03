using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using DGE.Bot;
using Discord.WebSocket;

namespace DGE.Misc.BetaTesting
{
    public struct UserPointsPair
    {
        public IUser user;
        public int points;

        public UserPointsPair(ulong id, int points, DiscordSocketClient client)
        {
            user = client.GetUser(id);
            this.points = points;
        }
    }
}
