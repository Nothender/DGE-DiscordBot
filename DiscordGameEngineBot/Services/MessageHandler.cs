using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGameEngine.Services
{
    internal static class MessageHandler
    {
        internal static async Task HandleMessageAsync(SocketMessage message)
        {
            SocketUserMessage uMessage = message as SocketUserMessage;
            string msg = message.Content.ToLower();
            int argPos = 0;
            if (uMessage.HasStringPrefix(DiscordGameEngineBot.commandPrefix, ref argPos))// && !uMessage.Author.IsBot)
            {
                SocketCommandContext context = new SocketCommandContext(DiscordGameEngineBot._client, uMessage);

                await CommandHandler.ExecuteCommand(context, argPos);
            }
            else
            {
                _ = Task.Run(() =>
                  {
                      try
                      {
                          if (!uMessage.HasStringPrefix(DiscordGameEngineBot.commandPrefix, ref argPos) && uMessage.Author.Id != DiscordGameEngineBot._client.CurrentUser.Id)
                          {
                              if (ChannelListener.IsChannelListened(uMessage.Channel.Id))
                                  ChannelListener.MessageRecieved(uMessage.Channel.Id, uMessage);
                          }
                      }
                      catch (Exception e)
                      {
                          message.Channel.SendMessageAsync(LogManager.DGE_ERROR + "Failed reading/executing message : " + e.Message);
                      }
                  });

            }
        }

    }
}
