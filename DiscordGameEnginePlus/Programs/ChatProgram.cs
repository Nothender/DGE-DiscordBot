using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Globalization;
using DiscordGameEngine.ProgramModules;

namespace DiscordGameEnginePlus
{
    public class ChatProgram : ProgramModule
    {
        static ChatProgram()
        {
            SetDescription(typeof(ChatProgram), "Creates a link between its interactions channels, that allows users to interact between different channels (even from different servers)");
        }

        public ChatProgram(ProgramData programData) : base(programData) { }

        public ChatProgram(SocketCommandContext context) : base(context)
        {
            AddChannel(context.Channel.Id);
        }

        protected override void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage)
        {
            SendMessageAsEmbedToOtherChannels(umessage);
        }

        /// <summary>
        /// Sends the message as an embed (can send up to one attachment
        /// </summary>
        private void SendMessageAsEmbedToOtherChannels(SocketUserMessage umessage)
        {
            Embed embed = GetUserMessageBroadcastEmbed(umessage).Build();

            foreach (ISocketMessageChannel channel in interactionChannels)
            {
                if (channel.Id == umessage.Channel.Id) continue;
                channel.SendMessageAsync(null, false, embed);
            }
        }

        private EmbedBuilder GetUserMessageBroadcastEmbed(SocketUserMessage umessage)
        {
            string guildName = (umessage.Channel as SocketGuildChannel).Guild.Name;
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithCurrentTimestamp();
            builder.WithUrl(umessage.GetJumpUrl());
            builder.WithDescription($"{umessage.Author.Mention}\n{umessage.Content}");
            builder.WithAuthor($"Message from {guildName}", umessage.Author.GetAvatarUrl());
            if (umessage.Attachments.Count != 0)
            {
                var test = umessage.Attachments.GetEnumerator();
                while (test.MoveNext())
                    if (IsImageUrl(test.Current.Url)) builder.WithImageUrl(test.Current.Url);
                    else builder.AddField("Attachment", test.Current.Url);
            }
            //builder.AddField("Attachementeeeeeeee", umessage.Attachments.GetEnumerator().Current);
            //builder.AddField("", umessage.Content);
            builder.WithFooter($"in #{umessage.Channel}");
            return builder;
        }

        private bool IsImageUrl(string URL)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
            req.Method = "HEAD";
            using (WebResponse resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("image/");
            }
        }

        protected override void OnChannelRemoving(ulong channelId)
        {
            Broadcast($"The channel *#{GetChannelById(channelId).Name}* was removed from this discussion thread.");
        }

        protected override void OnChannelAdded(ulong channelId)
        {
            //Broadcast($"The channel *#{GetChannelById(channelId).Name}* was added to this discussion thread."); //Maybe find a way to make it not send message when the program is being restored
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
