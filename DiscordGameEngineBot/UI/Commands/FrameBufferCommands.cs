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
using DiscordGameEngine.Exceptions;
using DiscordGameEngine.Rendering;

namespace DiscordGameEngine.UI.Commands
{

    //These commands will be removed soon (FrameBuffers and FB interactions overhaul/remake)
    [Summary("Global FrameBuffer instance manipulation commands (Deprecated)")]
    public class FrameBufferCommands : ModuleBase<SocketCommandContext>
    {

        internal static FrameBuffer frameBuffer;

        private static (int, int) clampPointToFrameBuffer(int x, int y)
        {
            
            if (x > frameBuffer.size.Width)
                x = frameBuffer.size.Width;
            if (x < 0)
                x = 0;
            if (y > frameBuffer.size.Height)
                y = frameBuffer.size.Height;
            if (y < 0)
                y = 0;
            x = Math.Abs(x % frameBuffer.size.Width);
            y = Math.Abs(y % frameBuffer.size.Width);
            return (x, y); //new Tuple<int, int>(x, y);
        }

        private static System.Drawing.Color newClampedColor(int a, int r, int g, int b)
        {
            return System.Drawing.Color.FromArgb(Math.Abs(a % 255), Math.Abs(r % 255), Math.Abs(g % 255), Math.Abs(b % 255));
        }

        [Command("InitFB")]
        [Summary("Inits the FrameBuffer")]
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

        [Command("DisplayFB")]
        [Summary("Renders and displays the frame buffer")]
        public async Task DisplayFrameBuffer()
        {
            if (frameBuffer is null)
                throw new CommandExecutionException("Cannot display the frame buffer if it was not initialized", new NullReferenceException());
            await Task.Run(() =>
            {
                frameBuffer.Render();
                frameBuffer.Display(Context.Channel);
            });
        }

        [Command("DrawToFB")]
        [Summary("Draws a pixel to the x, y coordinates, of color r, g, b, a (default 255)")]
        public async Task DrawToFrameBuffer(int x, int y, int r, int g, int b, int a = 255)
        {
            System.Drawing.Color color = newClampedColor(a, r, g, b);
            (x, y) = clampPointToFrameBuffer(x, y);
            frameBuffer.Draw(x, y, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a px {color.ToString()} at x={x} y={y}.");
        }

        [Command("DrawLineToFB")]
        [Summary("Draws a line from x1, y1 to the x2, y2 coordinates, of color r, g, b, a (default 255)")]
        public async Task DrawLineToFrameBuffer(int x1, int y1, int x2, int y2, int r, int g, int b, int a = 255)
        {
            System.Drawing.Color color = newClampedColor(a, r, g, b);
            (x1, y1) = clampPointToFrameBuffer(x1, y1);
            (x2, y2) = clampPointToFrameBuffer(x2, y2);
            frameBuffer.DrawLine(x1, y1, x2, y2, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a line of {color.ToString()} from x1={x1} y1={y1} to x2={x2} y2={y2}.");
        }

        [Command("DrawRectToFB")]
        [Summary("Draws a rectangle from x, y coordinates of sizeX, sizeY Size, and of color r, g, b, a (default 255)")]
        public async Task DrawRectToFrameBuffer(int x, int y, int sizeX, int sizeY, int r, int g, int b, int a = 255)
        {
            System.Drawing.Color color = newClampedColor(a, r, g, b);
            (x, y) = clampPointToFrameBuffer(x, y);
            (sizeX, sizeY) = clampPointToFrameBuffer(sizeX - x, sizeY - y);
            frameBuffer.DrawRect(x, y, sizeX, sizeY, color);
            await ReplyAsync($"{LogManager.DGE_LOG}Succesfuly drew a rectangle of {color.ToString()} from x={x} y={y} to x={x + sizeX} y={y + sizeY}.");

        }

        [Command("setFBPixelDrawMode")]
        [Summary("Sets the drawing mode REPLACE (Replaces the pixels when drawing), ALPHA_BLENDING (Blends every pixels using alpha value), or NORMAL (Like replace but alpha is ignored)")]
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
                    throw new CommandExecutionException($"Cannot change pixel draw mode to {mode}");
            }
            await ReplyAsync("{LogManager.DGE_LOG}Succesfuly set the Frame Buffer PixelRenderMode to {mode.ToUpper()}.");
        }
        
        [Command("ClearImageBuffersDir")]
        [Summary("Removes every stored ImageBuffers in the ImageBuffers directory")]
        public async Task ClearImageBuffersDir()
        {
            FrameBuffer.ClearStoredImageBuffers();
            await ReplyAsync(LogManager.DGE_LOG + "Succesfully Cleared the Image Frame Buffers Dir.");
        }
    }
}
