using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{

    public class Matrix : IMatrix
    {
        private double[,] _matrix;

        public int Width { get; }
        public int Height { get; }

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;

            _matrix = new double[height, width];
        }
        public Matrix(double[,] matrix)
        {
            _matrix = matrix.Clone() as double[,];

            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);
        }

        public double this[int i, int j]
        {
            get => _matrix[i, j];
            set => _matrix[i, j] = value;
        }


        public void RandomInit(double range = 1)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = (Rand.NextDouble() - 0.5) * 2 * range;
                }
            }
        }
        public void RandomInit(double min, double max)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = (Rand.NextDouble() - 0.5) * 2 * (max - min) + min;
                }
            }
        }
        public void Apply(Func<double, double> func)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = func(this[i, j]);
                }
            }
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1 == null) return m2;
            else if (m2 == null) return m1;
            else if (m2 == null && m1 == null) throw new NullReferenceException("Both matrices were null.");

            var sum = new Matrix(m1.Height, m1.Width);

            for (int i = 0; i < m1.Height; i++)
            {
                for (int j = 0; j < m1.Width; j++)
                {
                    sum[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return sum;
        }

        public void Add(IMatrix other)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] += other[i, j];
                }
            }
        }

        public override string ToString()
        {
            string s = string.Empty;
            for (int i = 0; i < Height; i++)
            {
                s += "|";
                for (int j = 0; j < Width; j++)
                {
                    s += $"{_matrix[i, j]}, ";
                }
                s += "|\n";
            }
            return s;
        }

        public void ZeroInit()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    this[i, j] = 0;
        }
    }
}
