using DGE.Rendering;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using DGE.Fractals;
using DGE.Exceptions;
using Discord;

namespace DGE.Discord.Commands
{//These commands will be removed soon (FrameBuffers and FB interactions overhaul/remake)
    [Summary("Global FrameBuffer instance manipulation commands (Deprecated)")]
    public class FractalCommands : DGEModuleBase
    {

        private static readonly DiscordFrameBuffer displaySurface = new DiscordFrameBuffer(new Size((int) (420 * 1.5f), 420), System.Drawing.Color.FromArgb(255, 0, 0, 0));
        private static readonly Mandelbrot mandelbrot = new Mandelbrot();

        private static bool IsMandelbrotBeingRenderedAndDisplayed = false;

        [Command("Mandelbrot")]
        [Summary("Renders and displays mandelbrot with a gradient")]
        [Remarks("RGBs are the ints paired by 3 to create n colors used to generate the gradient")]
        public async Task CommandRenderMandebrot(params int[] RGBs)
        {
            if (IsMandelbrotBeingRenderedAndDisplayed)
            {
                await ReactAsync(new Emoji("╳"));
                return;
            }
            IsMandelbrotBeingRenderedAndDisplayed = true;
            if (RGBs.Length == 0)
                RGBs = new int[3] { 0, 0, 0 };
            if (RGBs.Length % 3 != 0)
                throw new CommandExecutionException(new ArgumentException("Colors need to be defined using 3 values"));

            System.Drawing.Color[] colors = new System.Drawing.Color[RGBs.Length / 3];
            for (int i = 0; i < RGBs.Length; i += 3)
                colors[i / 3] = System.Drawing.Color.FromArgb(255, RGBs[i], RGBs[i + 1], RGBs[i + 2]);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            mandelbrot.Render(displaySurface, colors.ToArray());
            watch.Stop();
            displaySurface.Render();
            displaySurface.Display(Context.Channel, $"Render took {watch.ElapsedMilliseconds}ms");
            IsMandelbrotBeingRenderedAndDisplayed = false;
        }
        
    }
}
