﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class FullConLayer : HiddenLayer, ITrainableLayer
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

            //Console.WriteLine($"{OutputDepth}x{OutputHeight}x{OutputWidth}");
        }

        internal override void PropagateDeltas(Matrix3D previousDeltas)
        {
            //Console.WriteLine("Calculating deltas in FullConLayer");
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

                //Console.WriteLine(deltas);
                (_inputLayer as HiddenLayer).PropagateDeltas(deltas);
            }
        }

        internal override void CalculateOutput()
        {
            for (int i = 0; i < OutputHeight; i++)
            {
                _neurons[i].CalculateOutput();
                _output[0, i, 0] = _neurons[i].Output;
            }
            _activation.Run(_output);
        }

        internal override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            foreach (var neuron in _neurons)
                neuron.ConnectToInput(_inputLayer);
        }

        public void UpdateWeights()
        {
            //Console.WriteLine("updating weights in FullConLayer");

            var diffs = new Matrix3D(_input.Dimensions);
            //Console.WriteLine("deltas:");
            //Console.WriteLine(Deltas);
            for (int n = 0; n < _neurons.Length; n++)
            {
                diffs.ZeroInit();

                for (int k = 0; k < InputDepth; k++)
                    for (int i = 0; i < InputHeight; i++)
                        for (int j = 0; j < InputWidth; j++)
                            diffs[k, i, j] += Deltas[0, n, 0] * _input[k, i, j];

                diffs.Apply((d) => -0.03 * d);
                double biasDiff = - 0.03 * Deltas[0, n, 0];

                _neurons[n].UpdateWeights(diffs,biasDiff);
                
            }
            //Console.WriteLine(_neurons[1].Weights);
        }

        public void StreamWeights(StreamWriter sw)
        {
            foreach (var neuron in _neurons)
            {
                neuron.StreamWeights(sw);
            }
        }

        public void ReadWeights(StreamReader sr)
        {
            foreach (var neuron in _neurons)
            {
                neuron.ReadWeights(sr);
            }
        }
    }
}
