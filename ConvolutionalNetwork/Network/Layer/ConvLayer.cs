﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class ConvLayer : HiddenLayer
    {
        private ConvNeuron[] _neurons;
        private int _kernelSize;
        private ActivationFunc _activation;
        private Tuple<int, int>[,][] _weightIndexes;

        public ConvLayer(int outputCount, ActivationFunc activation = null, int kernelSize = 5)
        {
            if (activation == null) activation = ActivationFuncs.ReLU;

            OutputDepth = outputCount;

            _kernelSize = kernelSize;
            _activation = activation;

            _neurons = new ConvNeuron[outputCount];


            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new ConvNeuron(kernelSize);
            }
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputHeight = _inputLayer.OutputHeight - _kernelSize + 1;
            OutputWidth = _inputLayer.OutputWidth - _kernelSize + 1;

            foreach (var n in _neurons) n.ConnectToInput(_inputLayer);
            _findWeightIndexes();
        }

        public override void CalculateOutput()
        {
            if (IsConnected)
            {
                var output = new Matrix[OutputDepth];

                for (int i = 0; i < OutputDepth; i++)
                {
                    _neurons[i].CalculateOutput();
                    output[i] = _neurons[i].Output;
                }
                _output = new Matrix3D(output);
                _activation.Run(_output);
            }
            else throw new InvalidOperationException("The Layer is not connected to any input");
        }

        public override void LoadAndPropagateDeltas(Matrix3D previousDeltas)
        {
            Deltas = _activation.RecalculateDeltas(previousDeltas,Output);

            Console.WriteLine("Calculating deltas in ConvLayer");
            //throw new NotImplementedException();

            if (_inputLayer is HiddenLayer)
            {

                var deltas = new Matrix3D(_input.Dimensions);

                for (int k = 0; k < InputDepth; k++)
                {
                    for (int i = 0; i < InputHeight; i++)
                    {
                        for (int j = 0; j < InputWidth; j++)
                        {
                            deltas[k, i, j] = 0;
                            foreach (var neuron in _neurons)
                            {
                                foreach (var indexes in _weightIndexes[i, j])
                                {
                                    deltas[k, i, j] += neuron.Weights[k, indexes.Item1, indexes.Item2] * Deltas[k, i - indexes.Item1, j - indexes.Item2];
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(deltas);
                (_inputLayer as HiddenLayer).LoadAndPropagateDeltas(deltas);
            }
        }

        
        private void _findWeightIndexes()
        {
            Tuple<int, int>[] _findWeightIndexesFor(int i, int j)
            {
                var weightIndexes = new List<Tuple<int, int>>();

                for (int l = 0; l < _kernelSize; l++)
                {
                    for (int m = 0; m < _kernelSize; m++)
                    {
                        int i2 = i - l;
                        int j2 = j - m;
                        if (i2 >= 0 && i2 < OutputHeight && j2 >= 0 && j2 < OutputWidth)
                            weightIndexes.Add(new Tuple<int, int>(l, m));
                    }
                }

                return weightIndexes.ToArray();
            }

            _weightIndexes = new Tuple<int, int>[InputHeight,InputWidth][];
            for (int i = 0; i < InputHeight; i++)
                for (int j = 0; j < InputWidth; j++)
                {
                    _weightIndexes[i, j] = _findWeightIndexesFor(i, j);
                }
        }
    }
}
