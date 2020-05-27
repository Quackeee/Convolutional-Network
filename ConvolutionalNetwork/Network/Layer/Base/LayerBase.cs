using System;
using System.Collections.Generic;
using System.Drawing;
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

        public NetworkLayer OutputLayer { get; protected set; }

        public int OutputDepth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public int OutputWidth { get; protected set; }

        public void ConnectOutput(NetworkLayer output)
        {
            OutputLayer = output;
        }
    }
    public abstract class HiddenLayer : NetworkLayer
    {
        protected Matrix3D _input => _inputLayer.Output;
        protected NetworkLayer _inputLayer;


        public bool IsConnected => _inputLayer != null;
        public Matrix3D Deltas { get; protected set; }
        public int InputDepth => _input.Depth;
        public int InputHeight => _input.Height;
        public int InputWidth => _input.Width;


        public abstract void CalculateOutput();
        public abstract void ConnectToInput(NetworkLayer inputLayer);
        public abstract void CalculateDeltas(Matrix3D previousDeltas);
    }
}
