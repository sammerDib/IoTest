using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitySC.PM.DMT.Shared.UI.Extensions
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
    }
}
