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

namespace DiscordGameEngine
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        internal static StringFrameBuffer stringFrameBuffer;
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

        [Command("clearImageBuffersDir")]
        public async Task ClearImageBuffersDir()
        {
            FrameBuffer.ClearStoredImageBuffers();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully Cleared the Image Frame Buffers Dir.");
        }

        [Command("initFB")]
        public async Task CreateFrameBuffer()
        {
            if (frameBuffer != null)
            {
                await ReplyAsync(LogManager.DGE_WARN + "Couldn't initialize the Frame Buffer because it is already initialized.");
                return;
            }
            frameBuffer = new FrameBuffer(new Size(480 * 2, 270 * 2), System.Drawing.Color.FromArgb(42, 42, 42));
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully initialized the Frame Buffer.");
        }

        [Command("displayFB")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task DisplayFrameBuffer()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            frameBuffer.Render();
            frameBuffer.Display(Context);
        }

        [Command("drawToFB")]
        public async Task DrawToFrameBuffer(params string[] pixelInfo)
        {
            if (pixelInfo.Length < 5)
                await ReplyAsync(LogManager.DGE_WARN + "The 5 following arguments are required : int xPos, int yPos, int r, int g, int b.");
            else
            {
                System.Drawing.Color color = System.Drawing.Color.FromArgb(int.Parse(pixelInfo[2]), int.Parse(pixelInfo[3]), int.Parse(pixelInfo[4]));
                int x = int.Parse(pixelInfo[0]);
                int y = int.Parse(pixelInfo[1]);
                frameBuffer.Draw(x, y, color);
                await ReplyAsync(LogManager.DGE_LOG + "Succesfuly drew a px " + color.ToString() + " at x=" + x + " y=" + y + '.');
            }
        }

        [Command("drawRectToFB")]
        public async Task DrawRectToFrameBuffer(params string[] rectInfo)
        {
            if (rectInfo.Length < 7)
                await ReplyAsync(LogManager.DGE_WARN + "The 7 following arguments are required : int xPos, int yPos, int xSize, int ySize, int r, int g, int b.");
            else
            {
                System.Drawing.Color color = System.Drawing.Color.FromArgb(int.Parse(rectInfo[4]), int.Parse(rectInfo[5]), int.Parse(rectInfo[6]));
                int x = int.Parse(rectInfo[0]);
                int y = int.Parse(rectInfo[1]);
                int xSize = int.Parse(rectInfo[2]);
                int ySize = int.Parse(rectInfo[3]);
                frameBuffer.DrawRect(x, y, xSize, ySize, color);
                await ReplyAsync(LogManager.DGE_LOG + "Succesfuly drew a rectangle of " + color.ToString() + " from x=" + x + " y=" + y + " to x=" + (x + xSize) + " y=" + (y + ySize) + '.');
            }
        }

        [Command("clearFB")]
        public async Task ClearFrameBuffer()
        {
            frameBuffer.Clear();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly cleared the Frame Buffer.");
        }

        [Command("initSFB")]
        public async Task CreateStringFrameBuffer()
        {
            if (stringFrameBuffer != null)
            {
                await ReplyAsync(LogManager.DGE_WARN + "Couldn't initialize the String Frame Buffer because it is already initialized.");
                return;
            }
            stringFrameBuffer = new StringFrameBuffer(8, 8, ":red_square:");
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully initialized the String Frame Buffer.");
        }

        [Command("displaySFB")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task DisplayStringFrameBuffer()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            stringFrameBuffer.Display(Context);
            // await ReplyAsync("");
        }

        [Command("drawToSFB")]
        public async Task DrawToStringFrameBuffer(params string[] pixelInfo)
        {
            if (pixelInfo.Length < 3)
                await ReplyAsync(LogManager.DGE_WARN + "The 3 following arguments are required : int xPos, int yPos, string pixelInfo.");
            else
            {
                string px = pixelInfo[2] + "  ";
                int x = int.Parse(pixelInfo[0]);
                int y = int.Parse(pixelInfo[1]);
                stringFrameBuffer.Draw(x, y, px);
                await ReplyAsync(LogManager.DGE_LOG + "Succesfuly drew a px " + px + "at x=" + x + " y=" + y + '.');
            }
        }

        [Command("clearSFB")]
        public async Task ClearStringFrameBuffer()
        {
            stringFrameBuffer.Clear();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly cleared the String Frame Buffer.");
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
