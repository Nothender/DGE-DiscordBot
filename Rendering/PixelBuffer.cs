using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace DiscordGameEngine.Rendering
{

    public class PixelBuffer
    {

        public Size size { get { return size; } }
        private Size _size;
        public Color clearColor;
        public Bitmap buffer;
        public PixelRenderMode pixelRenderMode;

        public PixelBuffer(Size size, Color clearColor)
        {
            _size = size;
            this.clearColor = clearColor;
            pixelRenderMode = PixelRenderMode.NORMAL;
            CreateBuffer();
            Clear();
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
            if (pixelRenderMode == PixelRenderMode.ALPHA_BLENDING)
                color = RenderingCore.AlphaBlend(color, buffer.GetPixel(x, y));
            else if (pixelRenderMode == PixelRenderMode.NORMAL)
                color = RenderingCore.ToOpaqueColor(color);
            buffer.SetPixel(x, y, color);
        }

        public void DrawRect(int x, int y, int sizeX, int sizeY, Color color)
        {
            int x1 = x + sizeX;
            int y1 = y + sizeY;
            if (pixelRenderMode == PixelRenderMode.ALPHA_BLENDING)
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
                if (pixelRenderMode == PixelRenderMode.NORMAL)
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

        public void DrawLine()
        {
        }

        public virtual void SetSize(Size newSize)
        {
            _size = newSize;
            ResizeBuffer();
        }

        private void CreateBuffer()
        {
            buffer = new Bitmap(_size.Width, _size.Height);
        }

        private void ResizeBuffer()
        {
            buffer = new Bitmap(buffer, size);
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