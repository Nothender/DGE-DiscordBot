using DGE.Bot;
using Discord.Commands;
using Discord.WebSocket;

namespace DGE.Discord
{
    public interface IDGECommandContext : ICommandContext
    {
        IBot bot { get; }
        /// <summary>
        /// If the command the user sent had a reply message or reaction as feedback 
        /// </summary>
        bool commandGotFeedback { get; set; }

    }
}