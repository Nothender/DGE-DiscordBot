using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine.UI.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        /// <summary>
        /// Returns a string which is the result of the concatenation from args[beginIndex] to args[args.Length-1] with a space in between each elements
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

        [Command("pong")]
        public async Task Pong()
        {
            await ReplyAsync("Ping");
        }

        [Command("logTest")]
        public async Task LogTest(params string[] logInfo)
        {
            if (logInfo.Length < 1)
            {
                await ReplyAsync(LogManager.DGE_ERROR + "No logmode/msg has been provided.");
                return;
            }
            string prefix;
            switch (logInfo[0])
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
            await ReplyAsync(prefix + GetArgsAsSingleStringFrom(logInfo, 1));
        }

    }
}
