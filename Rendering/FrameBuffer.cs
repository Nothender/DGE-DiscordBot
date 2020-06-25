using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace DiscordGameEngine.Rendering
{
    public class FrameBuffer : PixelBuffer
    {
        private static string imageBuffersPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\imageBuffers\\";
        private static System.Drawing.Imaging.ImageFormat imageBufferSaveFormat = System.Drawing.Imaging.ImageFormat.Png;
        private static Random randomGen = new Random();
        private static List<string> imageBufferIDs = new List<string>();

        public event EventHandler OnResize;
        private string imageBufferID;

        public FrameBuffer(Size size, Color clearColor) : base(size, clearColor)
        {
            CheckForImageBuffersDir();
            this.imageBufferID = GetNewImageBufferID();
            Render();
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

        private void SaveBufferToDisk()
        {
            buffer.Save(imageBuffersPath + imageBufferID, imageBufferSaveFormat);
        }

        private void CreateBuffer()
        {
            buffer = new Bitmap(size.Width, size.Height);
        }

        public override void SetSize(Size newSize)
        {
            base.SetSize(newSize);
            OnResize?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Call to free RAM and Disk space if this FrameBuffer is no longer used -> using it after calling this method might create bugs
        /// </summary>
        public override void Dispose()
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
                res = "imageBuffer" + randomGen.Next(1000000, 10000000 - 1) + '.' + imageBufferSaveFormat.ToString().ToLower();
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