using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine.Core;
using System.Linq;
using Discord.Commands;

namespace DiscordGameEngine.Core
{
    public abstract class ProgramModule
    {
        private static List<ProgramModule> programs = new List<ProgramModule>();
        
        public static ProgramModule GetProgramById(int id) { return programs[id]; }

        private Dictionary<string, Action<SocketUserMessage>> triggerContentCallback = new Dictionary<string, Action<SocketUserMessage>>();
        protected List<ISocketMessageChannel> interactionChannels = new List<ISocketMessageChannel>();

        public readonly int Id;

        public ProgramModule(SocketCommandContext context)
        {
            //Registering the program
            Id = programs.Count();
            programs.Add(this);
        }

        protected void AddInteraction(string trigger, Action<SocketUserMessage> callback)
        {
            triggerContentCallback.Add(trigger, callback);
        }

        /// <summary>
        /// Adds a channel that can be used to interact with the program
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns>Returns true if the item was added, false if it was already added</returns>
        public bool AddChannel(ulong channelId)
        {
            if (interactionChannels.Exists(x => x.Id == channelId))
                return false;
            ChannelListener.AddListenedChannel(channelId, OnInteraction);
            interactionChannels.Add(DGEMain._client.GetChannel(channelId) as ISocketMessageChannel);
            return true;
        }

        //TODO: fix existing programs to use new methods + add commands to change program context

        protected abstract void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage);

        private void OnInteraction(ulong channelId, SocketUserMessage umessage)
        {
            int indexOfFirstSpace = umessage.Content.IndexOf(' ');
            string triggerWord = umessage.Content.Substring(0, indexOfFirstSpace < 0 ? umessage.Content.Length : indexOfFirstSpace);
            if (triggerContentCallback.ContainsKey(triggerWord))
                triggerContentCallback[triggerWord](umessage);
            else
                CallbackNoTriggerMessageRecieved(umessage);
        }

    }

}
