using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bot207
{
    /// <summary>
    /// Contains various methods to manage the voice channels
    /// </summary>
    public static class VoiceChannelsManager
    {
        /// <summary>
        /// This task is ran when a user changes voice state
        /// </summary>
        /// <param name="user"></param>
        /// <param name="b_voiceState"></param>
        /// <param name="e_voiceState"></param>
        /// <returns></returns>
        public static Task UserVoiceStateChange(SocketUser user, SocketVoiceState b_voiceState, SocketVoiceState e_voiceState)
        {
            /*if (e_voiceState.VoiceChannel != null || )
            {

            }*/
            return Task.CompletedTask;
        }

    }

    public struct ChannelInfo
    {
        
    }

}
