using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    class Synapse
    {
        NetworkLayer _inputLayer;
        double _input { get => _inputLayer.Output[_k, _i, _j]; }

        public double Weight = Rand.NextDouble();
        public double Output { get => _input * Weight; }

        int _k;
        int _i;
        int _j;

        public Synapse(NetworkLayer inputLayer, int k, int i, int j)
        {
            _inputLayer = inputLayer;
            _k = k;
            _i = i;
            _j = j;
        }
    }
}
