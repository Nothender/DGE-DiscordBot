using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace DiscordGameEngine.Rendering
{

    public class PixelBuffer : IPixelBuffer
    {

        public Size size { get { return _size; } }
        private Size _size;
        public Color clearColor;
        public Bitmap buffer;
        public PixelDrawMode pixelDrawMode;

        public PixelBuffer(Size size, Color color)
        {
            _size = size;
            Construct();
            this.clearColor = color;
            Clear();
        }
        public PixelBuffer(string pathToImage)
        {
            Construct(pathToImage);
            _size = buffer.Size;
            this.clearColor = Color.FromArgb(0, 0, 0);
        }
        public PixelBuffer(string pathToImage, Color clearColor)
        {
            Construct(pathToImage);
            _size = buffer.Size;
            this.clearColor = clearColor;
        }

        private void Construct(string pathToImage = null)
        {
            CreateBuffer(pathToImage);
            pixelDrawMode = PixelDrawMode.NORMAL;
        }

        public void Clear()
        {
            for (int x = 0; x < _size.Width; x++)
            {
                for (int y = 0; y < _size.Height; y++)
                {
                    buffer.SetPixel(x, y, clearColor);
                }
            }
        }

        public void Draw(int x, int y, Color color)
        {
            if (pixelDrawMode == PixelDrawMode.ALPHA_BLENDING)
                color = RenderingCore.AlphaBlend(color, buffer.GetPixel(x, y));
            else if (pixelDrawMode == PixelDrawMode.NORMAL)
                color = RenderingCore.ToOpaqueColor(color);
            buffer.SetPixel(x, y, color);
        }

        public void DrawRect(int x, int y, int sizeX, int sizeY, Color color)
        {
            int x1 = x + sizeX;
            int y1 = y + sizeY;
            if (pixelDrawMode == PixelDrawMode.ALPHA_BLENDING)
            {
                for (int i = x; i < x1; i++)
                {
                    for (int j = y; j < y1; j++)
                    {
                        buffer.SetPixel(i, j, RenderingCore.AlphaBlend(color, buffer.GetPixel(i, j)));
                    }
                }
            }
            else
            {
                if (pixelDrawMode == PixelDrawMode.NORMAL)
                    color = RenderingCore.ToOpaqueColor(color);
                for (int i = x; i < x1; i++)
                {
                    for (int j = y; j < y1; j++)
                    {
                        buffer.SetPixel(i, j, color);
                    }
                }
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color, int width = 1)
        {
            //If the line is Horizontal
            if (y1 == y2)
            {
                int diff = Math.Abs(x1 - x2);
                if (pixelDrawMode == PixelDrawMode.ALPHA_BLENDING)
                {
                    for (int i = Math.Min(x1, x2); i < diff; i++)
                    {
                        buffer.SetPixel(i, y1, RenderingCore.AlphaBlend(color, buffer.GetPixel(i, y1)));
                    }
                }
                else
                {
                    if (pixelDrawMode == PixelDrawMode.NORMAL)
                        color = RenderingCore.ToOpaqueColor(color);
                    for (int i = Math.Min(x1, x2); i < diff; i++)
                    {

                        buffer.SetPixel(i, y1, color);
                    }
                }
            }

            //If the line is Vertical
            else if (x1 == x2)
            {
                int diff = Math.Abs(y1 - y2);
                if (pixelDrawMode == PixelDrawMode.ALPHA_BLENDING)
                {
                    for (int i = Math.Min(y1, y2); i < diff + Math.Min(y1, y2); i++)
                    {
                        buffer.SetPixel(i, x1, RenderingCore.AlphaBlend(color, buffer.GetPixel(i, x1)));
                    }
                }
                else
                {
                    if (pixelDrawMode == PixelDrawMode.NORMAL)
                        color = RenderingCore.ToOpaqueColor(color);
                    for (int i = Math.Min(y1, y2); i < diff; i++)
                    {

                        buffer.SetPixel(i, x1, color);
                    }
                }
            }

            else
            {
                int step; //Copied from wikipedia : https://en.wikipedia.org/wiki/Digital_differential_analyzer_(graphics_algorithm)
                int dx = (x2 - x1);
                int dy = (y2 - y1);
                if (Math.Abs(dx) >= Math.Abs(dy))
                    step = Math.Abs(dx);
                else
                    step = Math.Abs(dy);
                dx = dx / step;
                dy = dy / step;
                int x = x1;
                int y = y1;
                int i = 1;
                while (i <= step)
                {
                    buffer.SetPixel(x, y, color);
                    x = x + dx;
                    y = y + dy;
                    i = i + 1;
                }
            }
        }

        public virtual void Resize(Size newSize, ImageScalingMethod scalingMethod = ImageScalingMethod.CLEAR)
        {
            _size = newSize;
            ResizeBuffer(scalingMethod);
        }

        private void CreateBuffer(string filePath = null)
        {
            if (filePath == null)
                buffer = new Bitmap(_size.Width, _size.Height);
            else
            {
                buffer = new Bitmap(filePath);
            }
        }

        private void ResizeBuffer(ImageScalingMethod scalingMethod = ImageScalingMethod.CLEAR)
        {
            Bitmap targetBuffer = new Bitmap(size.Width, size.Height);
            RenderingCore.ResizeBuffer(buffer, targetBuffer, scalingMethod);
            buffer.Dispose();
            buffer = targetBuffer;
        }

        /// <summary>
        /// Call to free RAM and Disk space if this PixelBuffer is no longer used -> using it after calling this method might create bugs
        /// </summary>
        public virtual void Dispose()
        {
            buffer.Dispose();
        }

    }
}