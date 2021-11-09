using DGE.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DGE.Fractals
{
    public interface IFractal
    {
        /// <summary>
        /// Renders the fractal on the given surface
        /// </summary>
        /// <param name="surface">The IPixelBuffer surface the fractal is drawn onto</param>
        /// <param name="colors">The colors used to create a gradient from color 0 to color n</param>
        public void Render(IPixelBuffer surface, params Color[] colors);

    }
}
