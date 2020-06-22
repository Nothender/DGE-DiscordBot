using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        internal static FrameBuffer frameBuffer;

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

        [Command("initFB")]
        public async Task CreateFrameBuffer()
        {
            if (frameBuffer != null)
            {
                await ReplyAsync(LogManager.DGE_WARN + "Couldn't initialize the Frame Buffer because it is already initialized.");
                return;
            }
            frameBuffer = new FrameBuffer(10, 10, ":black_large_square: ");
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully initialized the Frame Buffer.");
        }

        [Command("displayFB")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task DisplayFrameBuffer()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            frameBuffer.Display(Context);
            // await ReplyAsync("");
        }

        [Command("drawToFB")]
        public async Task DisplayFrameBuffer(params string[] pixelInfo)
        {
            if (pixelInfo.Length < 3)
                await ReplyAsync(LogManager.DGE_WARN + "The 3 following arguments are required : int xPos, int yPos, string pixelInfo.");
            else
            {
                string px = pixelInfo[2] + ' ';
                int x = int.Parse(pixelInfo[0]);
                int y = int.Parse(pixelInfo[1]);
                frameBuffer.Draw(x, y, px);
                await ReplyAsync(LogManager.DGE_LOG + "Succesfuly drew a px " + px + "at x=" + x + " y=" + y + '.');
            }
        }

        [Command("clearFB")]
        public async Task ClearFrameBuffer()
        {
            frameBuffer.Clear();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly cleared the Frame Buffer.");
        }

        [Command("logTest")]
        public async Task LogTest(params string[] logInfo)
        {
            if (logInfo.Length < 1)
            {
                await ReplyAsync(LogManager.DGE_ERROR + "No logmode/msg has been provided.");
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
