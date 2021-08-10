using Discord.Commands;
using Discord.WebSocket;
using DGE.Core;
using DGE;
using DGE.Bot;
using DGE.ProgramModules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.ProgramModules
{
    public class InteractionProgram : ProgramModule
    {
        static InteractionProgram()
        {
            SetDescription(typeof(InteractionProgram), "Adds a few keywords that trigger program execution (disabled for now cause dangerous, except for the 42 keyword) :\nbroadcast (string message) -> sends the string message in every channel,\n42 -> answers 42,\nadd (string keyword, string answer) -> adds a new interaction when saying keyword the bot will answer with answer");
        }

        public InteractionProgram(ProgramData programData, IBot bot) : base(programData, bot) { }

        public Dictionary<string, string> actionAnswer = new Dictionary<string, string>();

        public InteractionProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id, context.Client);
            AddInteraction("add", DisabledCallback);
            AddInteraction("broadcast", DisabledCallback);
            AddActionAnswer("42", "42");
        }

        private void DisabledCallback(SocketUserMessage umessage)
        {
            umessage.Channel.SendMessageAsync("This keyword was disabled cause it was subject of abuse");
        }

        private void BroadcastCallback(SocketUserMessage umessage)
        {
            Broadcast(umessage.Content);
        }

        private void InteractCallback(SocketUserMessage message)
        {
            if (actionAnswer.ContainsKey(message.Content))
                message.Channel.SendMessageAsync(actionAnswer[message.Content]);
        }

        private void AddCallback(SocketUserMessage message)
        {
            string[] parameters = message.Content.Split(' ');
            AddActionAnswer(parameters[1], parameters[2]);
            message.Channel.SendMessageAsync($"Adding answer {parameters[2]} to {parameters[1]}");
        }

        public void AddActionAnswer(string action, string answer)
        {
            actionAnswer.Add(action, answer);
            AddInteraction(action, InteractCallback);
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage)
        {
        }

        protected override List<object> GetData()
        {
            return new List<object>();
        }

        protected override void LoadData(List<object> data)
        {
        }
    }
}
