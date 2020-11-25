using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord.WebSocket;

namespace DiscordGameEngine.Core
{
    public static class ChannelListener
    {

        private static Dictionary<ulong, Action<ulong, SocketUserMessage>[]> channelCallbacks = new Dictionary<ulong, Action<ulong, SocketUserMessage>[]>();

        public static void AddListenedChannel(ulong channelID, Action<ulong, SocketUserMessage> callback)
        {
            if (channelCallbacks.ContainsKey(channelID))
            {
                channelCallbacks[channelID] = channelCallbacks[channelID].Concat(new Action<ulong, SocketUserMessage>[] { callback }).ToArray();
            }
            else
                channelCallbacks.Add(channelID, new Action<ulong, SocketUserMessage>[] { callback });
        }

        public static void MessageRecieved(ulong channelID, SocketUserMessage message)
        {
            foreach (Action<ulong, SocketUserMessage> callback in channelCallbacks[channelID])
                callback(channelID, message);
        }

        internal static bool IsChannelListened(ulong channelID)
        {
            return channelCallbacks.ContainsKey(channelID);
        }

    }
}
