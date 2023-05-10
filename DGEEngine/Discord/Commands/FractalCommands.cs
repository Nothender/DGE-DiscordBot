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
using System.Threading;

namespace DGE.Discord.Commands
{//These commands will be removed soon (FrameBuffers and FB interactions overhaul/remake)
    [Summary("Global FrameBuffer instance manipulation commands (Deprecated)")]
    public class FractalCommands : DGEModuleBase
    {

        private static readonly DiscordFrameBuffer displaySurface = new DiscordFrameBuffer(new Size((int) (420 * 1.5f), 420), System.Drawing.Color.FromArgb(255, 0, 0, 0));
        private static readonly Mandelbrot mandelbrot = new Mandelbrot();

        private static bool IsMandelbrotBeingRenderedAndDisplayed = false;

        [Command("Mandelbrot", RunMode = RunMode.Async)]
        [Summary("Renders and displays mandelbrot with a gradient")]
        [Remarks("RGBs are the ints paired by 3 to create n colors used to generate the gradient")]
        public async Task CommandRenderMandebrot(params int[] RGBs)
        {
            if (IsMandelbrotBeingRenderedAndDisplayed)
            {
                await RespondAsync("Mandelbrot is already being rendered, please wait", ephemeral: true);
                return;
            }
            IsMandelbrotBeingRenderedAndDisplayed = true;
            if (RGBs.Length == 0)
                RGBs = new int[3] { 255, 255, 255 };
            if (RGBs.Length % 3 != 0)
                throw new ArgumentException("Colors need to be defined using 3 values");

            System.Drawing.Color[] colors = new System.Drawing.Color[RGBs.Length / 3];
            for (int i = 0; i < RGBs.Length; i += 3)
                colors[i / 3] = System.Drawing.Color.FromArgb(255, Math.Clamp(RGBs[i], 0, 255), Math.Clamp(RGBs[i + 1], 0, 255), Math.Clamp(RGBs[i + 2], 0, 255));

            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                mandelbrot.Render(displaySurface, colors.ToArray());
                watch.Stop();
                displaySurface.Render();
                displaySurface.Display(Context.Channel, $"Render took {watch.ElapsedMilliseconds}ms");
            }
            catch
            {
                await RespondAsync("Mandelbrot had an unexpected exception, maybe try again", ephemeral: true); //The user has to slow down, (probably a GDI+ exception caused due to not being able to save on disk)
            }
            finally
            {
                IsMandelbrotBeingRenderedAndDisplayed = false;
            }

        }
        
    }
}
