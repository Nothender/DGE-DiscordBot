using Discord;
using Discord.WebSocket;
using System;
using System.Drawing;

namespace DGE.Rendering
{
    public interface IFrameBuffer : IPixelBuffer
    {
        Size displaySize { get; }
        bool scaleOnRender { get; }
        ImageScalingMethod scalingMethod { get; set; }

        event EventHandler OnResize;

        void ClearStoredImageBuffer();
        void Display(IMessageChannel channel, string message = null);
        void Render();
        void SetScaleOnRender(Size displaySize, bool scaleOnRender);
    }
}