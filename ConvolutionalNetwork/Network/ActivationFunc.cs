﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace ConvolutionalNetwork
{
    public abstract class ActivationFunc
    {
        public abstract Action<Matrix3D> Run { get; }
        public abstract Func<double, double> Derivative { get; }
    }

    public class _ReLU : ActivationFunc
    {
        public override Action<Matrix3D> Run => (arg) => arg.Apply( (d) => d < 0 ? 0 : d );
        public override Func<double, double> Derivative => (arg) => arg < 0 ? 0 : 1;
    }

    public class _SoftMax : ActivationFunc
    {
        public override Action<Matrix3D> Run => 
            (arg) =>
            {
                Console.WriteLine(arg);
                double sum = 0;
                for (int k = 0; k < arg.Depth; k++)
                {
                    for (int i = 0; i < arg.Height; i++)
                    {
                        for (int j = 0; j < arg.Width; j++)
                        {
                            Console.WriteLine(Exp(arg[k, i, j]/100));
                            sum += Exp(arg[k, i, j]/100);
                        }
                    }
                }
                Console.WriteLine(sum);
                Console.WriteLine(arg[0,0,0]/sum);
                arg.Apply((d) => Exp(d/100)/sum);
            };

        public override Func<double, double> Derivative => throw new NotImplementedException();
    }

    public class _Sigmoid : ActivationFunc
    {
        public override Action<Matrix3D> Run =>
            (arg) => arg.Apply((d) => 1 / (1 + Exp(-d)));
            
            
            //(d) => 1/(1 + Exp(-d));
        public override Func<double, double> Derivative => (arg) => 0.5 /(Cosh(arg) + 1);
    }

    public static class ActivationFuncs
    {
        public static ActivationFunc Sigmoid = new _Sigmoid();
        public static ActivationFunc SoftMax = new _SoftMax();
        public static ActivationFunc ReLU = new _ReLU();
    }
}
