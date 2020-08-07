using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalNetwork.Misc.GPUAccelleratied
{
    class GpuMatrix : IMatrix
    {
        public double this[int i, int j] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

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

        public void Add(IMatrix other)
        {
            throw new NotImplementedException();
        }

        public void ZeroInit()
        {
            throw new NotImplementedException();
        }
    }
}
