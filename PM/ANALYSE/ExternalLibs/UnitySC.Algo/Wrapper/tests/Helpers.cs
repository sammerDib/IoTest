using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using AlgosLibrary;

namespace UnitySC.Algorithms.Wrapper.Tests
{
    public static class Helpers
    {
        public static Bitmap ConvertToGrayScale(Bitmap original)
        {
            // create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            // get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            // create the grayscale ColorMatrix
            ColorMatrix colorMatrix =
                new ColorMatrix(new float[][] { new float[] { .3f, .3f, .3f, 0, 0 }, new float[] { .59f, .59f, .59f, 0, 0 },
                                          new float[] { .11f, .11f, .11f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 } });

            // create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            // set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            // draw the original image on the new image
            // using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel,
                        attributes);

            // dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static BareWaferAlignmentImageData CreateImageDataFromFile(string imagePath)
        {
            var img = new Bitmap(imagePath);
            var grayBitmap = ConvertToGrayScale(img);

            var imageData = new BareWaferAlignmentImageData(img.Width, img.Height) { Depth = 8 };

            var data = grayBitmap.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, grayBitmap.PixelFormat);

            int numberOfByteForOnePixel = 0;

            switch (grayBitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    numberOfByteForOnePixel = 4;
                    break;

                default:
                    throw new System.Exception("Cannot deduce number of byte per pixel");
            }

            unsafe
            {
                Parallel.For(0, (grayBitmap.Width * grayBitmap.Height), i =>
                {
                    byte firstByteOfPixel = *((byte*)IntPtr.Add(data.Scan0, i * numberOfByteForOnePixel));
                    imageData.ByteArray[i] = firstByteOfPixel;
                });
            }
            grayBitmap.UnlockBits(data);
            return imageData;
        }

        public static string CreateWrapperDataPath(string imgName)
        {
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string imgPath = projectDirectory + "\\Wrapper\\tests\\data\\" + imgName;

            return imgPath;
        }

        public static string CreateNativeLibAlgoDataPath(string imgName)
        {
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string imgPath = projectDirectory + "\\tests\\data\\" + imgName;

            return imgPath;
        }
    }
}
