using Discord;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DGE.Discord
{
    public class DGEInteractiveBase : InteractiveBase<DGECommandContext>
    {

        protected async Task ReactAsync(IEmote emote, RequestOptions options = null)
        {
            await Context.Message.AddReactionAsync(emote, options);
            Context.commandGotFeedback = true;
        }

        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            Context.commandGotFeedback = true;
            return await base.ReplyAsync(message, isTTS, embed, options, allowedMentions, messageReference);
        }

    }
}
