using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Config
{
    public struct Config : IConfig
    {
        public string Token { get; }

        public string Prefix { get; }

        public ulong FeedbackChannelId { get; }

        public Config(string token, string prefix, ulong feedbackChannelId)
        {
            Token = token;
            Prefix = prefix;
            FeedbackChannelId = feedbackChannelId;
        }

    }
}
