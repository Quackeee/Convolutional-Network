using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    public abstract class NetworkLayer
    {
        protected IMatrix3D _output;
        public IMatrix3D Output { get => _output; }

        public int OutputDepth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public int OutputWidth { get; protected set; }
    }
    public abstract class HiddenLayer : NetworkLayer
    {
        protected IMatrix3D _input => _inputLayer.Output;
        protected NetworkLayer _inputLayer;
        

        protected bool IsConnected => _inputLayer != null;
        public IMatrix3D Deltas { get; protected set; }
        public int InputDepth => _inputLayer.OutputDepth;
        public int InputHeight => _inputLayer.OutputHeight;
        public int InputWidth => _inputLayer.OutputWidth;


        internal abstract void CalculateOutput();
        internal abstract void ConnectToInput(NetworkLayer inputLayer);
        internal abstract void PropagateDeltas(IMatrix3D previousDeltas);

        protected ActivationFunc _activation;
    }

    internal interface ITrainableLayer
    {
        void UpdateWeights();
        void StreamWeights(StreamWriter sw);
        void ReadWeights(StreamReader sr);
    }
}
