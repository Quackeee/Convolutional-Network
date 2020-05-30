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
        static void Main(string[] args)
        {
            Network network = new Network
            (
                new InputLayer(32, 32),
                new ConvLayer(6),
                new MaxPoolLayer(2),
                new ConvLayer(16),
                new MaxPoolLayer(2),
                new ConvLayer(120),
                new FullConLayer(84, ActivationFuncs.Sigmoid),
                //new FullConLayer(42, ActivationFuncs.Sigmoid),
                new FullConLayer(2, ActivationFuncs.SoftMax)
            ) ;

            var expectedApple = new Matrix3D(new Matrix(
                new double[,]
                  {
                                      {0},
                                      {1}
                  }
                ));
            var expectedDiamond = new Matrix3D
                (
                new Matrix(
                new double[,]
                  {
                                      {1},
                                      {0}
                  }
                ));

            
            for (int i = 0; i < 1000; i++)
            {
                Console.Clear();
                Console.WriteLine(i);
                network.FeedForward(new Bitmap(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\apple.png"));
                Console.WriteLine("Apple:");
                Console.WriteLine(network.Output);
                network.BackPropagate(expectedApple);
                network.FeedForward(new Bitmap(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\diamond.png"));
                Console.WriteLine("Diamond:");
                Console.WriteLine(network.Output);
                network.BackPropagate(expectedDiamond);
                
                //Console.ReadKey();
                Thread.Sleep(100);
            }
            network.FeedForward(new Bitmap(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\diamond.png"));
            Console.WriteLine("Diamond:");
            Console.WriteLine(network.Output);
            network.FeedForward(new Bitmap(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\apple.png"));
            Console.WriteLine("Apple:");
            Console.WriteLine(network.Output);
            Console.ReadKey();
        }
    }
}
