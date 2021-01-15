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
    public class FrameBufferCommands : ModuleBase<SocketCommandContext>
    {

        internal static StringFrameBuffer stringFrameBuffer;
        internal static FrameBuffer frameBuffer;

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
            frameBuffer = new FrameBuffer(new Size(8, 8), System.Drawing.Color.FromArgb(42, 42, 42), true, new Size(800, 800));
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully initialized the Frame Buffer.");
        }

        [Command("displayFB")]
        public async Task DisplayFrameBuffer()
        {
            await Task.Run(() =>
            {
                frameBuffer.Render();
                frameBuffer.Display(Context);
            });
        }

        [Command("drawToFB")]
        public async Task DrawToFrameBuffer(params string[] pixelInfo)
        {
            if (pixelInfo.Length < 5)
                await ReplyAsync(LogManager.DGE_WARN + "The 5-6 following arguments are required : int xPos, int yPos, int r, int g, int b, (optional) int a.");
            else
            {
                int a = 255;
                if (pixelInfo.Length > 5)
                    a = int.Parse(pixelInfo[5]);
                System.Drawing.Color color = System.Drawing.Color.FromArgb(a, int.Parse(pixelInfo[2]), int.Parse(pixelInfo[3]), int.Parse(pixelInfo[4]));
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
                await ReplyAsync(LogManager.DGE_WARN + "The 7-8 following arguments are required : int xPos, int yPos, int xSize, int ySize, int r, int g, int b, (optional) int a.");
            else
            {
                int a = 255;
                if (rectInfo.Length > 7)
                    a = int.Parse(rectInfo[7]);
                System.Drawing.Color color = System.Drawing.Color.FromArgb(a, int.Parse(rectInfo[4]), int.Parse(rectInfo[5]), int.Parse(rectInfo[6]));
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

        [Command("setFBPixelDrawMode")]
        public async Task SetFrameBufferPixelDrawMode(params string[] mode)
        {
            mode[0] = mode[0].ToLower();
            switch (mode[0])
            {
                case "replace":
                    frameBuffer.pixelDrawMode = PixelDrawMode.REPLACE;
                    break;
                case "alpha_blending":
                    frameBuffer.pixelDrawMode = PixelDrawMode.ALPHA_BLENDING;
                    break;
                case "alphablending":
                    frameBuffer.pixelDrawMode = PixelDrawMode.ALPHA_BLENDING;
                    mode[0] = "alpha_blending";
                    break;
                default:
                    mode[0] = "normal";
                    frameBuffer.pixelDrawMode = PixelDrawMode.NORMAL;
                    break;
            }
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly set the Frame Buffer PixelRenderMode to " + mode[0].ToUpper() + ".");
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

    }
}
