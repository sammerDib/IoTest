using System;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;



namespace UnitySC.PM.DMT.Service.Flows.Shared
{
    public static class ImageDataExt
    {
        public static USPImageMil ConvertToUSPImageMil(this ImageData imageData, bool convertTo8BitsDepth = true)
        {
            ServiceImage serviceImage;
            switch (imageData.Type)
            {
                case ImageType.GRAYSCALE_Float32bits:
                    if (convertTo8BitsDepth)
                    {
                        var sourceImageData = Converter.convertTo8UC1(imageData);
                        serviceImage = new ServiceImage() { Data = sourceImageData.ByteArray, DataHeight = sourceImageData.Height, DataWidth = sourceImageData.Width, Type = ServiceImage.ImageType.Greyscale };
                        return new USPImageMil(serviceImage);
                    }

                    int numberOfFloats = imageData.ByteArray.Length / 4;
                    float[] floatArray = new float[numberOfFloats];
                    Buffer.BlockCopy(imageData.ByteArray, 0, floatArray, 0, imageData.ByteArray.Length);

                    var result = new USPImageMil(floatArray, imageData.Width, imageData.Height,
                        MIL.M_FLOAT + 32, MIL.M_IMAGE + MIL.M_PROC);

                    return result;

                case ImageType.GRAYSCALE_Unsigned8bits:
                    serviceImage = new ServiceImage() { Data = imageData.ByteArray, DataHeight = imageData.Height, DataWidth = imageData.Width, Type = ServiceImage.ImageType.Greyscale };
                    return new USPImageMil(serviceImage);

                default:
                    throw new Exception("Invalid ImageData type, only Float32Bits or Unsigned8Bits supported");
            }
        }
    }
}
