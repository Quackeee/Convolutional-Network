﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    public class MaxPoolLayer : HiddenLayer
    {
        private int _stride;

        public MaxPoolLayer(int stride)
        {
            _stride = stride;
        }

        public override void LoadAndPropagateDeltas(Matrix3D previousDeltas)
        {
            Console.WriteLine("Calculating deltas in MaxPoolLayer");
            Deltas = previousDeltas;


            if (_inputLayer is HiddenLayer)
            {
                var deltas = new Matrix3D(_input.Dimensions);
                deltas.ZeroInit();
                for (int k = 0; k < OutputDepth; k++)
                {
                    for (int i = 0; i < OutputHeight; i++)
                    {
                        var i2max = i * _stride + _stride;
                        for (int j = 0; j < OutputHeight; j++)
                        {
                            var j2max = j * _stride + _stride;

                            for (int i2 = i * _stride; i2 < i2max; i2++)
                                for (int j2 = j * _stride; j2 < j2max; j2++)
                                    if (Output[k, i, j] == _input[k, i2, j2])
                                    {
                                        deltas[k, i2, j2] = Deltas[k, i, j];
                                        i2 = i2max;
                                        j2 = j2max;
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
            var output = new Matrix[_input.Depth];

            for (int i = 0; i < _input.Depth; i++)
            {
                output[i] = MaxPool(_input[i], _stride);
            }

            _output = new Matrix3D(output);
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputDepth = _inputLayer.OutputDepth;
            OutputHeight = _inputLayer.OutputHeight / _stride;
            OutputWidth = _inputLayer.OutputWidth / _stride;
        }
    }
}
