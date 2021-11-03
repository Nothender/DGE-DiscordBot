using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord.WebSocket;

namespace DGE.Discord
{
    public static class ChannelListener
    {

        private static Dictionary<ulong, Action<ulong, SocketUserMessage>[]> channelCallbacks = new Dictionary<ulong, Action<ulong, SocketUserMessage>[]>();

        public static void RemoveListenedChannel(ulong channelId, Action<ulong, SocketUserMessage> callback)
        {
            if (channelCallbacks.ContainsKey(channelId) && channelCallbacks[channelId].Contains(callback))
            {
                List<Action<ulong, SocketUserMessage>> callbacks = channelCallbacks[channelId].ToList();
                callbacks.Remove(callback);
                channelCallbacks[channelId] = callbacks.ToArray();
            }
        }

        public static void AddListenedChannel(ulong channelId, Action<ulong, SocketUserMessage> callback)
        {
            if (channelCallbacks.ContainsKey(channelId))
            {
                channelCallbacks[channelId] = channelCallbacks[channelId].Concat(new Action<ulong, SocketUserMessage>[] { callback }).ToArray();
            }
            else
                channelCallbacks.Add(channelId, new Action<ulong, SocketUserMessage>[] { callback });
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
