using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using DiscordGameEnginePlus.Programs;

namespace DiscordGameEngineProgram
{
    public class CommandsExemple : ModuleBase<SocketCommandContext>
    {

        [Command("RenameChannel")]
        public async Task RenameChannel(params string[] args)
        {
            string channelName = args.Length > 0 ? string.Join(' ', args) : Context.Channel.Name;
            SocketGuildChannel channel = Context.Channel as SocketGuildChannel;
            await channel.ModifyAsync(c => c.Name = channelName);
            await ReplyAsync("Renamed channel to : " + channelName);
        }

    }
}
