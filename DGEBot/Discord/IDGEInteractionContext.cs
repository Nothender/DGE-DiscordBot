using DGE.Bot;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DGE.Discord
{
    public interface IDGEInteractionContext : IInteractionContext
    {
        IBot Bot { get; }
        /// <summary>
        /// If the command the user sent had a reply message or reaction as feedback 
        /// </summary>
        bool CommandGotFeedback { get; set; }

    }
}