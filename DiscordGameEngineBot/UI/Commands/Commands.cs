﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordGameEngine.Core;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine.UI.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        /// <summary>
        /// Returns a string which is the result of the concatenation from args[beginIndex] to args[args.Length-1] with a space in between each elements
        /// (Similar to the [Reminder] attribute)
        /// </summary>
        /// <param name="args"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        private string GetArgsAsSingleStringFrom(string[] args, int beginIndex)
        {
            if (args.Length <= beginIndex)
                return "";
            string res = "";
            for (int i = beginIndex; i < args.Length; i++)
            {
                res += args[i] + ' ';
            }
            return res;
        }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [Command("42")]
        public async Task C42()
        {
            await ReplyAsync("42");
        }


        [Command("stop")]
        public async Task Stop()
        {
            await ReplyAsync("Shutting DiscordGameEngine down...");
            DGEMain.Shutdown();
        }

        [Command("pong")]
        public async Task Pong()
        {
            await ReplyAsync("Ping");
        }

        [Command("logTest")]
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

        /*[Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, [Remainder] string reason = null)
        {
            //await user?.BanAsync();
            await ReplyAsync(user.Nickname + " 42 " + (reason != null ? reason : "no reason"));
        }*/

    }
}
