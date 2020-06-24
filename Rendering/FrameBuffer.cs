using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace DiscordGameEngine.Rendering
{

    public enum PixelRenderMode
    { 
        ALPHA_BLENDING=0,
        NORMAL=1,
        REPLACE=2
    }

    public class FrameBuffer
    {
        private static string imageBuffersPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\imageBuffers\\";
        private static System.Drawing.Imaging.ImageFormat imageBufferSaveFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
        private static Random randomGen = new Random();
        private static List<string> imageBufferIDs = new List<string>();

        public Size size { get { return size; } }
        private Size _size;
        public Color clearColor;
        public Bitmap buffer;
        public PixelRenderMode pixelRenderMode;

        public event EventHandler OnResize;

        private string imageBufferID;

        public FrameBuffer(Size size, Color clearColor)
        {
            this._size = size;
            this.clearColor = clearColor;
            this.pixelRenderMode = PixelRenderMode.NORMAL;
            CheckForImageBuffersDir();
            this.imageBufferID = GetNewImageBufferID();
            CreateBuffer();
            Clear();
            Render();
        }

        private Color AlphaBlend(Color c1, Color c2)
        {
            float a1 = c1.A / 255;
            float a2 = c2.A / 255;
            c1 = Color.FromArgb((int)(c1.R * a1), (int)(c1.G * a1), (int)(c1.B * a1));
            c2 = Color.FromArgb((int)(c2.R * a2), (int)(c2.G * a2), (int)(c2.B * a2));
            return Color.FromArgb(Math.Min(255, (int) ((c1.A + c2.A) * (1 - a1))), Math.Min(255, (int) ((c1.R + c2.R) * (1 - a1))), Math.Min(255, (int) ((c1.G + c2.G) * (1 - a1))), Math.Min(255, (int) ((c1.B + c2.B) * (1 - a1))));
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
                color = AlphaBlend(color, buffer.GetPixel(x, y));
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
                        buffer.SetPixel(i, j, AlphaBlend(color, buffer.GetPixel(i, j)));
                    }
                }
            }
            else
            {
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

        /// <summary>
        /// Renders the buffer from memory to disk to be able to display it via a discord message
        /// </summary>
        public void Render()
        {
            SaveBufferToDisk();
        }

        public void Display(SocketCommandContext context)
        {
            context.Channel.SendFileAsync(imageBuffersPath + imageBufferID);
        }

        public void SetSize(Size newSize)
        {
            _size = newSize;
            ResizeBuffer();
            OnResize?.Invoke(this, new EventArgs());
        }

        private void SaveBufferToDisk()
        {
            buffer.Save(imageBuffersPath + imageBufferID, System.Drawing.Imaging.ImageFormat.Jpeg);
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
        /// Call to free RAM and Disk space if this FrameBuffer is no longer used -> using it after calling this method might create bugs
        /// </summary>
        public void Dispose()
        {
            buffer.Dispose();
            ClearStoredImageBuffer();
            if (imageBufferIDs.Contains(imageBufferID))
                imageBufferIDs.Remove(imageBufferID);
        }

        public void ClearStoredImageBuffer()
        {
            string path = imageBuffersPath + imageBufferID;
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetNewImageBufferID()
        {
            string res;
            do
            {
                res = "imageBuffer" + randomGen.Next(1000000, 10000000) + '.' + imageBufferSaveFormat.ToString().ToLower();
            } while (imageBufferIDs.Contains(res));
            imageBufferIDs.Add(res);
            return res;
        }

        private static void CheckForImageBuffersDir()
        {
            if (Directory.Exists(imageBuffersPath))
                return;
            Directory.CreateDirectory(imageBuffersPath);
        }

        public static void ClearStoredImageBuffers()
        {
            Directory.Delete(imageBuffersPath, true);
            CheckForImageBuffersDir();
        }

    }
}