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

    }
}
