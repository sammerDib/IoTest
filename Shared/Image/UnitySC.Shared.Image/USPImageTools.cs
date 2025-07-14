using System;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Data.Extensions;

namespace UnitySC.Shared.Image
{
    public static class USPImageTools
    {
        public static int GetWidth(USPImage img)
        {
            return img.ConvertToWpfBitmapSource().PixelWidth;
        }

        public static int GetWidth(USPImageMil img)
        {
            return img.ConvertToWpfBitmapSource().PixelWidth;
        }

        public static int GetHeight(USPImage img)
        {
            return img.ConvertToWpfBitmapSource().PixelHeight;
        }

        public static int GetHeight(USPImageMil img)
        {
            return img.ConvertToWpfBitmapSource().PixelHeight;
        }

        public static int GetDepth(USPImage img)
        {
            switch (img.Format)
            {
                case ImageFormat.GreyLevel: return 8;
                case ImageFormat.Height3D: return 32;
                default:
                    throw new ApplicationException("unknown image format");
            }
        }

        public static int GetDepth(USPImageMil img)
        {
            switch (img.Format)
            {
                case ImageFormat.GreyLevel: return 8;
                case ImageFormat.Height3D: return 32;
                default:
                    throw new ApplicationException("unknown image format");
            }
        }

        public static byte[] GetByteArray(USPImage img)
        {
            BitmapSource bitmap = img.ConvertToWpfBitmapSource();
            return bitmap.ConvertToByteArray();
        }

        public static byte[] GetByteArray(USPImageMil img)
        {
            BitmapSource bitmap = img.ConvertToWpfBitmapSource();
            return bitmap.ConvertToByteArray();
        }
    }
}
