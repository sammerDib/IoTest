using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
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

        public static ImageData CreateImageDataFromFile(string imagePath)
        {
            var imageData = new ImageData();
            Stream imageStreamSource = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!imageStreamSource.CanRead)
            {
                imageStreamSource.Close();
                throw new Exception("Image cannot be read: " + imagePath);
            }

            string extension = Path.GetExtension(imagePath);

            BitmapDecoder decoder = null;
            switch (extension.ToLower())
            {
                case ".png":
                    decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".jpg":
                    decoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".bmp":
                    decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".tif":
                case ".tiff":
                    decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;
                default:
                    throw new Exception("unsupported file extension: " + Path.GetExtension(imagePath));
            }

            BitmapSource img = decoder.Frames[0];

            int bpp;

            if (img.Format == System.Windows.Media.PixelFormats.Gray8 || img.Format == System.Windows.Media.PixelFormats.Indexed8)
            {
                imageData.Type = ImageType.GRAYSCALE_Unsigned8bits;
                bpp = 8;
            }
            else if (img.Format == System.Windows.Media.PixelFormats.Gray16)
            {
                imageData.Type = ImageType.GRAYSCALE_Unsigned16bits;
                bpp = 16;
            }
            else if (img.Format == System.Windows.Media.PixelFormats.Gray32Float)
            {
                imageData.Type = ImageType.GRAYSCALE_Float32bits;
                bpp = 32;
            }
            else if (img.Format == System.Windows.Media.PixelFormats.Rgb24 || img.Format == System.Windows.Media.PixelFormats.Bgr24)
            {
                imageData.Type = ImageType.RGB_Unsigned8bits;
                bpp = 24;
            }
            else
            {
                throw new Exception("unsupported image format");
            }

            long stride = (img.PixelWidth * bpp + 7) / 8;
            long size = img.PixelHeight * stride;
            imageData.ByteArray = new byte[size];
            img.CopyPixels(imageData.ByteArray, (int)stride, 0);
            imageData.Width = img.PixelWidth;
            imageData.Height = img.PixelHeight;

            imageStreamSource.Close();
            return imageData;
        }

        public static BareWaferAlignmentImageData CreateBWAImageDataFromFile(string imagePath)
        {
            var img = new Bitmap(imagePath);
            var grayBitmap = ConvertToGrayScale(img);

            var imageData = new BareWaferAlignmentImageData(img.Width, img.Height) { Type = ImageType.GRAYSCALE_Unsigned8bits };

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
            string imgPath = projectDirectory + "\\Tests\\Data\\" + imgName;

            return imgPath;
        }

        public static string CreateNativeLibAlgoDataPath(string imgName)
        {
            // get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string imgPath = projectDirectory + "\\Tests\\Data\\" + imgName;

            return imgPath;
        }
    }
}
