using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class InputLayer : NetworkLayer
    {
        private bool _rgb;

        public void PushInput(Bitmap bm)
        {
            if (bm.Height != OutputHeight || bm.Width != OutputWidth) throw new ArgumentException("Bitmap is not of proper size");

            if (_rgb) _output = bm.AsMatrixRGB();
            else _output = bm.AsMatrixGrayscale();
        }

        public InputLayer(int height, int width, bool rgb = true)
        {
            _rgb = rgb;
            OutputHeight = height;
            OutputWidth = width;

            if (rgb) OutputDepth = 3;
            else OutputDepth = 1;
        }

        public InputLayer(int length)
        {
            OutputHeight = length;
            OutputWidth = 1;
            OutputDepth = 1;
        }
    }
}
