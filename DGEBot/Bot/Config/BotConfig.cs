using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Bot.Config
{
    public struct BotConfig : IBotConfig
    {
        public string Token { get; }

        public ulong? DebugGuildId { get; }

        public ulong FeedbackChannelId { get; }

        public BotConfig(string token, ulong debugGuildId, ulong feedbackChannelId)
        {
            Token = token;
            DebugGuildId = debugGuildId;
            FeedbackChannelId = feedbackChannelId;
        }

    }
}
