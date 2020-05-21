using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    class ConvNeuron
    {
        private NetworkLayer _inputLayer;
        private Matrix _output;
        private Matrix3D _kernel;
        private int _kernelSize;

        public bool IsConnected => _inputLayer != null;

        public Matrix Output { get => _output; }

        private Matrix3D _input { get => _inputLayer.Output; }

        public void CalculateOutput()
        {
            if (IsConnected)
                _output = ConvolveWhole(_input, _kernel);
            else throw new InvalidOperationException("The Neuron was not connected");
        }

        public ConvNeuron(int kernelSize)
        {
            _kernelSize = kernelSize;
        }

        public void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            Debug.WriteLine(inputLayer.OutputDepth);
            Debug.WriteLine(_kernel);

            _kernel = new Matrix3D(inputLayer.OutputDepth, _kernelSize, _kernelSize);
            _kernel.RandomInit();
        }
    }
}
