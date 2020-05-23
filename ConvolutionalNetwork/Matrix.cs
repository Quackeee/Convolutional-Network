﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{

    public class Matrix
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
            _matrix = matrix.Clone() as double [,] ;

            Height = matrix.GetLength(0);
            Width = matrix.GetLength(1);
        }
        public Matrix(Matrix matrix)
        {
            _matrix = matrix._matrix.Clone() as double[,];
            Width = matrix.Width;
            Height = matrix.Height;
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
    
    }

    public class Matrix3D
    {
        private Matrix[] _matrices;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public double this[int k, int i, int j]
        {
            get => _matrices[k][i, j];
            set => _matrices[k][i, j] = value;
        }
        public Matrix this[int k]
        {
            get => _matrices[k];
        }

        public Matrix3D(int depth, int height, int width)
        {
            Depth = depth;
            Height = height;
            Width = width;

            _matrices = new Matrix[Depth];
            for (int i = 0; i < depth; i++)
            {
                _matrices[i] = new Matrix(height, width);
            }

        }
        public Matrix3D(params Matrix[] matrices)
        {
            if (matrices.Length == 0) throw new ArgumentException("There must be at least one Matrix provided");

            _matrices = matrices;

            Depth = matrices.Length;
            Width = matrices[0].Width;
            Height = matrices[0].Height;
        }

        public void RandomInit(double range = 1)
        {
            for (int k = 0; k < Depth; k++)
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                        this[k, i, j] = (Rand.NextDouble() - 0.5) * 2 * range;
        }
        public void RandomInit(double min, double max)
        {
            for (int k = 0; k < Depth; k++)
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                        this[k, i, j] = (Rand.NextDouble() - 0.5) * 2 * (max - min) + min;
        }

        public static Matrix3D operator +(Matrix3D m1, Matrix3D m2)
        {
            if (m1 == null) return m2;
            else if (m2 == null) return m1;
            else if (m2 == null && m1 == null) throw new NullReferenceException("Both matrices were null.");

            var sum = new Matrix3D(m1.Depth, m1.Height, m1.Width);

            for (int k = 0; k < m1.Depth; k++)
                for (int i = 0; i < m1.Height; i++)
                    for (int j = 0; j < m1.Width; j++)
                        sum[k, i, j] = m1[k, i, j] + m2[k, i, j];
            return sum;
        }
        public static implicit operator Matrix3D(Matrix m) => new Matrix3D(m);
        public static implicit operator Matrix[](Matrix3D m) => m._matrices;

        public override string ToString()
        {
            string s = string.Empty;
            for (int k = 0; k < Depth; k++)
            {
                for (int i = 0; i < Height; i++)
                {
                    s += "|";
                    for (int j = 0; j < Width; j++)
                    {
                        s += $"{this[k, i, j]}, ";
                    }
                    s += "|\n";
                }
                s += "---";
            }
            s.Remove(s.Length - 3, 3);
            s += "___________________";
            return s;
        }
    }
}
