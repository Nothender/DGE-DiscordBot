using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DiscordGameEngine.Rendering
{

    public enum PixelDrawMode
    {
        ALPHA_BLENDING = 0,
        NORMAL = 1,
        REPLACE = 2
    }

    public enum ImageScalingMethod 
    {
        CLEAR=0, //Clears the buffer when resizing
        NEAREST=1 //Nearest neighbor per pixel scaling method
    }

    public static class RenderingCore
    {

        public static Color AlphaBlend(Color c1, Color c2)
        {
            float a1 = c1.A / 255f;
            float a2 = c2.A / 255f;
            c1 = Color.FromArgb((int)(c1.R * a1), (int)(c1.G * a1), (int)(c1.B * a1));
            c2 = Color.FromArgb((int)(c2.R * a2), (int)(c2.G * a2), (int)(c2.B * a2));
            return Color.FromArgb(
                Math.Min(255, (int)(c1.A + (c2.A * (1 - a1)))), 
                Math.Min(255, (int)(c1.R + (c2.R * (1 - a1)))), 
                Math.Min(255, (int)(c1.G + (c2.G * (1 - a1)))), 
                Math.Min(255, (int)(c1.B + (c2.B * (1 - a1))))
            );
        }

        public static Color ToOpaqueColor(Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceBuffer"></param>
        /// <param name="targetBuffer">/!\ Warning the target buffer must not be the same as the source buffer</param>
        /// <param name="scalingMethod"></param>
        public static void ResizeBuffer(Bitmap sourceBuffer, Bitmap targetBuffer, ImageScalingMethod scalingMethod=ImageScalingMethod.NEAREST)
        {
            if (scalingMethod == ImageScalingMethod.CLEAR)
            {
                Color color = Color.FromArgb(0, 0, 0);
                for (int x = 0; x < targetBuffer.Size.Width; x++)
                {
                    for (int y = 0; y < targetBuffer.Size.Height; y++)
                    {
                        targetBuffer.SetPixel(x, y, color);
                    }
                }
            }
            else if (scalingMethod == ImageScalingMethod.NEAREST)
            {
                float xR = sourceBuffer.Size.Width / (float)targetBuffer.Size.Width;
                float yR = sourceBuffer.Size.Height / (float)targetBuffer.Size.Height;
                for (int x = 0; x < targetBuffer.Size.Width; x++)
                {
                    for (int y = 0; y < targetBuffer.Size.Height; y++)
                    {
                        targetBuffer.SetPixel(x, y, sourceBuffer.GetPixel((int) (x * xR), (int)(y * yR)));
                    }
                }
            }
        }

    }
}
