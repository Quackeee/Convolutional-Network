using ConvolutionalNetwork.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConvolutionalNetwork.Utils;

namespace ConvolutionalNetwork
{
    class LearningSet
    {
        private int _pictureHeight;
        private int _pictureWidth;
        private bool _rgb;
        private int _cathegoryCount;


        private List<Tuple<IMatrix3D, IMatrix3D>> _pictureValuePairs = new List<Tuple<IMatrix3D, IMatrix3D>>();

        public int Size { get => _pictureValuePairs.Count; }


        public LearningSet(string path, bool rgb = true)
        {
            _rgb = rgb;
            LoadSet(path);
        }

        public Tuple<IMatrix3D, IMatrix3D> GetPairAt(int index)
        {
            return _pictureValuePairs[index];
        }

        public Tuple<IMatrix3D, IMatrix3D> GetRandomPair()
        {
            return _pictureValuePairs[Rand.Next(_pictureValuePairs.Count)];
        }

        private void LoadSet(string path)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                var dir = new DirectoryInfo(fileInfo.FullName);
                var dirs = dir.GetDirectories();

                _cathegoryCount = dirs.Length;

                var exampleFile = new Bitmap(dirs[0].GetFiles()[0].FullName);

                _pictureWidth = exampleFile.Width;
                _pictureHeight = exampleFile.Height;


                var stopwatch = Stopwatch.StartNew();

                var fileLoadings = new List<Task>();

                for (int i = 0; i < dirs.Length; i++)
                {
                    fileLoadings.Add(FillPictureValuePairsAsync(dirs[i], i));
                }
                Debug.WriteLine($"Loading Finished: {stopwatch.ElapsedMilliseconds}");

                Task.WhenAll(fileLoadings).Wait();

            }
            else throw new NotImplementedException("Other methods for creating a learning set are not yet implemented");
        }

        private IMatrix3D FileToMatrix3D(FileInfo file)
        {
            var picture = new Bitmap(file.FullName);

            if (_pictureWidth != picture.Width || _pictureHeight != picture.Height)
                throw new InvalidOperationException($"Picture at {file.FullName} is not of proper size " +
                                                    $"should be {_pictureWidth}x{_pictureHeight}, " +
                                                    $"recieved {picture.Width}x{picture.Height}");


             return _rgb ? picture.AsMatrixRGB() : Factory.CreateMatrix3D(picture.AsMatrixGrayscale());
        }

        private async Task FillPictureValuePairsAsync(DirectoryInfo dir, int index)
        {
            var outputVector = CreateVersor(_cathegoryCount, index);
            var fileLoadings = new List<Task>();

            foreach (var file in dir.GetFiles())
            {
                fileLoadings.Add(
                    Task.Run(() =>
                   {
                       _pictureValuePairs.Add
                           (
                           Tuple.Create
                               (
                               FileToMatrix3D(file),
                               outputVector
                               )
                           );
                   }
                ));
            }
            await Task.WhenAll(fileLoadings);
        }


    }
}
