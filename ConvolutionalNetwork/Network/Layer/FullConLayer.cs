using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    public class FullConLayer : HiddenLayer, ITrainableLayer
    {
        Neuron[] _neurons;

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

        }

        internal override void PropagateDeltas(Matrix3D previousDeltas)
        {

            var sw = Stopwatch.StartNew();
            Deltas = _activation.RecalculateDeltas(previousDeltas, _output);
            Debug.WriteLine($"Delta Recalculation Time: {sw.ElapsedMilliseconds}");

            sw.Restart();

            if (_inputLayer is HiddenLayer)
            {
                

                var deltas = new Matrix3D(InputDepth, InputHeight, InputWidth);
                var calculations = new List<Task>(InputDepth);

                for (int k = 0; k < InputDepth; k++)
                {
                    int k2 = k;

                    calculations.Add(Task.Run(() =>
                    {
                        for (int i = 0; i < InputHeight; i++)
                        {
                            for (int j = 0; j < InputWidth; j++)
                            {
                                for (int n = 0; n < OutputHeight; n++)
                                {
                                    deltas[k2, i, j] += Deltas[0, n, 0] * _neurons[n].Weights[k2, i, j];
                                }
                            }
                        }
                    }
                    ));
                }

                Task.WhenAll(calculations).Wait();

                Debug.WriteLine($"Delta Propagation Time: {sw.ElapsedMilliseconds}");

                (_inputLayer as HiddenLayer).PropagateDeltas(deltas);
            }
        }


        internal override void CalculateOutput()
        {
            var calculations = new Task[OutputHeight];

            for (int i = 0; i < OutputHeight; i++)
            {
                int k = i;

                calculations[k] =
                    Task.Run(
                        () => {
                            _neurons[k].CalculateOutput();
                            _output[0, k, 0] = _neurons[k].Output;
                        }
                );
            }

            Task.WaitAll(calculations);
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
            var diffs = new Matrix3D[_neurons.Length];
            var sw = Stopwatch.StartNew();

            var calculations = new Task[_neurons.Length];

            for (int n = 0; n < _neurons.Length; n++)
            {
                int n2 = n;

                calculations[n2] = Task.Run(
                    () =>
                    {
                        diffs[n2] = new Matrix3D(_input.Dimensions);

                        for (int k = 0; k < InputDepth; k++)
                            for (int i = 0; i < InputHeight; i++)
                                for (int j = 0; j < InputWidth; j++)
                                    diffs[n2][k, i, j] += Deltas[0, n2, 0] * _input[k, i, j];

                        diffs[n2].Apply((d) => -0.03 * d);
                        double biasDiff = -0.03 * Deltas[0, n2, 0];

                        _neurons[n2].UpdateWeights(diffs[n2], biasDiff);
                    }
                );
            }

            Task.WaitAll(calculations);

            Debug.WriteLine($"Weight Update Time: {sw.ElapsedMilliseconds}");
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
