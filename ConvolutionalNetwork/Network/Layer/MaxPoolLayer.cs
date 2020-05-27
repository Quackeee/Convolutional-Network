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

        public override double GetWeightBetween(int k1, int i1, int j1, int k2, int i2, int j2)
        {
            if (i2 / _stride != i1 || j2 / _stride != j2) throw new InvalidOperationException();
            if (Output[k1, i1, j1] == _input[k2, i2, j2]) return 1; else return 0;
        }
    }
}
