using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            #region definicje

            Stopwatch sw = new Stopwatch();

            var input1 = new Matrix
                (
                new double[,]
                {
                    {0,0,0,0,0,0,0 },
                    {0,2,2,2,1,1,0 },
                    {0,2,1,1,2,2,0 },
                    {0,2,0,0,0,0,0 },
                    {0,1,0,2,1,0,0 },
                    {0,2,1,2,2,1,0 },
                    {0,0,0,0,0,0,0 }
                }
                );

            var input2 = new Matrix
                (
                new double[,]
                {
                    {0,0,0,0,0,0,0 },
                    {0,0,2,0,0,1,0 },
                    {0,2,2,1,2,1,0 },
                    {0,0,2,1,1,1,0 },
                    {0,0,2,2,1,1,0 },
                    {0,1,0,0,2,0,0 },
                    {0,0,0,0,0,0,0 }
                }
                );

            var input3 = new Matrix
                (
                new double[,]
                {
                    { 0,0,0,0,0,0,0 },
                    { 0,2,1,2,2,0,0 },
                    { 0,0,2,0,1,1,0 },
                    { 0,2,0,0,1,0,0 },
                    { 0,1,1,1,0,0,0 },
                    { 0,1,0,0,1,2,0 },
                    { 0,0,0,0,0,0,0 }
                }
                );

            var input = new Matrix3D(input1, input2, input3);

            var filter1 = new Matrix
                (
                new double[,]
                {
                    {-1,1,-1 },
                    {1,1,-1 },
                    {1,1,0 }
                }
                );
            var filter2 = new Matrix
                (
                new double[,]
                {
                    {1,0,1 },
                    {0,1,1 },
                    {1,-1,0 }
                }
                );
            var filter3 = new Matrix
                (
                new double[,]
                {
                    {-1,-1,1 },
                    {0,1,0 },
                    {0,0,0 }
                }
                );

            var filter = new Matrix3D(filter1, filter2, filter3);

            #endregion


            Matrix output = new Matrix(5,5);
            output.ZeroInit();
            double miliseconds = 0;

            for (int i = 0; i < 100000; i++)
            {
                sw.Restart();
                output = ConvolveWhole(input, filter);
                sw.Stop();
                miliseconds += sw.Elapsed.TotalMilliseconds;
            }
            

            Console.WriteLine(output);
            Console.WriteLine(miliseconds/1000000);

            /*
             *  should be
             *  |4, 8, 8, 7, 8, |
             *  |8, 4, 4, 0, 4, |
             *  |12, 3, 5, 8, 2, |
             *  |7, 5, 12, 8, 7, |
             *  |6, -1, 5, 7, 5, |
             *  
             */

            ActivationFuncs.SoftMax.Run(output);

            Console.WriteLine(output);

            Console.ReadKey();
        }
    }
}
