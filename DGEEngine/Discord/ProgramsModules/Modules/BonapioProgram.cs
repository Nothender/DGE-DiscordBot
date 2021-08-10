using DGE.Bot;
using DGE.ProgramModules;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.ProgramModules
{
    public class BonapioProgram : ProgramModule
    {
        public BonapioProgram(ProgramData programData, IBot bot) : base(programData, bot) { }

        //public static new string description = "bagouette but derived";

        static BonapioProgram()
        {
            SetDescription(typeof(BonapioProgram), "haha null ref goes banana, can give you some profiles");
        }

        public BonapioProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id, context.Client);
            AddInteraction("profile", ProfileCallback, "AAAAA null ref cause Bonapio");
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage) 
        {
            umessage.Channel.SendMessageAsync("42");
        }

        protected override List<object> GetData()
        {
            return new List<object>();
        }

        protected override void LoadData(List<object> data)
        {
        }

        private void ProfileCallback(SocketUserMessage umessage)
        {
            throw new Exception("haha no what you'd thought");
        }

    }
}
