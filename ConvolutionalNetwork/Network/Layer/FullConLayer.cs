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

        public FullConLayer(int outputCount, ActivationFunc activation)
        {
            OutputDepth = 1;
            OutputWidth = 1;
            OutputHeight = outputCount;

            _neurons = new Neuron[outputCount];
            _output = new Matrix3D(1, outputCount, 1);
            _activation = activation;

            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new Neuron();
            }
        }

        public override void LoadAndPropagateDeltas(Matrix3D previousDeltas)
        {
            Console.WriteLine("Calculating deltas in FullConLayer");

            Deltas = _activation.RecalculateDeltas(previousDeltas, _output);

            if (_inputLayer is HiddenLayer)
            {

                var deltas = new Matrix3D(InputDepth, InputHeight, InputWidth);

                for (int k = 0; k < InputDepth; k++)
                {
                    for (int i = 0; i < InputHeight; i++)
                    {
                        for (int j = 0; j < InputWidth; j++)
                        {
                            deltas[k, i, j] = 0;
                            for (int n = 0; n < OutputHeight; n++)
                            {
                                deltas[k, i, j] += Deltas[0, n, 0] * _neurons[n].Weights[k, i, j];
                            }
                        }
                    }
                }

                Console.WriteLine(deltas);
                (_inputLayer as HiddenLayer).LoadAndPropagateDeltas(deltas);
            }
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

            foreach (var neuron in _neurons)
                neuron.ConnectToInput(_inputLayer);
        }
    }
}
