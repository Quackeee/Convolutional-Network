using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    class Network
    {
        private InputLayer _input;
        private HiddenLayer[] _layers;
        public LearningSet TrainingSet;
        public LearningSet TestSet;

        public Matrix3D Output { get => _layers.Last().Output; }

        public Network(InputLayer inputLayer, params HiddenLayer[] layers)
        {
            _input = inputLayer;
            _layers = layers;

            _layers[0].ConnectToInput(inputLayer);

            for (int i = 1; i < _layers.Length; i++)
            {
                _layers[i].ConnectToInput(_layers[i - 1]);
            }
        }

        public void FeedForward(Bitmap bm)
        {
            _input.PushInput(bm);

            foreach (var layer in _layers)
                layer.CalculateOutput();
        }

        public void FeedForward(Matrix3D m)
        {
            _input.PushInput(m);

            foreach (var layer in _layers)
                layer.CalculateOutput();
        }

        public void BackPropagate(Matrix3D expectedOutput)
        {
            var deltas = new Matrix3D(expectedOutput.Dimensions);

            for (int k = 0; k < Output.Depth; k++)
                for (int i = 0; i < Output.Height; i++)
                    for (int j = 0; j < Output.Width; j++)
                        deltas[k, i, j] = -expectedOutput[k, i, j] / Output[k, i, j];

            _layers.Last().PropagateDeltas(deltas);

            foreach (var layer in _layers)
                    (layer as ITrainableLayer)?.UpdateWeights();
        }

        public void Train(int epochs)
        {
            if (TrainingSet != null)
            {
                int cathegorySize = TrainingSet.Size / Output.Height;
                int k = 0;
                int j = 0;

                for (int i = 0; i < epochs; i++)
                {
                    if (i%100 == 0) Save(@".\backup.cnet");

                    Console.WriteLine(i);
                    var pair = TrainingSet.GetPairAt(j + k*cathegorySize);
                    FeedForward(pair.Item1);
                    Console.WriteLine("output:");
                    Console.WriteLine(Output);
                    Console.WriteLine("expected:");
                    Console.WriteLine(pair.Item2);
                    BackPropagate(pair.Item2);
                    Console.Clear();
                    k++;
                    if (k == Output.Height) { k = 0; j++; }
                    if (j == cathegorySize) j = 0;
                }
            }
            else throw new InvalidOperationException("There was no training set included for this network");
        }

        public void Test()
        {
            if (TestSet != null)
            {
                int goodGuesses = 0;

                for (int i = 0; i < TestSet.Size; i++)
                {
                    var pair = TrainingSet.GetPairAt(i);

                    FeedForward(pair.Item1);
                    Console.WriteLine(Output);
                    Console.WriteLine("expected:");
                    Console.WriteLine(pair.Item2);

                    int maxIndex = 0;
                    for (int n = 0; n < Output.Height; n++)
                    {
                        if (Output[0, n, 0] > Output[0, maxIndex, 0]) maxIndex = n;
                    }
                    string goodOrBad = pair.Item2[0, maxIndex, 0] == 1 ? "good" : "bad";
                    if (pair.Item2[0, maxIndex, 0] == 1) goodGuesses += 1;

                    Console.Write($"item nr {i} belongs to class {maxIndex} with {(int)(Output[0, maxIndex, 0] * 100)}% certainty.\n" +
                        $"that's a {goodOrBad} guess");
                }

                Console.WriteLine($"We got {(double) goodGuesses / TestSet.Size}% accuracy!");
            }
            else throw new InvalidOperationException("There was no test set included for this network");
        }

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var layer in _layers)
                {
                    (layer as ITrainableLayer)?.StreamWeights(sw);
                }
            }
        }

        public void Load(string path)
        {
            using(StreamReader sr = new StreamReader(path))
            {
                foreach (var layer in _layers)
                {
                    (layer as ITrainableLayer)?.ReadWeights(sr);
                }
            }
        }
    }
}
