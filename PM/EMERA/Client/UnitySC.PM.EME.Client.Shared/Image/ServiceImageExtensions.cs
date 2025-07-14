using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;


namespace UnitySC.PM.EME.Client.Shared.Image
{
    public static class ServiceImageExtensions
    {
        public static BitmapSource ToCachedBitmapSource(this ServiceImage image)
        {
            var format = GetPixelFormat(image.Type);
            int pitch = image.DataWidth * (image.Depth / 8);
            return BitmapSource.Create(image.DataWidth, image.DataHeight, 96, 96, format, null, image.Data, pitch);
        }

        private static PixelFormat GetPixelFormat(ServiceImage.ImageType type)
        {
            switch (type)
            {
                case ServiceImage.ImageType.Greyscale:
                    return PixelFormats.Gray8;
                case ServiceImage.ImageType.Greyscale16Bit:
                    return PixelFormats.Gray16;
                case ServiceImage.ImageType.RGB:
                    return PixelFormats.Rgb24;

                case ServiceImage.ImageType._3DA:
                    return PixelFormats.Gray8;
                default:
                    throw new ApplicationException("Unknown image format.");
            }
        }

        public static ImageData ToOpenCvImage(this ServiceImage image)
        {
            var imageType = GetOpenCvImageType(image.Type);
            return new ImageData(image.Data, image.DataWidth, image.DataHeight, imageType);
        }

        private static ImageType GetOpenCvImageType(ServiceImage.ImageType type)
        {

            switch (type)
            {
                case ServiceImage.ImageType.Greyscale:
                    return ImageType.GRAYSCALE_Unsigned8bits;
                case ServiceImage.ImageType.Greyscale16Bit:
                    return ImageType.GRAYSCALE_Unsigned16bits;
                case ServiceImage.ImageType.RGB:
                    return ImageType.RGB_Unsigned8bits;
                case ServiceImage.ImageType._3DA:
                    return ImageType.GRAYSCALE_Float32bits;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
