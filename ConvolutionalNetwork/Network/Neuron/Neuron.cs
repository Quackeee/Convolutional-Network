using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{

    abstract class NeuronBase
    {
        internal Matrix3D Weights;
        protected double _bias = (Rand.NextDouble() -0.5) * 2;

        internal abstract void CalculateOutput();
        internal abstract void ConnectToInput(NetworkLayer inputLayer);
        internal void UpdateWeights(Matrix3D diffs, double biasDiff)
        {
            Weights += diffs;
            _bias += biasDiff;
        }
    }

    class ConvNeuron : NeuronBase
    {
        private NetworkLayer _inputLayer;
        private Matrix _output;
        private int _kernelSize;

        public bool IsConnected => _inputLayer != null;

        public Matrix Output { get => _output; }

        private Matrix3D _input { get => _inputLayer.Output; }

        internal override void CalculateOutput()
        {
            if (IsConnected)
            {
                _output = ConvolveWhole(_input, Weights);
                _output.Apply((d) => _bias + d);
            }
            else throw new InvalidOperationException("The Neuron was not connected");
        }

        public ConvNeuron(int kernelSize)
        {
            _kernelSize = kernelSize;
        }

        internal override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            //Debug.WriteLine(inputLayer.OutputDepth);
            //Debug.WriteLine(Weights);

            Weights = new Matrix3D(inputLayer.OutputDepth, _kernelSize, _kernelSize);
            Weights.RandomInit();
        }
    }

    class Neuron : NeuronBase
    {
        private NetworkLayer _inputLayer;

        public double Output { get; private set; }
        public bool IsConnected => _inputLayer != null;

        internal override void CalculateOutput()
        {
            if (IsConnected)
            {
                double output = 0;

                for (int k = 0; k < Weights.Depth; k++)
                    for (int i = 0; i < Weights.Height; i++)
                        for (int j = 0; j < Weights.Width; j++)

                            output += _inputLayer.Output[k, i, j] * Weights[k, i, j];

                Output = output + _bias;
            }
            else throw new InvalidOperationException("The Neuron was not connected");
        }

        internal override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            Weights = new Matrix3D(inputLayer.OutputDepth, inputLayer.OutputHeight, inputLayer.OutputWidth);
            Weights.RandomInit();
        }
    }
}
