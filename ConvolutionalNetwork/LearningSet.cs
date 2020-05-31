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
    class LearningSet
    {
        private List<Tuple<Matrix3D, Matrix3D>> PictureValuePairs = new List<Tuple<Matrix3D, Matrix3D>>();

        public int Size { get => PictureValuePairs.Count; }


        public LearningSet(string path, bool rgb = true)
        {
            var fileInfo = new FileInfo(path);
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                var dir = new DirectoryInfo(fileInfo.FullName);
                var dirs = dir.GetDirectories();

                var outputs = new Matrix3D[dirs.Length];

                for (int i = 0; i < dirs.Length; i++)
                {
                    outputs[i] = new Matrix3D(1, dirs.Length, 1);
                    outputs[i].ZeroInit();
                    outputs[i][0, i, 0] = 1;
                    
                }

                var exampleFile = new Bitmap(dirs[0].GetFiles()[0].FullName);
                int width = exampleFile.Width;
                int height = exampleFile.Height;

                for (int i = 0; i < dirs.Length; i++)
                {
                    foreach (var file in dirs[i].GetFiles())
                    {
                        var picture = new Bitmap(file.FullName);
                        //Console.WriteLine(file.FullName + "\n" +outputs[i]);

                        if (width != picture.Width || height != picture.Height)
                            throw new InvalidOperationException($"Picture at {file.FullName} is not of proper size " +
                                                                $"should be {width}x{height}, " +
                                                                $"recieved {picture.Width}x{picture.Height}");


                        PictureValuePairs.Add
                            (
                            rgb?
                            new Tuple<Matrix3D, Matrix3D>(picture.AsMatrixRGB(), outputs[i]):
                            new Tuple<Matrix3D, Matrix3D>(picture.AsMatrixGrayscale(), outputs[i])
                            );
                    }
                }
            }
        }

        public Tuple<Matrix3D,Matrix3D> GetPairAt(int index)
        {
            return PictureValuePairs[index];
        }

        public Tuple<Matrix3D, Matrix3D> GetRandomPair()
        {
            return PictureValuePairs[Rand.Next(PictureValuePairs.Count)];
        }
    }
}
