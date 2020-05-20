using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ConvolutionalNetwork
{
    static class Utils
    {
        public static double Convolve(Matrix matrix, Matrix kernel)
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

        public static double Convolve(Matrix matrix, Matrix kernel, int yoffset, int xoffset)
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
    
        public static Matrix MaxPool(Matrix matrix, int stride)
        {
            if (matrix.Width % stride != 0 || matrix.Height % stride != 0)
                throw new InvalidOperationException(
                    $"Can't Pool matrix if dimensions aren't divisable by the stride;" +
                    $"\nWidth={matrix.Height} Height={matrix.Width} Stride={stride}"
                    );

            int polledWidth = matrix.Width / stride;
            int polledHeight = matrix.Height / stride;


            Matrix polledMatrix = new Matrix(polledHeight, polledWidth);

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
    
        public static Matrix ConvolveWhole(Matrix matrix, Matrix kernel)
        {

            var output = new Matrix(matrix.Height - kernel.Height + 1, matrix.Width - kernel.Width + 1);

            for (int i = 0; i < output.Height; i++)
            {
                for (int j = 0; j < output.Width; j++)
                {
                    output[i, j] = Convolve(matrix, kernel, i, j);
                }
            }

            return output;
        }

        public static Matrix[] AsMatrixRGB(this Bitmap bitmap)
        {
            var output = new Matrix[3];

            for (int i = 0; i < 3; i++)
            {
                output[i] = new Matrix(bitmap.Height, bitmap.Width);
            }

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    output[0][i, j] = bitmap.GetPixel(j, i).R;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    output[1][i, j] = bitmap.GetPixel(j, i).G;

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    output[2][i, j] = bitmap.GetPixel(j, i).B;

            return output;
        }

        public static Matrix AsMatrixGrayScale(this Bitmap bitmap)
        {
            var output = new Matrix(bitmap.Height, bitmap.Width);

            for (int i = 0; i < bitmap.Height; i++)
                for (int j = 0; j < bitmap.Width; j++)
                    output[i,j] = bitmap.GetPixel(j, i).GetSaturation();

            return output;
        }

    }
}
