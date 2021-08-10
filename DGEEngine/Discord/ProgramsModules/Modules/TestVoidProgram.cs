using DGE.Bot;
using DGE.ProgramModules;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DGE.ProgramModules
{
    public class TestVoidProgram : ProgramModule
    {

        static TestVoidProgram()
        {
            SetDescription(typeof(TestVoidProgram), "ProgramModule restoring tests, keywords : value, taket ime");
        }

        private int value;

        public TestVoidProgram(ProgramData data, IBot bot) : base(data, bot) { }

        public TestVoidProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id, context.Client);
            AddInteraction("value", ValueCallback);
            AddInteraction("taketime", TakeTime);
        }

        private void ValueCallback(SocketUserMessage umessage)
        {
            value = int.Parse(umessage.Content.Split(' ')[1]);
            umessage.Channel.SendMessageAsync("42");
        }

        private void TakeTime(SocketUserMessage umessage)
        {
            Thread.Sleep(4200);
            umessage.Channel.SendMessageAsync("TookTime").Wait();
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
            //value = (int) data[0];
        }
    }
}
