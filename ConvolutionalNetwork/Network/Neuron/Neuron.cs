using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{

    internal abstract class NeuronBase
    {
        internal Matrix3D Weights;
        protected double _bias = (Rand.NextDouble() -0.5) * 2;

        internal abstract void CalculateOutput();
        internal abstract void ConnectToInput(NetworkLayer inputLayer);
        internal void UpdateWeights(Matrix3D diffs, double biasDiff)
        {
            Weights.Add(diffs);
            _bias += biasDiff;
        }
        internal void StreamWeights(StreamWriter sw)
        {
            for (int k = 0; k < Weights.Depth; k++)
                for (int i = 0; i < Weights.Height; i++)
                    for (int j = 0; j < Weights.Width; j++)
                    {
                        sw.WriteLine(Weights[k, i, j]);
                    }
            sw.WriteLine(_bias);
        }
        internal void ReadWeights(StreamReader sr)
        {
            for (int k = 0; k < Weights.Depth; k++)
                for (int i = 0; i < Weights.Height; i++)
                    for (int j = 0; j < Weights.Width; j++)
                    {
                        Weights[k, i, j] = Convert.ToDouble(sr.ReadLine());
                    }
            _bias = Convert.ToDouble(sr.ReadLine());
        }
    }

    class ConvNeuron : NeuronBase
    {
        private NetworkLayer _inputLayer;
        private Matrix _output;
        private int _kernelSize;

        private bool IsConnected => _inputLayer != null;

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

        internal ConvNeuron(int kernelSize)
        {
            _kernelSize = kernelSize;
        }

        internal override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            Weights = new Matrix3D(inputLayer.OutputDepth, _kernelSize, _kernelSize);
            Weights.RandomInit();
        }


    }

    class Neuron : NeuronBase
    {
        private NetworkLayer _inputLayer;

        public double Output { get; private set; }
        private bool IsConnected => _inputLayer != null;

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
