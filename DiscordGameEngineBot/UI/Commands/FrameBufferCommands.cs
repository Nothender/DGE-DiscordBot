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
        public async Task DrawToFrameBuffer(int x, int y, int r, int g, int b, int? a)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(a ?? 255, r, g, b);
            frameBuffer.Draw(x, y, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a px {color.ToString()} at x={x} y={y}.");

        }

        [Command("drawLineToFB")]
        public async Task DrawLineToFrameBuffer(int x1, int y1, int x2, int y2, int r, int g, int b, int? a)
        {

            System.Drawing.Color color = System.Drawing.Color.FromArgb(a ?? 255, r, g, b);
            frameBuffer.DrawLine(x1, y1, x2, y2, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a line of {color.ToString()} from x1={x1} y1={y1} to x2={x2} y2={y2}.");
        }

        [Command("drawRectToFB")]
        public async Task DrawRectToFrameBuffer(int x, int y, int sizeX, int sizeY, int r, int g, int b, int? a)
        {

            System.Drawing.Color color = System.Drawing.Color.FromArgb(a ?? 255, r, g, b);
            frameBuffer.DrawRect(x, y, sizeX, sizeY, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a rectangle of {color.ToString()} from x={x} y={y} to x={x + sizeX} y={y + sizeY}.");
        }

        [Command("clearFB")]
        public async Task ClearFrameBuffer()
        {
            frameBuffer.Clear();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly cleared the Frame Buffer.");
        }

        [Command("setFBPixelDrawMode")]
        public async Task SetFrameBufferPixelDrawMode(string mode)
        {
            switch (mode.ToLower())
            {
                case "replace":
                    frameBuffer.pixelDrawMode = PixelDrawMode.REPLACE;
                    break;
                case "alpha_blending":
                    frameBuffer.pixelDrawMode = PixelDrawMode.ALPHA_BLENDING;
                    break;
                case "alphablending":
                    frameBuffer.pixelDrawMode = PixelDrawMode.ALPHA_BLENDING;
                    mode = "alpha_blending";
                    break;
                default:
                    mode = "normal";
                    frameBuffer.pixelDrawMode = PixelDrawMode.NORMAL;
                    break;
            }
            await ReplyAsync("{LogManager.DGE_LOG}Succesfuly set the Frame Buffer PixelRenderMode to {mode.ToUpper()}.");
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
        public async Task DrawToStringFrameBuffer(int x, int y, string pixelInfo)
        {
            string px = pixelInfo[2] + "  ";
            stringFrameBuffer.Draw(x, y, px);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew the pixel {px} at x={x} y={y}.");

        }

        [Command("clearSFB")]
        public async Task ClearStringFrameBuffer()
        {
            stringFrameBuffer.Clear();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfuly cleared the String Frame Buffer.");
        }

    }
}
