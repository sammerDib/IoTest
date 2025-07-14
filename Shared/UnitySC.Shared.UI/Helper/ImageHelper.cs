using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace UnitySC.Shared.UI.Helper
{
    public static class ImageHelper
    {
        public static BitmapFrame ConvertToBitmapFrame(Bitmap bitmap, int size)
        {
            using (var imageStream = new MemoryStream())
            {
                var resized = new Bitmap(bitmap, new Size(size, size));
                resized.Save(imageStream, ImageFormat.Png);
                return BitmapFrame.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        public static BitmapImage FromBitmap(Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width,
                bitmapData.Height,
                bitmap.HorizontalResolution,
                bitmap.VerticalResolution,
                ConvertPixelFormat(bitmap.PixelFormat),
                null,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        private static PixelFormat ConvertPixelFormat(System.Drawing.Imaging.PixelFormat sourceFormat)
        {
            switch (sourceFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Bgra32;

                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    return PixelFormats.Gray16;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    return PixelFormats.Gray8;
            }
            return new PixelFormat();
        }

        /// <summary>
        /// Iterates through all the pixels of the image and calls the function passed as a parameter to obtain the color to apply.
        /// </summary>
        /// <param name="bitmap">Bitmap to complete</param>
        /// <param name="margin">Margin to apply around the area to be processed</param>
        /// <param name="getPixel">Function returning a color from X Y coordinates</param>
        public static void ProcessBitmap(Bitmap bitmap, int margin, Func<int, int, Color> getPixel)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            var ptrFirstPixel = bitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            int heightInPixels = bitmapData.Height - 2 * margin;
            int widthInBytes = (bitmapData.Width - 2 * margin) * bytesPerPixel;

            Parallel.For(0, heightInPixels, y =>
            {
                int currentLine = y * bitmapData.Stride;
                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    var color = getPixel(x / 4, y);
                    pixels[currentLine + x] = color.B;
                    pixels[currentLine + x + 1] = color.G;
                    pixels[currentLine + x + 2] = color.R;
                    pixels[currentLine + x + 3] = color.A;
                }
            });

            // Copy modified bytes back
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
