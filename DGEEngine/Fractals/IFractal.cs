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
        /// Gets a render of the fractal using specific parameters as an IPixelBuffer
        /// </summary>
        /// <returns>The IPixelBuffer</returns>
        public void Render(IPixelBuffer surface, params Color[] colors);

    }
}
