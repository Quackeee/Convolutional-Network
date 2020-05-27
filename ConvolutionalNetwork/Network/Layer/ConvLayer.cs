using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class ConvLayer : HiddenLayer
    {
        private ConvNeuron[] _neurons;
        private int _kernelSize;
        private ActivationFunc _activation;

        public ConvLayer(int outputCount, ActivationFunc activation = null, int kernelSize = 5)
        {
            if (activation == null) activation = ActivationFuncs.ReLU;

            OutputDepth = outputCount;

            _kernelSize = kernelSize;
            _activation = activation;

            _neurons = new ConvNeuron[outputCount];


            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new ConvNeuron(kernelSize);
            }
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputHeight = _inputLayer.OutputHeight - _kernelSize + 1;
            OutputWidth = _inputLayer.OutputWidth - _kernelSize + 1;

            foreach (var n in _neurons) n.ConnectToInput(_inputLayer);
        }

        public override void CalculateOutput()
        {
            if (IsConnected)
            {
                var output = new Matrix[OutputDepth];

                for (int i = 0; i < OutputDepth; i++)
                {
                    _neurons[i].CalculateOutput();
                    output[i] = _neurons[i].Output;
                }
                _output = new Matrix3D(output);
                _activation.Run(_output);
            }
            else throw new InvalidOperationException("The Layer is not connected to any input");
        }

        public override void CalculateDeltas(Matrix3D previousDeltas)
        {
            Console.WriteLine("Calculating deltas in ConvLayer");
            throw new NotImplementedException();
        }
    }
}
