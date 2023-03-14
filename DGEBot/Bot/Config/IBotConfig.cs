using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Bot.Config
{
    public interface IBotConfig
    {
        /// <summary>
        /// The bot token to connect
        /// </summary>
        public string Token { get; }
        /// <summary>
        /// Channel id for feedback and important errors to be logged to
        /// </summary>
        public ulong FeedbackChannelId { get; }
        /// <summary>
        /// While in debug mode, commands will not be registered globally but only in the debugging guild
        /// </summary>
        public ulong? DebugGuildId { get; }
    }
}
