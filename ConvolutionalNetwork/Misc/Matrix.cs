﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{

    public class Matrix
    {
        public int Width;
        public int Height;

        private double[,] _matrix;
        public double this[int i, int j]
        {
            get => _matrix[i, j];
            set => _matrix[i, j] = value;
        }
        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;

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
        public void Apply(Func<double,double> func)
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

        public void Sum(Matrix other)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = other[i, j];
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

        internal void ZeroInit()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    this[i, j] = 0;
        }
    }

    public class Matrix3D
    {
        private Matrix[] _matrices;
        public Tuple<int, int, int> Dimensions => new Tuple<int, int, int>(Depth, Height, Width);

        public int Width => _matrices[0].Width;
        public int Height => _matrices[0].Height;
        public int Depth => _matrices.Length;

        public double this[int k, int i, int j]
        {
            get => _matrices[k][i, j];
            set => _matrices[k][i, j] = value;
        }
        public Matrix this[int k]
        {
            get
            {
                if (k >= Depth) throw new IndexOutOfRangeException($"Depth was {Depth} recieved {k}");
                return _matrices[k];
            }
        }

        public Matrix3D(int depth, int height, int width)
        {
            _matrices = new Matrix[depth];
            for (int i = 0; i < depth; i++)
            {
                _matrices[i] = new Matrix(height, width);
            }
        }
        public Matrix3D(params Matrix[] matrices)
        {
            if (matrices.Length == 0) throw new ArgumentException("There must be at least one Matrix provided");

            _matrices = matrices;
        }
        public Matrix3D(Tuple<int, int, int> dimensions) : this(dimensions.Item1, dimensions.Item2, dimensions.Item3)
        {
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
        public void ZeroInit()
        {
            for (int k = 0; k < Depth; k++)
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                        this[k, i, j] = 0;
        }
        public void Apply(Func<double,double> func)
        {
            //var sw = Stopwatch.StartNew();

            /*
            Parallel.For(0, Depth, k =>
                {
                    for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                            this[k, i, j] = func(this[k, i, j]);
                }
            );
            */

            for (int k = 0; k < Depth; k++)
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                        this[k, i, j] = func(this[k, i, j]);
            
            //Debug.WriteLine($"Apply time: {sw.ElapsedMilliseconds}");
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

        public void Add(Matrix3D other)
        {
            for (int k = 0; k < Depth; k++)
                for (int i = 0; i < Height; i++)
                    for (int j = 0; j < Width; j++)
                        this[k, i, j] += other[k, i, j];
        }


        public static implicit operator Matrix3D(Matrix m) => new Matrix3D(m);
        public static implicit operator Matrix[](Matrix3D m) => m._matrices;

        public override string ToString()
        {
            string s = string.Empty;
            for (int k = 0; k < Depth; k++)
            {
                s += "\n";
                for (int i = 0; i < Height; i++)
                {
                    s += "|";
                    for (int j = 0; j < Width; j++)
                    {
                        s += string.Format("{0:0.00}, ", this[k,i,j]);
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
