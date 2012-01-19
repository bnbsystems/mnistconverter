using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MnistConverter
{
    public class MnistConverterEngine
    {

        private bool SaveList(IEnumerable<CharAndImage> vList, string folder)
        {
            try
            {
                string path = folder;
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                if (vList != null)
                {
                    CounterList<char> counter = new CounterList<char>();
                    foreach (var item in vList)
                    {
                        counter.Set(item.Char);
                        string fileName = (int)item.Char + "_" + counter.Get(item.Char) + ".bmp";
                        item.Image.Save(Path.Combine(path, fileName));
                    }
                    return true;
                }

            }
            catch 
            {

            }
            return false;
        }

        private List<Image> MakeImageList(List<int> pixelList, int numberOfImages, int columnNumber, int rowNumber)
        {
            List<Image> rImageList = new List<Image>();
            if (pixelList != null)
            {

                Bitmap vBitmap;
                int vPixelCounter = 0;
                while (true)
                {
                    bool setBreak = false;
                    vBitmap = new Bitmap(columnNumber, rowNumber);
                    for (int y = 0; y < rowNumber; y++)
                    {
                        for (int x = 0; x < columnNumber; x++)
                        {
                            if (vPixelCounter >= pixelList.Count)
                            {
                                setBreak = true;
                                break;
                            }
                            int colorUint = (int)pixelList[vPixelCounter];
                            if (colorUint < 0)
                            {
                                colorUint = 0;
                            }

                            Color newColor = Color.FromArgb(colorUint, colorUint, colorUint);
                            vBitmap.SetPixel(x, y, newColor);
                            vPixelCounter++;
                        }
                        if (setBreak)
                        {
                            break;
                        }

                    }

                    rImageList.Add(vBitmap);
                    if (setBreak)
                    {
                        vBitmap.Save("LastImage.bmp");
                        break;
                    }
                }
            }
            return rImageList;
        }


        private string GetLabelFile(string iPath, bool isTreaningData)
        {
            string vFileName = isTreaningData ? "train-labels.idx1-ubyte" : "t10k-labels.idx1-ubyte";
            return Path.Combine(iPath, vFileName);
        }
        private string GetImageFile(string iPath, bool isTreaningData)
        {
            string vFileName = isTreaningData ? "train-images.idx3-ubyte" : "t10k-images.idx3-ubyte";
            return Path.Combine(iPath, vFileName);
        }
        List<Image> imageBitmapList = new List<Image>();
        List<char> imageCharList = new List<char>();


        public bool ConvertMnist(string path, bool isTrainData)
        {
                string subFolder = "";
                string labelFile = GetLabelFile(path, isTrainData);
                string imageFile = GetImageFile(path, isTrainData);
                if (File.Exists(labelFile) && File.Exists(imageFile))
                {
                    #region getting image
                    using (StreamReader reader = new StreamReader(imageFile, Encoding.ASCII))
                    {

                        char[] buffer4 = new char[4];
                        char[] buffer1 = new char[1];

                        List<int> pixelList = new List<int>();

                        reader.Read(buffer4, 0, buffer4.Length);
                        var magivnumber = buffer4;

                        reader.Read(buffer4, 0, buffer4.Length);
                        int number1 = Convert.ToInt32(buffer4[buffer4.Length - 1]);
                        int number2 = Convert.ToInt32(buffer4[buffer4.Length - 2]);

                        int vNumberOfImages = int.Parse(number2.ToString() + number1.ToString());

                        reader.Read(buffer4, 0, buffer4.Length);
                        int vNumberOfRows = buffer4[buffer4.Length - 1];

                        reader.Read(buffer4, 0, buffer4.Length);
                        int vNumberOfColumns = buffer4[buffer4.Length - 1];


                        while (reader.Read(buffer1, 0, buffer1.Length) > 0)
                        {
                            pixelList.Add(buffer1[0] * 2);

                        }

                        imageBitmapList = MakeImageList(pixelList, vNumberOfImages, vNumberOfColumns, vNumberOfRows);
                    }
                    #endregion
                    #region getting labels
                    using (StreamReader reader = new StreamReader(labelFile, Encoding.Default))
                    {
                        char[] buffer4 = new char[4];
                        char[] buffer1 = new char[1];


                        reader.Read(buffer4, 0, buffer4.Length);
                        var magivnumber = buffer4;

                        reader.Read(buffer4, 0, buffer4.Length);
                        int number1 = Convert.ToInt32(buffer4[buffer4.Length - 1]);
                        int number2 = Convert.ToInt32(buffer4[buffer4.Length - 2]);

                        int vNumberOfImages = int.Parse(number2.ToString() + number1.ToString());

                        while (reader.Read(buffer1, 0, buffer1.Length) > 0)
                        {
                            imageCharList.Add(buffer1[0]);
                        }
                    }
                    #endregion

                    subFolder = Path.GetFileNameWithoutExtension(labelFile);

                    var vList = imageCharList.Zip(imageBitmapList, (c, i) => new CharAndImage { Image = i, Char = c });

                    var vFolder = Path.Combine(path, subFolder);
                    return SaveList(vList, vFolder);

                }
                return false;
        }
    }
}
