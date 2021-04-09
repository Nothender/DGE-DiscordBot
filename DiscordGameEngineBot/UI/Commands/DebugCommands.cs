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
using System.Linq;
using DiscordGameEngine.Services;
using DiscordGameEngine.Exceptions;

namespace DiscordGameEngine.UI.Commands
{

    [Summary("Commands to test misc bot features")]
    public class DebugCommands : ModuleBase<SocketCommandContext>
    {

        [Command("LogTest")]
        [RequireUserPermission(ChannelPermission.MentionEveryone)]
        [Summary("tests the DGE logging prefixes, the log level has to be ERROR, LOG, WARN, or DEBUG, the rest of the message is counted as single string to be used after the prefix")]
        public async Task LogTest(string logLevel, [Remainder] string message)
        {
            string prefix;
            switch (logLevel.ToUpper())
            {
                case "ERROR":
                    prefix = LogManager.DGE_ERROR;
                    break;
                case "LOG":
                    prefix = LogManager.DGE_LOG;
                    break;
                case "WARN":
                    prefix = LogManager.DGE_WARN;
                    break;
                case "DEBUG":
                    prefix = LogManager.DGE_DEBUG;
                    break;
                default:
                    prefix = "nope : ";
                    break;
            }
            await ReplyAsync(prefix + message);
        }

        [Command("TriggerException")]
        [Summary("Generates a new exception that will cause a BugReport to be created using the given string as exception")]
        [RequireOwner]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task CommandTriggerException([Remainder] string exceptionMessage)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new Exception(exceptionMessage);
        }

    }
}
