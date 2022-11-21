using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Discord.Config
{
    public interface IConfig
    {
        public string Token { get; }
        public string Prefix { get; }
        public ulong FeedbackChannelId { get; }

    }
}
