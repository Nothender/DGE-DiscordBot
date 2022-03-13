using DGE.Bot;
using System;
using System.Collections.Generic;
using System.Text;
using DGE.Core;
using DGE.Utils;
using Discord;
using System.Threading.Tasks;

namespace DGE.Discord
{
    public class DiscordMessageStreamQueue // Represents a large flow of messages to be sent in a short time
    {

        private readonly LoopingEnumerator<DiscordBot> bots;
        private Queue<string> messageQueue;

        /// <summary>
        /// Creates a new message stream queue
        /// </summary>
        /// <param name="bots">The bots will be used and swapped from to send the messages</param>
        public DiscordMessageStreamQueue(DiscordBot[] bots)
        {
            this.bots = new LoopingEnumerator<DiscordBot>(bots);
            messageQueue = new Queue<string>(42);
        }

        public void EnqueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        /// <summary>
        /// Starts the sending all of the messages
        /// </summary>
        public async Task StartSending(ulong channelId)
        {
            DiscordBot bot;

            bots.Reset();
            bots.MoveNext();
            bot = bots.Current;

            IMessageChannel channel = await bot.client.GetChannelAsync(channelId) as IMessageChannel;

            while (messageQueue.Count > 0)
            {
                bots.MoveNext();
                bot = bots.Current;
                await channel.SendMessageAsync(messageQueue.Dequeue());
            }
        }

    }
}
