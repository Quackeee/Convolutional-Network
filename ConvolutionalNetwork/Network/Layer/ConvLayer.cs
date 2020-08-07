using ConvolutionalNetwork.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class ConvLayer : HiddenLayer, ITrainableLayer
    {
        private ConvNeuron[] _neurons;
        private int _kernelSize;
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

        internal override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputHeight = _inputLayer.OutputHeight - _kernelSize + 1;
            OutputWidth = _inputLayer.OutputWidth - _kernelSize + 1;

            foreach (var n in _neurons) n.ConnectToInput(_inputLayer);
            _findWeightIndexes();

        }

        internal override void CalculateOutput()
        {
            if (IsConnected)
            {
                var output = new IMatrix[OutputDepth];
                var calculations = new Task[OutputDepth];

                for (int i = 0; i < OutputDepth; i++)
                {
                    int k = i;

                    calculations[k] = Task.Run(() =>
                    {
                        _neurons[k].CalculateOutput();
                        output[k] = _neurons[k].Output;
                    }
                    );
                }

                Task.WaitAll(calculations);

                _output = Factory.CreateMatrix3D(output);
                _activation.Run(_output);
            }
            else throw new InvalidOperationException("The Layer is not connected to any input");
        }

        internal override void PropagateDeltas(IMatrix3D previousDeltas)
        {
            var sw = Stopwatch.StartNew();
            Deltas = _activation.RecalculateDeltas(previousDeltas,Output);
            Debug.WriteLine($"Delta Recalculation Time: {sw.ElapsedMilliseconds}");
            sw.Restart();

            if (_inputLayer is HiddenLayer)
            {
                

                var deltas = new Matrix3D(_input.Dimensions);
                var calculations = new List<Task>();


                for (int k = 0; k < InputDepth; k++)
                {
                    int k2 = k;
                    calculations.Add(Task.Run(() =>
                    {
                        for (int i = 0; i < InputHeight; i++)
                        {

                            for (int j = 0; j < InputWidth; j++)
                            {
                                deltas[k2, i, j] = 0;
                                foreach (var neuron in _neurons)
                                {
                                    foreach (var indexes in _weightIndexes[i, j])
                                    {
                                        deltas[k2, i, j] += neuron.Weights[k2, indexes.Item1, indexes.Item2] * Deltas[k2, i - indexes.Item1, j - indexes.Item2];
                                    }
                                }
                            }
                        }

                    }));
                }

                Task.WhenAll(calculations).Wait();

                Debug.WriteLine($"Delta Propagation Time: {sw.ElapsedMilliseconds}");
                (_inputLayer as HiddenLayer).PropagateDeltas(deltas);
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

            _weightIndexes = new Tuple<int, int>[InputHeight, InputWidth][];
            for (int i = 0; i < InputHeight; i++)
                for (int j = 0; j < InputWidth; j++)
                {
                    _weightIndexes[i, j] = _findWeightIndexesFor(i, j);
                }
        }
        public void UpdateWeights()
        {
            var sw = Stopwatch.StartNew();

            var diffs = new Matrix3D[OutputDepth];
            var calculations = new Task[OutputDepth];

            for (int k = 0; k < OutputDepth; k++)
            {
                int kc = k;
                calculations[kc] = Task.Run(
                    () =>
                    {

                        diffs[kc] = new Matrix3D(InputDepth, _kernelSize, _kernelSize);

                        for (int i = 0; i < _kernelSize; i++)
                        {
                            int i2max = InputHeight - _kernelSize + i;
                            for (int j = 0; j < _kernelSize; j++)
                            {
                                int j2max = InputWidth - _kernelSize + j;
                                for (int i2 = i; i2 < i2max; i2++)
                                    for (int j2 = j; j2 < j2max; j2++)
                                        for (int k2 = 0; k2 < InputDepth; k2++)
                                            diffs[kc][k2, i, j] -= Deltas[kc, i2 - i, j2 - j] * _input[k2, i2, j2];
                            }
                        }

                        double biasDiff = 0;

                        for (int i = 0; i < OutputHeight; i++)
                            for (int j = 0; j < OutputWidth; j++)
                            {
                                biasDiff += Deltas[kc, i, j];
                            }

                        diffs[kc].Apply((d) => 0.03 * d);
                        biasDiff *= -0.03;

                        _neurons[kc].UpdateWeights(diffs[kc], biasDiff);
                    }
                );
            }

            Debug.WriteLine($"Weights Update time: {sw.ElapsedMilliseconds}");
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
