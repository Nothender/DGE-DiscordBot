using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord
{
    public class DGEModuleBase : ModuleBase<IDGECommandContext>
    {
        
        protected async Task ReactAsync(IEmote emote, RequestOptions options = null)
        {
            await Context.Message.AddReactionAsync(emote, options);
            Context.commandGotFeedback = true;
        }

        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null, MessageComponent components = null, ISticker[] stickers = null, Embed[] embeds = null)
        {
            Context.commandGotFeedback = true;
            return await base.ReplyAsync(message, isTTS, embed, options, allowedMentions, messageReference, components, stickers, embeds);
        }

    }
}
