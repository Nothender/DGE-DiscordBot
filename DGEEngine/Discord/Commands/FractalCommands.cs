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

namespace DGE.Discord.Commands
{//These commands will be removed soon (FrameBuffers and FB interactions overhaul/remake)
    [Summary("Global FrameBuffer instance manipulation commands (Deprecated)")]
    public class FractalCommands : DGEModuleBase
    {

        private static DiscordFrameBuffer displaySurface = new DiscordFrameBuffer(new Size((int) (420 * 1.5f), 420), Color.FromArgb(255, 0, 0, 0));
        private static Mandelbrot mandelbrot = new Mandelbrot();

        private static (int, int) clampPointToFrameBuffer(int x, int y, IFrameBuffer b)
        {
            return (Math.Abs(x % b.size.Width), Math.Abs(y % b.size.Height)); //new Tuple<int, int>(x, y);
        }

        private static System.Drawing.Color newClampedColor(int a, int r, int g, int b)
        {
            return System.Drawing.Color.FromArgb(Math.Abs(a % 256), Math.Abs(r % 256), Math.Abs(g % 256), Math.Abs(b % 256));
        }

        [Command("Mandelbrot")]
        [Summary("Renders and displays mandelbrot with a gradient from color1 to color2")]
        public async Task CommandRenderMandebrot(params int[] RGBs)
        {
            if (RGBs.Length == 0)
                RGBs = new int[3] { 0, 0, 0 };
            if (RGBs.Length % 3 != 0)
                throw new CommandExecutionException(new ArgumentException("Colors need to be defined using 3 values"));

            List<Color> colors = new List<Color>();
            for (int i = 0; i < RGBs.Length; i += 3)
                colors.Add(Color.FromArgb(255, RGBs[i], RGBs[i + 1], RGBs[i + 2]));

            Stopwatch watch = new Stopwatch();
            watch.Start();
            mandelbrot.Render(displaySurface, colors.ToArray());
            watch.Stop();
            displaySurface.Render();
            displaySurface.Display(Context.Channel, $"Render took {watch.ElapsedMilliseconds}ms");
            await Task.Run(() => { /* do literally nothing but now the compiler is happy */ });
        }
        
    }
}
