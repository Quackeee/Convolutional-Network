using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //var sw = Stopwatch.StartNew();

            for (int k = 0; k < Output.Depth; k++)
                for (int i = 0; i < Output.Height; i++)
                    for (int j = 0; j < Output.Width; j++)
                        deltas[k, i, j] = -expectedOutput[k, i, j] / Output[k, i, j];

            //Debug.WriteLine($"Delta calculation time {sw.ElapsedMilliseconds}");

            _layers.Last().PropagateDeltas(deltas);

            foreach (var layer in _layers)
                    (layer as ITrainableLayer)?.UpdateWeights();
        }

        public void Train(int epochs)
        {
            //var sw = new Stopwatch();
            double? testOutput = null;
            double? previousTestOutput;

            double sumOfCosts = 0;

            if (TrainingSet != null)
            {
                for (int i = 0; i < epochs; i++)
                {
                    Console.WriteLine($"Training epoch {i + 1}/{epochs}");
                    int j = 0;

                    foreach (var pair in TrainingSet)
                    {
                        Console.Write($"\r{j}/{TrainingSet.Size}");

                        //sw.Restart();

                        FeedForward(pair.Item1);
                        //Console.WriteLine("output:");
                        //Console.WriteLine(Output);
                        //Console.WriteLine("expected:");
                        //Console.WriteLine(pair.Item2);

                        sumOfCosts = Utils.Cost(Output, pair.Item2);

                        //Console.WriteLine($"ForwardFeed time: {sw.ElapsedMilliseconds}");

                        //sw.Restart();
                        BackPropagate(pair.Item2);

                        //Console.Clear();
                        //Console.WriteLine($"Last backpropagation time: {sw.ElapsedMilliseconds}");

                        j++;
                    }

                    Console.WriteLine($"\r{j}/{TrainingSet.Size}");
                    Console.WriteLine($"Average cost: {sumOfCosts / TrainingSet.Size}");


                    previousTestOutput = testOutput ?? double.PositiveInfinity;
                    Test(error => testOutput = error);
                    Console.WriteLine($"Cost difference (should be positive): {previousTestOutput - testOutput}");

                    Save(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\bin\Debug\backup.cnet");
                }
            }
            else throw new InvalidOperationException("There was no training set included for this network");
        }

        public void Test(Action<double> listener = null)
        {
            if (TestSet != null)
            {
                TestSet.ReadingOrder = ReadingOrder.Random;

                int goodGuesses = 0;
                int i = 0;
                double sumOfCosts = 0;

                Console.WriteLine("Running tests...");

                foreach (var pair in TestSet)
                {
                    Console.Write($"\r{i}/{TestSet.Size}");

                    FeedForward(pair.Item1);
                    //Console.Clear();
                    //Console.WriteLine(Output);
                    //Console.WriteLine("expected:");
                    //Console.WriteLine(pair.Item2);

                    int maxIndex = 0;
                    for (int n = 0; n < Output.Height; n++)
                    {
                        if (Output[0, n, 0] > Output[0, maxIndex, 0]) maxIndex = n;
                    }
                    //string goodOrBad = pair.Item2[0, maxIndex, 0] == 1 ? "good" : "bad";
                    if (pair.Item2[0, maxIndex, 0] == 1) goodGuesses += 1;

                    //Console.Write($"item nr {i} belongs to class {maxIndex} with {(int)(Output[0, maxIndex, 0] * 100)}% certainty.\n" +
                    //    $"that's a {goodOrBad} guess");

                    sumOfCosts += Utils.Cost(Output, pair.Item2);

                    i++;
                }

                Console.WriteLine($"\r{TestSet.Size}/{TestSet.Size}");

                double accuracy = (double)goodGuesses / TestSet.Size;
                double averageCost = sumOfCosts / TestSet.Size;

                listener?.Invoke(averageCost);

                Console.WriteLine($"We got {accuracy * 100}% accuracy and average cost of {averageCost}!");

                //Console.ReadKey();
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
