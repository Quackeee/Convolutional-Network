using System;
using System.Collections;
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
    public enum ReadingOrder
    {
        Sequential,
        Random
    }

    class LearningSet : IEnumerable<Tuple<Matrix3D, Matrix3D>>
    {
        private int _pictureHeight;
        private int _pictureWidth;
        private bool _rgb;
        public ReadingOrder ReadingOrder = ReadingOrder.Sequential;

        public int CathegoryCount { get; private set; }
        public int CathegorySize { get; private set; }
        public int Size { get; private set; }

        private List<Tuple<Matrix3D, Matrix3D>>[] _cathegories;
        
        public LearningSet(string path, bool rgb = true)
        {
            _rgb = rgb;
            LoadSet(path);
        }

        public Tuple<Matrix3D,Matrix3D> GetPairAt(int cathegory, int index)
        {
            return _cathegories[cathegory][index];
        }

        public Tuple<Matrix3D, Matrix3D> GetRandomPair()
        {
            return _cathegories[Rand.Next(CathegoryCount)][Rand.Next(CathegorySize)];
        }

        private void LoadSet(string path)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                var dir = new DirectoryInfo(fileInfo.FullName);
                var dirs = dir.GetDirectories();

                CathegoryCount = dirs.Length;
                CathegorySize = dirs[0].GetFiles().Length;

                var exampleFile = new Bitmap(dirs[0].GetFiles()[0].FullName);

                _pictureWidth = exampleFile.Width;
                _pictureHeight = exampleFile.Height;

                _cathegories = new List<Tuple<Matrix3D, Matrix3D>>[CathegoryCount];

                //var stopwatch = Stopwatch.StartNew();

                var fileLoadings = new List<Task>();

                for (int i = 0; i < dirs.Length; i++)
                {
                    fileLoadings.Add(FillPictureValuePairsAsync(dirs[i], i));
                }
                //Debug.WriteLine($"Loading Finished: {stopwatch.ElapsedMilliseconds}");

                Task.WhenAll(fileLoadings).Wait();

            }
            else throw new ArgumentException("Please provide a valid path to a directory");
        }

        private Matrix3D FileToMatrix3D(FileInfo file)
        {
            var picture = new Bitmap(file.FullName);

            if (_pictureWidth != picture.Width || _pictureHeight != picture.Height)
                throw new InvalidOperationException($"Picture at {file.FullName} is not of proper size " +
                                                    $"should be {_pictureWidth}x{_pictureHeight}, " +
                                                    $"recieved {picture.Width}x{picture.Height}");


             return _rgb ? picture.AsMatrixRGB() : picture.AsMatrixGrayscale();
        }

        private async Task FillPictureValuePairsAsync(DirectoryInfo dir, int index)
        {
            var outputVector = CreateVersor(CathegoryCount, index);
            var fileLoadings = new List<Task>();

            _cathegories[index] = new List<Tuple<Matrix3D, Matrix3D>>();


            Size += dir.GetFiles().Length;

            foreach (var file in dir.GetFiles())
            {
                fileLoadings.Add(
                    Task.Run(() =>
                   {
                       _cathegories[index].Add
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

        public IEnumerator<Tuple<Matrix3D, Matrix3D>> GetEnumerator()
        {
            if (ReadingOrder == ReadingOrder.Sequential)
            {
                int currentCathegory = 0;
                int currentElement = 0;

                while (true)
                {
                    //Console.WriteLine($"{currentCathegory}, {currentElement}");
                    yield return GetPairAt(currentCathegory, currentElement);

                    if (currentCathegory == CathegoryCount - 1)
                    {
                        if (currentElement == CathegorySize - 1)
                        {
                            yield break;
                        }
                        else
                        {
                            currentCathegory = 0;
                            currentElement++;
                        }
                    }
                    else currentCathegory++;
                }
            }
            else
            {
                while (true)
                {
                    foreach (var cathegory in _cathegories)
                        foreach (var pair in cathegory) yield return pair;
                    yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
