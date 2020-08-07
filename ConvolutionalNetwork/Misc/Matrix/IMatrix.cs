using System;

namespace ConvolutionalNetwork
{
    public interface IMatrix
    {
        public int Width { get; }
        public int Height { get; }
        double this[int i, int j] { get; set; }

        void Apply(Func<double, double> func);
        void RandomInit(double range = 1);
        void RandomInit(double min, double max);
        void Add(IMatrix other);
        string ToString();
        void ZeroInit();
    }
}