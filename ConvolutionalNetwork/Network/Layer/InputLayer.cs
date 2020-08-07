using ConvolutionalNetwork.Misc;
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

        public void PushInput(IMatrix3D bm)
        {
            if (bm.Height != OutputHeight || bm.Width != OutputWidth) throw new ArgumentException("Bitmap is not of proper size");

            _output = bm;
        }

        public void PushInput(Matrix3D m)
        {
            if (m.Height != OutputHeight || m.Width != OutputWidth) throw new ArgumentException("Bitmap is not of proper size");
            _output = m;
        }

        public InputLayer(int width, int height, bool rgb = true)
        {
            _rgb = rgb;
            OutputHeight = height;
            OutputWidth = width;

            if (rgb) OutputDepth = 3;
            else OutputDepth = 1;

            //Console.WriteLine($"{OutputDepth}x{OutputHeight}x{OutputWidth}");
        }

        public InputLayer(int length)
        {
            OutputHeight = length;
            OutputWidth = 1;
            OutputDepth = 1;
        }
    }
}
