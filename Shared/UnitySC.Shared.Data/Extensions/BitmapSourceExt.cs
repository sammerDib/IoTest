using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitySC.Shared.Data.Extensions
{
    public static class BitmapSourceExt
    {
        public static BitmapSource ConvertToRGB24(this BitmapSource bitmap)
        {
            ////////// Convert the BitmapSource to a new format ////////////
            // Use the BitmapImage created above as the source for a new BitmapSource object
            // which is set to a gray scale format using the FormatConvertedBitmap BitmapSource.
            // Note: New BitmapSource does not cache. It is always pulled when required.

            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();

            // BitmapSource objects like FormatConvertedBitmap can only have their properties
            // changed within a BeginInit/EndInit block.
            newFormatedBitmapSource.BeginInit();

            // Use the BitmapSource object defined above as the source for this new
            // BitmapSource (chain the BitmapSource objects together).
            newFormatedBitmapSource.Source = bitmap;

            // Set the new format to Gray32Float (grayscale).
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Rgb24;
            newFormatedBitmapSource.EndInit();

            return newFormatedBitmapSource;
        }
        public static byte[] ConvertToByteArray(this BitmapSource bitmap)
        {
            int bpp;
            if (bitmap.Format == PixelFormats.Gray8 || bitmap.Format == PixelFormats.Indexed8)
            {
                bpp = 8;
            }
            else if (bitmap.Format == PixelFormats.Gray16)
            {
                bpp = 16;
            }
            else if (bitmap.Format == PixelFormats.Rgb24 || bitmap.Format == PixelFormats.Bgr24)
            {
                bpp = 24;
            }
            else if (bitmap.Format == PixelFormats.Gray32Float || bitmap.Format == PixelFormats.Bgr32 || bitmap.Format == PixelFormats.Bgra32)
            {
                bpp = 32;
            }
            else
            {
                throw new Exception("unknown image format");
            }

            long stride = (bitmap.PixelWidth * bpp + 7) / 8;
            long size = bitmap.PixelHeight * stride;
            byte[] data = new byte[size];
            bitmap.CopyPixels(data, (int)stride, 0);

            return data;
        }
    }
}
