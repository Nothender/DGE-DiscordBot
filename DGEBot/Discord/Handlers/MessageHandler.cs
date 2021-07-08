using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DGE.Core;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DGE.Misc;
using DGE.Bot;

namespace DGE.Discord.Handlers
{
    internal static class MessageHandler
    {
        internal static async Task HandleMessageAsync(IMessage message, DiscordBot bot)
        {
            SocketUserMessage uMessage = message as SocketUserMessage;
            string msg = message.Content.ToLower();
            int argPos = 0;
            if (uMessage.HasStringPrefix(bot.commandPrefix, ref argPos))// && !uMessage.Author.IsBot)
            {
                DGECommandContext context = new DGECommandContext(bot.client, uMessage, bot);

                await CommandHandler.ExecuteCommand(context, argPos);
            }
            else
            {
                _ = Task.Run(() =>
                  {
                      try
                      {
                          if (!uMessage.HasStringPrefix(bot.commandPrefix, ref argPos) && uMessage.Author.Id != bot.client.CurrentUser.Id)
                          {
                              if (ChannelListener.IsChannelListened(uMessage.Channel.Id))
                                  ChannelListener.MessageRecieved(uMessage.Channel.Id, uMessage);
                          }
                      }
                      catch (Exception e)
                      {
                          message.Channel.SendMessageAsync(LogPrefixes.DGE_ERROR + "Failed reading/executing message : " + e.Message);
                      }
                  });

            }
        }

    }
}
