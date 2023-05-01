using Discord;
using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord
{
    public class DGEModuleBase : InteractionModuleBase<IDGEInteractionContext>
    {

        protected override Task RespondAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null)
        {
            Context.CommandGotFeedback = true;
            return base.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed);
        }
    }
}
