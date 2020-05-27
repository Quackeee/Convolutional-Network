using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            /*  Matrix matrix = new Matrix
              (
                  new double[,]
                  {
                          {1,6,0,3,9,3},
                          {2,7,1,-1,2,8},
                          {3,2,8,6,2,4 },
                          {2,1,1,7,5,3 },
                          {5,4,-4,7,6,3 },
                          {7,6,5,9,1,0 }
                  }
              );

              //Console.WriteLine(matrix);
              //Console.WriteLine(MaxPool(matrix, 2));

              Matrix matrix2 = new Matrix
              (
                  new double[,]
                  {
                      {0,0,0},
                      {0,1,1},    
                      {0,1,0}
                  }
              );
              Matrix kernel = new Matrix
              (
                  new double[,]
                  {
                          {-1,0,1},
                          {-2,0,2},
                          {-1,0,1}
                  }
              );

              //Console.WriteLine(matrix2);
              //Console.WriteLine(kernel);
              //Console.WriteLine(Convolve(matrix2, kernel));

              Matrix matrix3 = new Matrix
              (
                  new double[,]
                  {
                          {0,0,0,0,0,0,0,0},
                          {0,1,1,1,1,1,1,0},
                          {0,1,0,0,0,0,0,0},
                          {0,1,1,1,1,1,0,0},
                          {0,0,0,0,0,0,1,0},
                          {0,1,0,0,0,0,1,0},
                          {0,0,1,1,1,1,0,0},
                          {0,0,0,0,0,0,0,0}
                  }
              );

              Console.WriteLine(matrix3);
              Console.WriteLine(ConvolveWhole(matrix3, kernel));
              */

            Network network = new Network
            (
                new InputLayer(32, 32),
                new ConvLayer(6),
                new MaxPoolLayer(2),
                new ConvLayer(16),
                new MaxPoolLayer(2),
                //new ConvLayer(120),
                new FullConLayer(84, ActivationFuncs.Sigmoid),
                new FullConLayer(10, ActivationFuncs.SoftMax)
            ) ;



            network.FeedForward(new Bitmap(@"D:\Users\Krzysiu\Documents\Studia\Programowanie\AI\ConvolutionalNetwork\ConvolutionalNetwork\apple.png"));

            var networkOutput = network.Output;

            var expected = new Matrix3D(new Matrix(
                new double[,]
                  {
                          {0},
                          {0},
                          {0},
                          {1},
                          {0},
                          {0},
                          {0},
                          {0},
                          {0},
                          {0}
                  }
                ));


            Console.WriteLine($"{networkOutput.Depth} {networkOutput.Height} {networkOutput.Width}");
            Console.WriteLine(networkOutput);

            network.BackPropagate(expected);

            Console.ReadKey();



        }
    }
}
