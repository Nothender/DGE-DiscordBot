using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordGameEnginePlus.Programs
{
    public class InteractionProgram : ProgramModule
    {
        public InteractionProgram(ProgramData programData) : base(programData) { }

        public Dictionary<string, string> actionAnswer = new Dictionary<string, string>();

        public InteractionProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
            AddInteraction("add", AddCallback);
            AddInteraction("broadcast", Broadcast);
            AddActionAnswer("42", "42");
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

        public void Broadcast(SocketUserMessage umessage)
        {
            string message = "42 " + umessage.Content.Remove(0, 10);
            foreach (ISocketMessageChannel channel in interactionChannels)
                channel.SendMessageAsync(message);
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
