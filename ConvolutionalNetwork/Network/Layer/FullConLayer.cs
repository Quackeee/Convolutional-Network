using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class FullConLayer : HiddenLayer
    {
        Neuron[] _neurons;
        private ActivationFunc _activation;
        new int OutputDepth = 1;
        new int OutputWidth = 1;

        public FullConLayer(int outputCount, ActivationFunc activation)
        {
            OutputHeight = outputCount;

            _neurons = new Neuron[outputCount];
            _output = new Matrix3D(1, outputCount, 1);
            _activation = activation;

            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new Neuron();
            }
        }

        public override void CalculateDeltas(Matrix3D previousDeltas)
        {
            /*var deltas = new Matrix3D(InputDepth, InputHeight, InputWidth);

            
            for (int k = 0; k < InputDepth; k++)
            {
                for (int i = 0; i < InputHeight; i++)
                {
                    for (int j = 0; j < InputWidth; j++)
                    {
                        deltas[k, i, j] = 0;
                        for (int n = 0; n < OutputHeight; n++)
                        {
                            deltas[k, i, j] += previousDeltas[0, n, 0] * _neurons[n].Weights[k, i, j];
                        }
                        deltas[k, i, j] *= 
                    }
                }
            }
            
            
            Deltas = deltas;*/
        }

        public override void CalculateOutput()
        {
            for (int i = 0; i < OutputHeight; i++)
            {
                _neurons[i].CalculateOutput();
                _output[0, i, 0] = _neurons[i].Output;
            }
            _activation.Run(_output);
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;
            _inputLayer.ConnectOutput(this);

            foreach (var neuron in _neurons)
                neuron.ConnectToInput(_inputLayer);
        }

        public override double GetWeightBetween(int k1, int i1, int j1, int k2, int i2, int j2)
        {
            return _neurons[i1].Weights[k2, i2, j2];

            //throw new NotImplementedException();
        }
    }
}
