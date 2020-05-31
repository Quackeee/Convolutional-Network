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
        protected Matrix3D _output;
        public Matrix3D Output { get => _output; }

        public int OutputDepth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public int OutputWidth { get; protected set; }
    }
    public abstract class HiddenLayer : NetworkLayer
    {
        protected Matrix3D _input => _inputLayer.Output;
        protected NetworkLayer _inputLayer;


        public bool IsConnected => _inputLayer != null;
        public Matrix3D Deltas { get; protected set; }
        public int InputDepth => _inputLayer.OutputDepth;
        public int InputHeight => _inputLayer.OutputHeight;
        public int InputWidth => _inputLayer.OutputWidth;


        public abstract void CalculateOutput();
        public abstract void ConnectToInput(NetworkLayer inputLayer);
        public abstract void PropagateDeltas(Matrix3D previousDeltas);

    }

    public interface ITrainableLayer
    {
        void UpdateWeights();
        void StreamWeights(StreamWriter sw);
    }
}
