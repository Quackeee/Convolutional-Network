using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    interface INeuron
    {
        void CalculateOutput();
        void ConnectToInput(NetworkLayer inputLayer);
        void UpdateWeights(Matrix3D diffs);
    }

    class ConvNeuron : INeuron
    {
        private NetworkLayer _inputLayer;
        private Matrix _output;
        public Matrix3D Weights;
        private int _kernelSize;

        public bool IsConnected => _inputLayer != null;

        public Matrix Output { get => _output; }

        private Matrix3D _input { get => _inputLayer.Output; }

        public void CalculateOutput()
        {
            if (IsConnected)
                _output = ConvolveWhole(_input, Weights);
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
            Debug.WriteLine(Weights);

            Weights = new Matrix3D(inputLayer.OutputDepth, _kernelSize, _kernelSize);
            Weights.RandomInit();
        }

        public void UpdateWeights(Matrix3D diffs)
        {
            Weights += diffs;
        }
    }

    class Neuron : INeuron
    {
        public Matrix3D Weights { get; private set; }
        double _inputSum;
        private NetworkLayer _inputLayer;

        private Func<double, double> _activation = (arg) => arg;

        public double Output { get; private set; }
        public bool IsConnected { get; private set; } = false;

        public void CalculateOutput()
        {
            if (IsConnected)
            {
                double output = 0;

                for (int k = 0; k < Weights.Depth; k++)
                    for (int i = 0; i < Weights.Height; i++)
                        for (int j = 0; j < Weights.Width; j++)
                            output += _inputLayer.Output[k, i, j] * Weights[k, i, j];

                _inputSum = output;
                Output = _activation(output);
            }
            else throw new InvalidOperationException("The Neuron was not connected");
        } 

        public void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;


            //Console.WriteLine($"{inputLayer.OutputDepth} + {inputLayer.OutputHeight} + {inputLayer.OutputWidth}" );
            Weights = new Matrix3D(inputLayer.OutputDepth, inputLayer.OutputHeight, inputLayer.OutputWidth);
            Weights.RandomInit();

            IsConnected = true;
        }

        public void UpdateWeights(Matrix3D diffs)
        {
            Weights += diffs;
        }
    }
}
