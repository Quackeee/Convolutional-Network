using ConvolutionalNetwork.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace ConvolutionalNetwork
{
    public abstract class ActivationFunc 
    {
        internal abstract Action<IMatrix3D> Run { get; }
        internal abstract IMatrix3D RecalculateDeltas(IMatrix3D oldDeltas, IMatrix3D outputs);
    }

    internal class _ReLU : ActivationFunc
    {
        internal override Action<IMatrix3D> Run => (arg) => arg.Apply( (d) => d < 0 ? 0 : d );

        internal override IMatrix3D RecalculateDeltas(IMatrix3D oldDeltas, IMatrix3D outputs)
        {
            var newDeltas = Factory.CreateMatrix3D(oldDeltas.Dimensions);

            for (int k = 0; k < oldDeltas.Depth; k++)
                for (int i = 0; i < oldDeltas.Height; i++)
                    for (int j = 0; j < oldDeltas.Width; j++)
                        newDeltas[k, i, j] = outputs[k, i, j] > 0 ? oldDeltas[k, i, j] : 0;

            return newDeltas;
        }
    }

    internal class _SoftMax : ActivationFunc
    {
        internal override Action<IMatrix3D> Run => 
            (arg) =>
            {
                double sum = 0;
                for (int k = 0; k < arg.Depth; k++)
                {
                    for (int i = 0; i < arg.Height; i++)
                    {
                        for (int j = 0; j < arg.Width; j++)
                        {
                            sum += Exp(arg[k, i, j]/100);
                        }
                    }
                }
                arg.Apply((d) => Exp(d/100)/sum);
            };

        internal override IMatrix3D RecalculateDeltas(IMatrix3D oldDeltas, IMatrix3D outputs)
        {
            var newDeltas = Factory.CreateMatrix3D(oldDeltas.Depth, oldDeltas.Height, oldDeltas.Width);

            for (int k = 0; k < oldDeltas.Depth; k++)
                for (int i = 0; i < oldDeltas.Height; i++)
                    for (int j = 0; j < oldDeltas.Width; j++)
                    {
                        newDeltas[k, i, j] = oldDeltas[k,i,j] * outputs[k,i,j];
                        for (int k2 = 0; k2 < oldDeltas.Depth; k2++)
                            for (int i2 = 0; i2 < oldDeltas.Height; i2++)
                                for (int j2 = 0; j2 < oldDeltas.Width; j2++)
                                    newDeltas[k, i, j] += oldDeltas[k,i,j] * outputs[k, i, j] * (1 - outputs[k2, i2, j2]);
                    }

            return newDeltas;
        }
    }

    internal class _Sigmoid : ActivationFunc
    {

        internal override Action<IMatrix3D> Run =>
            (arg) => arg.Apply((d) => 1 / (1 + Exp(-d)));

        internal override IMatrix3D RecalculateDeltas(IMatrix3D oldDeltas, IMatrix3D outputs)
        {
            var newDeltas = Factory.CreateMatrix3D(oldDeltas.Depth, oldDeltas.Height, oldDeltas.Width);

            for (int k = 0; k < oldDeltas.Depth; k++)
                for (int i = 0; i < oldDeltas.Height; i++)
                    for (int j = 0; j < oldDeltas.Width; j++)
                        newDeltas[k, i, j] = oldDeltas[k, i, j] * outputs[k, i, j] * (1 - outputs[k, i, j]);

            return newDeltas;
        }
    }
        
    public static class ActivationFuncs 
    {
        public static ActivationFunc Sigmoid = new _Sigmoid();
        public static ActivationFunc SoftMax = new _SoftMax();
        public static ActivationFunc ReLU = new _ReLU();
    }
}
