using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEnginePlus.Programs
{
    public class BonapioProgram : ProgramModule
    {

        public BonapioProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
            AddInteraction("profile", ProfileCallback);
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage) 
        {
            umessage.Channel.SendMessageAsync("42");
        }

        private void ProfileCallback(SocketUserMessage umessage)
        {
            umessage.Channel.SendMessageAsync(DiscordGameEngine.Core.LogManager.DGE_ERROR + "haha no, what'd you thought ?");
        }

    }
}
