using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using DiscordGameEngine.ProgramModules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEnginePlus.Programs
{
    public class BonapioProgram : ProgramModule
    {
        public BonapioProgram(ProgramData programData) : base(programData) { }

        //public static new string description = "bagouette but derived";

        static BonapioProgram()
        {
            SetDescription(typeof(BonapioProgram), "haha null ref goes banana, can give you some profiles");
        }

        public BonapioProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
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
