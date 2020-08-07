using System;

namespace ConvolutionalNetwork
{
    public interface IMatrix3D
    {
        public int Depth { get; }
        public int Width { get; }
        public int Height { get; }


        IMatrix this[int k] { get; }
        double this[int k, int i, int j] { get; set; }
        Tuple<int, int, int> Dimensions { get; }

        void Add(IMatrix3D other);
        void Apply(Func<double, double> func);
        void RandomInit(double range = 1);
        void RandomInit(double min, double max);
        string ToString();
        void ZeroInit();
    }
}