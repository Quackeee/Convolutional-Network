using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork
{
    class Matrix
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public Tuple<int, int, int> Dimensions { get => new Tuple<int, int, int>(Width, Height, Depth); }

        private double[,,] _matrix;
        public double this[int k, int i, int j]
        {
            get => _matrix[k, i, j];
            set => _matrix[k, i, j] = value;
        }
        public Matrix(int width, int height, int depth)
        {
            Width = width;
            Height = height;
            Depth = depth;

            _matrix = new double[depth, height, width];
        }
        public Matrix(double[,,] matrix)
        {
            _matrix = matrix;

            Depth = matrix.GetLength(0);
            Height = matrix.GetLength(1);
            Width = matrix.GetLength(2);
        }

        public void RandomInit(double range)
        {
            var random = new Random();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        this[i, j, k] = (random.NextDouble() - 0.5) * 2 * range;
                    }
                }
            }
        }
        public void RandomInit(double min, double max)
        {
            var random = new Random();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        this[i, j, k] = (random.NextDouble() - 0.5) * 2 * (max-min) + min;
                    }
                }
            }
        }

        public override string ToString()
        {
            string s = String.Empty;
            for (int k = 0; k < Depth; k++)
            {
                for (int i = 0; i < Height; i++)
                {
                    s += "|";
                    for (int j = 0; j < Width; j++)
                    {
                        s += $"{_matrix[k, i, j]}, ";
                    }
                    s += "|\n";
                }
                s += @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + '\n';
            }
            return s;
        }

    }
}
