using Discord.WebSocket;
using System;
using System.Drawing;

namespace DiscordGameEngine.Rendering
{
    public interface IFrameBuffer : IPixelBuffer
    {
        Size displaySize { get; }
        bool scaleOnRender { get; }
        ImageScalingMethod scalingMethod { get; set; }

        event EventHandler OnResize;

        void ClearStoredImageBuffer();
        void Display(ISocketMessageChannel channel);
        void Render();
        void SetScaleOnRender(Size displaySize, bool scaleOnRender);
    }
}