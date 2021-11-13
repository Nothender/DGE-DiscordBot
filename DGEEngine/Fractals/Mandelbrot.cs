using System;
using System.Collections.Generic;
using System.Text;
using DGE.Rendering;
using System.Drawing;
using DGE.Core;
using System.Numerics;
using System.Linq;

namespace DGE.Fractals
{
    public class Mandelbrot : IFractal
    {

        private readonly float[,] mask;

        public Mandelbrot(int size = 1000, int maxIterations = 256)
        {
            size = size > 0 ? size : 1;

            int sizex = (int) (1.5f * size);
            mask = new float[sizex, size];

            float logMaxIterations = ((float)Math.Log2(maxIterations+1));
            float[] alphas = new float[maxIterations];
            for (int i = 0; i < maxIterations; i++)
            {
                alphas[i] = ((float) Math.Log2(i + 1)) / logMaxIterations;
            }

            alphas[maxIterations - 1] = 0f;

            maxIterations -= 1;

            float xv = 3f / sizex;
            float yv = 2f / size; 

            float x = -2f;
            float y;

            for (int xp = 0; xp < sizex; xp++)
            {
                x += xv;
                y = -1f;
                for (int yp = 0; yp < size; yp++)
                {
                    Complex c = new Complex(x, y);
                    Complex z = new Complex(0f, 0f);
                    y += yv;
                    int i = -1;
                    while (z.Magnitude < 2f && i < maxIterations)
                    {
                        z = z * z + c;
                        i++;
                    }
                    mask[xp, yp] = alphas[i];
                }
            }

        }

        public void Render(IPixelBuffer surface, params Color[] colors)
        {
            //If not enough colors to create a gradient
            if (colors.Length == 1)
                colors = new Color[1] { Color.FromArgb(255, 0, 0, 0) }.Concat(colors).ToArray();
            if (colors.Length == 0)
                colors = new Color[2] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 255, 255, 255) };

            //TODO: move gradient generation somewhere else (maybe a utils file), and create a LogGradient as it works better for most fractals
            //TODO: Create a Fast Log method either here (math utils file) or in EE | Also could do a GPU accelerated Gradient generator

            int[] tsteps = new int[colors.Length-1];

            for (int i = 0; i < colors.Length - 1; i += 1)
            {
                Color c1 = colors[i], c2 = colors[i + 1];
                tsteps[i] = Math.Max(Math.Max(Math.Abs(c1.R - c2.R), Math.Abs(c1.G - c2.G)), Math.Abs(c1.B - c2.B));
            }

            //Generate gradient for Colors
            Color[] gradient = new Color[tsteps.Sum()];
            int p = 0;
            for (int i = 0; i < colors.Length - 1; i += 1)
            {
                Color c1 = colors[i];
                Color c2 = colors[i + 1];
                 
                int steps = tsteps[i]; //Gathering the largest number of steps possible (for example from 13, 12, 11 to 134, 42, 252, the maximum number of steps is 252 - 11 = 241  
                steps = steps > 2 ? steps : 2;

                for (int s = p; s < p + steps; s++)
                {
                    float v = (s + 1f) / steps;
                    gradient[s] = Color.FromArgb(
                        255,
                        Math.Min(c1.R + (int) ((c2.R - c1.R) * v), 255),
                        Math.Min(c1.G + (int) ((c2.G - c1.G) * v), 255),
                        Math.Min(c1.B + (int) ((c2.B - c1.B) * v), 255));
                }

            }

            gradient[0] = Color.FromArgb(255, 0, 0, 0);

            //ratio
            float xr = mask.GetLength(0) / (float)surface.size.Width;
            float yr = mask.GetLength(1) / (float)surface.size.Height;

            int gradientColorsCount = gradient.Length - 1;

            //Drawing on the surface using the mask
            for (int x = 0; x < surface.size.Width; x++)
            {
                for (int y = 0; y < surface.size.Height; y++)
                {
                    surface.Draw(x, y, gradient[(int) (mask[(int)(x * xr), (int)(y * yr)] * gradientColorsCount)]);    
                }
            }

        }

    }
}
