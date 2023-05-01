using DGE.Bot;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord
{
    public class DGEInteractionContext : InteractionContext, IDGEInteractionContext
    {
        public DGEInteractionContext(DiscordBot bot, IDiscordInteraction interaction, IMessageChannel channel = null) : base(bot.client, interaction, channel)
        {
            CommandGotFeedback = false;
            this.Bot = bot;
        }

        public IBot Bot { get; set; }

        public bool CommandGotFeedback { get; set; }
    }
}
