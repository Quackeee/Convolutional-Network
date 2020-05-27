using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    public class MaxPoolLayer : HiddenLayer
    {
        private int _stride;

        public MaxPoolLayer(int stride)
        {
            _stride = stride;
        }

        public override void CalculateDeltas(Matrix3D previousDeltas)
        {
            Console.WriteLine("Calculating deltas in MaxPoolLayer");
            throw new NotImplementedException();
        }

        public override void CalculateOutput()
        {
            var output = new Matrix[_input.Depth];

            for (int i = 0; i < _input.Depth; i++)
            {
                output[i] = MaxPool(_input[i], _stride);
            }

            _output = new Matrix3D(output);
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;
            _inputLayer.ConnectOutput(this);

            OutputDepth = _inputLayer.OutputDepth;
            OutputHeight = _inputLayer.OutputHeight / _stride;
            OutputWidth = _inputLayer.OutputWidth / _stride;
        }
    }
}
