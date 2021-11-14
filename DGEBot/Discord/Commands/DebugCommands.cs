using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EnderEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DGE.Core;

namespace DGE.Discord.Commands
{

    [Summary("Commands to test misc bot features")]
    public class DebugCommands : DGEModuleBase
    {

        [Command("LogTest")]
        [RequireUserPermission(ChannelPermission.MentionEveryone, Group = "debug.log")]
        [RequireOwner(Group = "debug.log")]
        [Summary("tests the DGE logging prefixes, the log level has to be ERROR, LOG, WARN, or DEBUG, the rest of the message is counted as single string to be used after the prefix")]
        public async Task LogTest(string logLevel, [Remainder] string message)
        {
            string prefix;
            switch (logLevel.ToUpper())
            {
                case "ERROR":
                    prefix = LogPrefixes.DGE_ERROR;
                    break;
                case "LOG":
                    prefix = LogPrefixes.DGE_LOG;
                    break;
                case "WARN":
                    prefix = LogPrefixes.DGE_WARN;
                    break;
                case "DEBUG":
                    prefix = LogPrefixes.DGE_DEBUG;
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

        [Command("ShowPaths")]
        [Summary("Shows the bots file paths")]
        [RequireOwner]
        public async Task CommandShowPaths()
        {
            await ReplyAsync(
                "not operational"
                );
        }

        [Command("ShowModules")]
        [Summary("Shows the currently loaded assembly DGEModules")]
        public async Task CommandShowDGEModules()
        {
            await ReplyAsync("Currently loaded DGE modules in assembly :\n" + string.Join("\n", DGEModules.modules));
        }

        [Command("ShowGuilds")]
        [Summary("Shows the guilds in which the bot is")]
        public async Task CommandShowGuilds()
        {
            await ReplyAsync("Joined guilds :\n - " + string.Join("\n - ", Context.bot.client.Guilds));
        }

        [Command("ShowApps")]
        [RequireOwner]
        [Summary("Shows the applications in DGE-Framework")]
        public async Task CommandShowApps()
        {
            string res = "Instanced applications :";
            foreach (Application.IApplication app in Application.ApplicationManager.GetAll()) res += $"\n - {app.GetType().Name} application of id {app.Id}, currently {app.status}";
            await ReplyAsync(res);
        }
    }
}
