using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork.Misc.GPUAccelleratied
{
    class GpuMatrix3D : IMatrix3D
    {
        public IMatrix this[int k] => throw new NotImplementedException();

        public double this[int k, int i, int j] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Depth => throw new NotImplementedException();

        public Tuple<int, int, int> Dimensions => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public int Width => throw new NotImplementedException();

        public void Add(IMatrix3D other)
        {
            throw new NotImplementedException();
        }

        public void Apply(Func<double, double> func)
        {
            throw new NotImplementedException();
        }

        public void RandomInit(double range = 1)
        {
            throw new NotImplementedException();
        }

        public void RandomInit(double min, double max)
        {
            throw new NotImplementedException();
        }

        public void ZeroInit()
        {
            throw new NotImplementedException();
        }
    }
}
