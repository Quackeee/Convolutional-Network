using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ConvolutionalNetwork.Misc;

namespace ConvolutionalNetwork
{
    static class Utils
    {

        private static Random _rand;
        public static Random Rand
        {
            get
            {
                if (_rand == null)
                {
                    _rand = new Random();
                }
                return _rand;
            }
        }

        public static double Convolve(IMatrix matrix, IMatrix kernel)
        {
            //if (m1.Dimensions != m2.Dimensions)
                //throw new InvalidOperationException("Can't convolve matrices of different dimensions");
            
            double convolution = 0;

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    convolution += matrix[i, j] * kernel[i, j];
                }
            }

            return convolution;
        }

        public static double Convolve(IMatrix matrix, IMatrix kernel, int yoffset, int xoffset)
        {
            double convolution = 0;

                for (int i = 0; i < kernel.Height; i++)
                {
                    for (int j = 0; j < kernel.Width; j++)
                    {
                        convolution += matrix[i + yoffset, j + xoffset] * kernel[i, j];
                    }
                }
            return convolution;
        }
    
        public static IMatrix MaxPool(IMatrix matrix, int stride)
        {
            if (matrix.Width % stride != 0 || matrix.Height % stride != 0)
                throw new InvalidOperationException(
                    $"Can't Pool matrix if dimensions aren't divisable by the stride;" +
                    $"\nWidth={matrix.Height} Height={matrix.Width} Stride={stride}"
                    );

            int polledWidth = matrix.Width / stride;
            int polledHeight = matrix.Height / stride;


            IMatrix polledMatrix = new Matrix(polledHeight, polledWidth);

                for (int l = 0; l < polledHeight; l++)
                {
                    for (int m = 0; m < polledWidth; m++)
                    {
                        var compared = new List<double>();
                        int imax = l * stride + stride;

                        for (int i = l * stride; i < imax; i++)
                        {
                            int jmax = m * stride + stride;
                            
                            for (int j = m * stride; j < jmax; j++)
                            {
                                //Console.WriteLine(i + " " + j + " -> " + m[k, i, j]);
                                compared.Add(matrix[i, j]);
                            }
                        }
                        polledMatrix[l, m] = compared.Max();
                    }                    
                }
            return polledMatrix;
        }
    
        public static IMatrix ConvolveWhole(IMatrix matrix, IMatrix kernel)
        {

            var convolution = new Matrix(matrix.Height - kernel.Height + 1, matrix.Width - kernel.Width + 1);

            for (int i = 0; i < convolution.Height; i++)
            {
                for (int j = 0; j < convolution.Width; j++)
                {
                    convolution[i, j] = Convolve(matrix, kernel, i, j);
                }
            }

            return convolution;
        }

        public static IMatrix ConvolveWhole(IMatrix3D matrix, IMatrix3D kernel)
        {
            var convolution = new Matrix(matrix[0].Height - kernel[0].Height + 1, matrix[0].Width - kernel[0].Width + 1);

            for (int k = 0; k < matrix.Depth; k++)
            {
                for (int i = 0; i < convolution.Height; i++)
                {
                    for (int j = 0; j < convolution.Width; j++)
                    {
                        convolution[i, j] += Convolve(matrix[k], kernel[k], i, j);
                    }
                }
            }
            
            //Console.WriteLine($"{convolution.Height}x{convolution.Width}");

            return convolution;
        }

        public static void ConvolveWholeInto(IMatrix3D matrix, IMatrix3D kernel, IMatrix convolution)
        {
            convolution.ZeroInit();

            for (int k = 0; k < matrix.Depth; k++)
            {
                for (int i = 0; i < convolution.Height; i++)
                {
                    for (int j = 0; j < convolution.Width; j++)
                    {
                        convolution[i, j] += Convolve(matrix[k], kernel[k], i, j);
                    }
                }
            }
        }

        public static IMatrix3D AsMatrixRGB(this Bitmap bitmap)
        {
            var rMatrix = Factory.CreateMatrix(bitmap.Height, bitmap.Width);
            var gMatrix = Factory.CreateMatrix(bitmap.Height, bitmap.Width);
            var bMatrix = Factory.CreateMatrix(bitmap.Height, bitmap.Width);

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    rMatrix[i, j] = bitmap.GetPixel(j, i).R/255;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    gMatrix[i, j] = bitmap.GetPixel(j, i).G/255;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    bMatrix[i, j] = bitmap.GetPixel(j, i).B/255;

            return Factory.CreateMatrix3D(rMatrix, bMatrix, bMatrix);
        }

        public static IMatrix AsMatrixGrayscale(this Bitmap bitmap)
        {
            var output = Factory.CreateMatrix(bitmap.Height, bitmap.Width);

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    output[i,j] = bitmap.GetPixel(j, i).GetSaturation();

            return output;
        }

        public static IMatrix3D CreateVersor(int dimensions, int direction)
        {
            if (direction >= dimensions || direction < 0) throw new ArgumentOutOfRangeException("direction", "direction must be at least 0 and lower than dimensions");
            var versor = Factory.CreateMatrix3D(1, dimensions, 1);
            versor[0, direction, 0] = 1;
            return versor;
        }


    }
}
