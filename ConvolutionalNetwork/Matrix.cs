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

        private double[,] _matrix;
        public double this[int i, int j]
        {
            get => _matrix[i, j];
            set => _matrix[i, j] = value;
        }
        public Matrix(int height, int width)
        {
            Width = width;
            Height = height;

            _matrix = new double[height, width];
        }
        public Matrix(double[,] matrix)
        {
            _matrix = matrix;

            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);
        }

        public void RandomInit(double range)
        {
            var random = new Random();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = (random.NextDouble() - 0.5) * 2 * range;
                }
            }
        }
        public void RandomInit(double min, double max)
        {
            var random = new Random();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = (random.NextDouble() - 0.5) * 2 * (max-min) + min;
                }
            }
        }

        public override string ToString()
        {
            string s = String.Empty;
            for (int i = 0; i < Height; i++)
            {
                s += "|";
                for (int j = 0; j < Width; j++)
                {
                    s += $"{_matrix[i,j]}, ";
                }
                s += "|\n";
            }
            return s;
        }
            
    }
}
