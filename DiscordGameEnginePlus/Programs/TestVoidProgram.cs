using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEnginePlus.Programs
{
    public class TestVoidProgram : ProgramModule
    {
        public int value;

        public TestVoidProgram(ProgramData data) : base(data) { }

        public TestVoidProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
            AddInteraction("value", ValueCallback);
        }

        private void ValueCallback(SocketUserMessage umessage)
        {
            value = int.Parse(umessage.Content.Split(' ')[1]);
            umessage.Channel.SendMessageAsync("42");
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage)
        {

        }

        protected override List<object> GetData()
        {
            return new List<object> 
            {
                
            };
        }

        protected override void LoadData(List<object> data)
        {
            value = (int) data[0];
        }
    }
}
