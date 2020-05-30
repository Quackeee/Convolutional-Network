using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    class Network
    {
        private InputLayer _input;
        private HiddenLayer[] _layers;

        public Matrix3D Output { get => _layers.Last().Output; }

        public Network(InputLayer inputLayer, params HiddenLayer[] layers)
        {
            _input = inputLayer;
            _layers = layers;

            _layers[0].ConnectToInput(inputLayer);

            for (int i = 1; i < _layers.Length; i++)
            {
                _layers[i].ConnectToInput(_layers[i - 1]);
            }
        }

        public void FeedForward(Bitmap bm)
        {
            _input.PushInput(bm);

            foreach (var layer in _layers)
                layer.CalculateOutput();
        }

        public void BackPropagate(Matrix3D expectedOutput)
        {
            var deltas = new Matrix3D(expectedOutput.Dimensions);

            for (int k = 0; k < Output.Depth; k++)
                for (int i = 0; i < Output.Height; i++)
                    for (int j = 0; j < Output.Width; j++)
                        deltas[k, i, j] = -expectedOutput[k, i, j] / Output[k, i, j];

            Console.WriteLine(deltas);

            _layers.Last().PropagateDeltas(deltas);

            foreach (var layer in _layers)
                if (layer is ITrainableLayer)
                    (layer as ITrainableLayer).UpdateWeights();
        }

    }
}
