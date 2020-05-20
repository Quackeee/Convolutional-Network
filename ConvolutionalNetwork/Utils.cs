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

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    for (int k = 0; k < matrix.Depth; k++)
                    {
                        convolution += matrix[k, i, j] * kernel[k, i, j];
                    }
                }
            }

            return convolution;
        }

        public static double Convolve(Matrix matrix, Matrix kernel, int yoffset, int xoffset)
        {
            if (matrix.Depth != kernel.Depth) throw new InvalidOperationException("Can't convolve matrices of different depths");

            double convolution = 0;

            for (int k = 0; k < kernel.Depth; k++)
            {
                for (int i = 0; i < kernel.Width; i++)
                {
                    for (int j = 0; j < kernel.Height; j++)
                    {
                        convolution += matrix[k, j + yoffset, i + xoffset] * kernel[k, j, i];
                    }
                }
            }
            return convolution;
        }
    
        public static Matrix MaxPool(Matrix m, int stride)
        {
            if (m.Width % stride != 0 || m.Height % stride != 0)
                throw new InvalidOperationException(
                    $"Can't Pool matrix if dimensions aren't divisable by the stride;" +
                    $"\nWidth={m.Height} Height={m.Width} Stride={stride}"
                    );

            int polledWidth = m.Width / stride;
            int polledHeight = m.Height / stride;


            Matrix polledMatrix = new Matrix(polledWidth, polledHeight, m.Depth);

            

            for (int k = 0; k < m.Depth; k++)
            {
                for (int l = 0; l < polledHeight; l++)
                {
                    for (int n = 0; n < polledWidth; n++)
                    {
                        var compared = new List<double>();
                        int imax = l * stride + stride;

                        for (int i = l * stride; i < imax; i++)
                        {
                            int jmax = n * stride + stride;
                            
                            for (int j = n * stride; j < jmax; j++)
                            {
                                //Console.WriteLine(i + " " + j + " -> " + m[k, i, j]);
                                compared.Add(m[k, i, j]);
                            }
                        }
                        polledMatrix[k, l, n] = compared.Max();
                    }                    
                }
            }
            return polledMatrix;
        }
    
        public static Matrix ConvolveWhole(Matrix matrix, Matrix kernel)
        {
            if (matrix.Depth!= kernel.Depth) throw new InvalidOperationException("Can't convolve matrices of different depths");

            int outputWidth = matrix.Width - kernel.Width + 1;
            int outputHeight = matrix.Height - kernel.Height + 1;

            var output = new Matrix(outputWidth,outputHeight,1);

            for (int i = 0; i < outputHeight; i++)
            {
                for (int j = 0; j < outputWidth; j++)
                {
                    output[0, i, j] = Convolve(matrix, kernel, i, j);
                }
            }

            return output;
        }

        public static Matrix AsMatrix(this Bitmap bitmap, bool rgb = true)
        {
            Matrix output;

            if (rgb)
            {
                output = new Matrix(bitmap.Width, bitmap.Height, 3);
                for (int i = 0; i < bitmap.Height; i++)
                    for (int j = 0; j < bitmap.Width; j++)
                        output[0, i, j] = bitmap.GetPixel(j, i).R;

                for (int i = 0; i < bitmap.Height; i++)
                    for (int j = 0; j < bitmap.Width; j++)
                        output[1, i, j] = bitmap.GetPixel(j, i).G;

                for (int i = 0; i < bitmap.Height; i++)
                    for (int j = 0; j < bitmap.Width; j++)
                        output[2, i, j] = bitmap.GetPixel(j, i).B;
            }
            else
            {
                output = new Matrix(bitmap.Width, bitmap.Height, 3);
                for (int i = 0; i < bitmap.Height; i++)
                    for (int j = 0; j < bitmap.Width; j++)
                        output[0, i, j] = bitmap.GetPixel(j, i).GetSaturation();
            }

            return output;
        }

    }
}
