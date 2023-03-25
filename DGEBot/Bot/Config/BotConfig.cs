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

        public ICommandModuleConfig[] Modules { get; set; }

        public BotConfig(string token, ulong debugGuildId, ulong feedbackChannelId, ICommandModuleConfig[] modules)
        {
            Token = token;
            DebugGuildId = debugGuildId;
            FeedbackChannelId = feedbackChannelId;
            Modules = modules;
        }

    }
}
