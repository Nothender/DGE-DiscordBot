using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DGE;
using DGE.Core;

using Color = System.Drawing.Color;
using Discord;

namespace DGE.Rendering
{
    public class DiscordFrameBuffer : PixelBuffer, IFrameBuffer
    {
        private static readonly List<string> imageBufferIDs = new List<string>();
        private static readonly System.Drawing.Imaging.ImageFormat imageBufferSaveFormat = System.Drawing.Imaging.ImageFormat.Png;
        private static readonly Random random = new Random();
        private Size _displaySize;

        private bool _scaleOnRender;

        private ImageScalingMethod _scalingMethod;

        //To render the buffer at display size scaleOnRender must be true
        private Bitmap displayBuffer;

        private string imageBufferID;

        public DiscordFrameBuffer(Size size, Color clearColor, bool scaleOnRender = false, Size displaySize = new Size(), ImageScalingMethod scalingMethod = ImageScalingMethod.NEAREST) : base(size, clearColor)
        {
            this.imageBufferID = GetNewImageBufferID();
            this.scalingMethod = scalingMethod;
            SetScaleOnRender(displaySize, scaleOnRender);
            Render();
        }

        public event EventHandler OnResize;

        public Size displaySize { get { return _displaySize; } }

        public bool scaleOnRender { get { return _scaleOnRender; } }

        public ImageScalingMethod scalingMethod //Scaling method used when rendering to a different displaySize than the PixelBuffer's size
        {
            get { return _scalingMethod; }
            set
            {
                if (value == ImageScalingMethod.CLEAR)
                {
                    value = ImageScalingMethod.NEAREST;
                    AssemblyEngine.logger.Log("scalingMethod in FrameBuffer cannot be ImageScalingMethod.CLEAR -> will be set by default to nearest", EnderEngine.Logger.LogLevel.WARN);
                }
                _scalingMethod = value;
            }
        }
        public static void ClearStoredImageBuffers()
        {
            Directory.Delete(Paths.Get("ImageBuffers"), true);
            Paths.CheckForDir(Paths.Get("ImageBuffers"));
        }

        public void ClearStoredImageBuffer()
        {
            string path = Paths.Get("ImageBuffers") + imageBufferID;
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Sends the last rendered frame to the MessageChannel channel
        /// </summary>
        /// <param name="channel"></param>
        public void Display(IMessageChannel channel, string message = null)
        {
            channel.SendFileAsync(Paths.Get("ImageBuffers") + imageBufferID, message);
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

        /// <summary>
        /// Renders the buffer from memory to disk to be able to display it via a discord message
        /// </summary>
        public void Render()
        {
            if (scaleOnRender)
                RenderToDisplayBuffer();
            SaveBufferToDisk();
        }

        public override void Resize(Size newSize, ImageScalingMethod scalingMethod = ImageScalingMethod.CLEAR)
        {
            base.Resize(newSize, scalingMethod);
            OnResize?.Invoke(this, new EventArgs());
        }

        public void SetScaleOnRender(Size displaySize, bool scaleOnRender)
        {
            this._scaleOnRender = scaleOnRender;
            if (displaySize.Width == 0)
                displaySize.Width = 1;
            if (displaySize.Height == 0)
                displaySize.Height = 1;
            this._displaySize = displaySize;
            if (displayBuffer != null)
                displayBuffer.Dispose();
            this.displayBuffer = new Bitmap(displaySize.Width, displaySize.Height);
        }

        private static string GetNewImageBufferID()
        {
            string res = "imageBuffer" + DateTime.Now.Ticks.ToString() + '.' + imageBufferSaveFormat.ToString().ToLower();
            imageBufferIDs.Add(res);
            return res;
        }

        private void RenderToDisplayBuffer() //Renders the buffer onto the displayBuffer at displaySize
        {
            RenderingCore.ResizeBuffer(buffer, displayBuffer, scalingMethod);
        }

        private void SaveBufferToDisk()
        {
            if (scaleOnRender)
                displayBuffer.Save(Paths.Get("ImageBuffers") + imageBufferID, imageBufferSaveFormat);
            else
                buffer.Save(Paths.Get("ImageBuffers") + imageBufferID, imageBufferSaveFormat);
        }
    }
}