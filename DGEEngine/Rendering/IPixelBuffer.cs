using System.Drawing;

namespace DGE.Rendering
{
    public interface IPixelBuffer
    {
        Size size { get; }

        void Clear();
        void Dispose();
        void Draw(int x, int y, Color color);
        void DrawLine(int x1, int y1, int x2, int y2, Color color, int width = 1);
        void DrawRect(int x, int y, int sizeX, int sizeY, Color color);
        void Resize(Size newSize, ImageScalingMethod scalingMethod = ImageScalingMethod.CLEAR);
    }
}