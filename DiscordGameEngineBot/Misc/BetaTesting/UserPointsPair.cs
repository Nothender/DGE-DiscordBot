using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEngine.Misc.BetaTesting
{
    public struct UserPointsPair
    {
        public IUser user;
        public int points;

        public UserPointsPair(ulong id, int points)
        {
            this.user = DiscordGameEngineBot._client.GetUser(id);
            this.points = points;
        }
    }
}
