using System;

using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;


namespace UnitySC.PM.DMT.Service.Flows.Shared
{
    public static class ImageUtils
    {
        public static ImageData CreateImageDataFromUSPImageMil(USPImageMil sourceImg)
        {
            var isMilSimulated = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().MilIsSimulated;
            if (isMilSimulated)
            {
                var imageData = new ImageData()
                {
                    ByteArray = new byte[] { 0 }
                };
                return imageData;
            }

            byte[] imageByteArray = sourceImg.ToByteArray();
            int sizeBit = sourceImg.GetMilImage().SizeBit;
            int sizeBand = sourceImg.GetMilImage().SizeBand;

            ImageType imageDataType;
            switch (sizeBit)
            {
                case 32 when sizeBand == 1:
                    imageDataType = ImageType.GRAYSCALE_Float32bits;
                    break;
                case 16 when sizeBand == 1:
                    imageDataType = ImageType.GRAYSCALE_Unsigned16bits;
                    break;
                case 8 when sizeBand == 1:
                    imageDataType = ImageType.GRAYSCALE_Unsigned8bits;
                    break;
                case 8 when sizeBand == 3:
                    imageDataType = ImageType.RGB_Unsigned8bits;
                    break;
                default:
                    throw new ArgumentException("No matching image type found.");
            }

            return new ImageData(imageByteArray, sourceImg.Width, sourceImg.Height, imageDataType);
        }
    }
}
