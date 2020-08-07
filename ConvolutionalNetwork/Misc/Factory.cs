using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork.Misc
{
    static class Factory
    {
        public static IMatrix CreateMatrix(int height, int width)
        {
            return new Matrix(height, width);
        }
        public static IMatrix CreateMatrix(double[,] matrix)
        {
            return new Matrix(matrix);
        }

        public static IMatrix3D CreateMatrix3D(int depth, int height, int width)
        {
            return new Matrix3D(depth, height, width);
        }
        public static IMatrix3D CreateMatrix3D(params IMatrix[] matrices)
        {
            return new Matrix3D(matrices);
        }
        public static IMatrix3D CreateMatrix3D(Tuple<int, int, int> dimensions) 
        {
            return CreateMatrix3D(dimensions.Item1, dimensions.Item2, dimensions.Item3);
        }
    }
}
