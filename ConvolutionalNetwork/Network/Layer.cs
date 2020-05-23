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

        public int OutputDepth { get; protected set; }
        public int OutputHeight { get; protected set; }
        public int OutputWidth { get; protected set; }
    }
    public abstract class HiddenLayer : NetworkLayer
    {
        protected Matrix3D _input => _inputLayer.Output;
        protected NetworkLayer _inputLayer;
        public bool IsConnected => _inputLayer != null;

        public abstract void CalculateOutput();
        public abstract void ConnectToInput(NetworkLayer inputLayer);
    }
    
    public class ConvLayer : HiddenLayer
    {
        private ConvNeuron[] _neurons;
        private int _kernelSize;

        public ConvLayer(int outputCount, int kernelSize = 5)
        {
            OutputDepth = outputCount;

            _kernelSize = kernelSize;
            _neurons = new ConvNeuron[outputCount];
            

            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new ConvNeuron(kernelSize);
            }
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputHeight = _inputLayer.OutputHeight - _kernelSize + 1;
            OutputWidth = _inputLayer.OutputWidth - _kernelSize + 1;

            foreach (var n in _neurons) n.ConnectToInput(_inputLayer);
        }

        public override void CalculateOutput()
        {
            if (IsConnected)
            {
                var output = new Matrix[OutputDepth];

                for (int i = 0; i < OutputDepth; i++)
                {
                    _neurons[i].CalculateOutput();
                    output[i] = _neurons[i].Output;
                }
                _output = new Matrix3D(output);

            }
            else throw new InvalidOperationException("The Layer is not connected to any input");
        }
    }
    public class MaxPoolLayer : HiddenLayer
    {
        private int _stride;

        public MaxPoolLayer(int stride)
        {
            _stride = stride;
        }

        public override void CalculateOutput()
        {
            var output = new Matrix[_input.Depth];

            for (int i = 0; i < _input.Depth; i++)
            {
                output[i] = MaxPool(_input[i], _stride);
            }

            _output = new Matrix3D(output);
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;

            OutputDepth = _inputLayer.OutputDepth;
            OutputHeight = _inputLayer.OutputHeight / _stride;
            OutputWidth = _inputLayer.OutputWidth / _stride;
        }
    }
    public class InputLayer : NetworkLayer
    {
        private bool _rgb;

        public void PushInput(Bitmap bm)
        {
            if (bm.Height != OutputHeight || bm.Width != OutputWidth) throw new ArgumentException("Bitmap is not of proper size");

            if (_rgb) _output = bm.AsMatrixRGB();
            else _output = bm.AsMatrixGrayscale();
        }

        public InputLayer(int height, int width, bool rgb = true)
        {
            _rgb = rgb;
            OutputHeight = height;
            OutputWidth = width;

            if (rgb) OutputDepth = 3;
            else OutputDepth = 1;
        }
    }

    public class FullConLayer : HiddenLayer
    {
        Neuron[] _neurons;
        

        public FullConLayer(int outputCount)
        {
            OutputDepth = 1;
            OutputHeight = outputCount;
            OutputWidth = 1;

            _neurons = new Neuron[outputCount];
            _output = new Matrix3D(1, outputCount, 1);

            for (int i = 0; i < outputCount; i++)
            {
                _neurons[i] = new Neuron();
            }
        }

        public override void CalculateOutput()
        {
            for (int i = 0; i < OutputHeight; i++)
            {
                _neurons[i].CalculateOutput();
                _output[0, i, 0] = _neurons[i].Output;
            }
        }

        public override void ConnectToInput(NetworkLayer inputLayer)
        {
            _inputLayer = inputLayer;
            foreach (var neuron in _neurons)
                neuron.ConnectToInput(_inputLayer);
        }
    }
}
