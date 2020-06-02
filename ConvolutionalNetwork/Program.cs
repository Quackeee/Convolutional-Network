using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    class Program
    {
        static void Main()
        {
            Network network = new Network
            (
                new InputLayer(128, 96, false),
                new ConvLayer(6),
                new MaxPoolLayer(2),
                new ConvLayer(16),
                new MaxPoolLayer(2),
                new ConvLayer(120),
                new FullConLayer(84, ActivationFuncs.Sigmoid),
                new FullConLayer(9, ActivationFuncs.SoftMax)
            );

            network.TrainingSet = new LearningSet(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\LearningSet",false);
            network.TestSet = new LearningSet(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\TestSet",false);


            network.Load(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\bin\Debug\backup.cnet");
            network.Test();

            Console.ReadKey();
        }
    }
}
