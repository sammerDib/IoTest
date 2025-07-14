using System;

using UnitySC.Shared.Image;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public static class AlgorithmLibraryUtils
    {
        public static ServiceImage Convert16BitServiceImageTo8Bit(ServiceImage imgToConvert)
        {
            switch (imgToConvert.Type)
            {
                case ServiceImage.ImageType.Greyscale:
                    return imgToConvert;

                case ServiceImage.ImageType.Greyscale16Bit:
                {
                    var convertedImg = new ServiceImage();
                    convertedImg.Data = new byte[imgToConvert.Data.Length / 2];
                    for (int i = 0; i < imgToConvert.Data.Length; i += 2)
                    {
                        ushort bytesTo16Bit = BitConverter.ToUInt16(imgToConvert.Data, i);
                        byte convertedByte = (byte)((bytesTo16Bit / 65535.0) * 255);
                        convertedImg.Data[i / 2] = convertedByte;
                    }
                    convertedImg.DataWidth = imgToConvert.DataWidth;
                    convertedImg.DataHeight = imgToConvert.DataHeight;
                    convertedImg.Type = ServiceImage.ImageType.Greyscale;

                    return convertedImg;
                }

                default:
                    throw new ApplicationException("A 16 Bit image format is needed");
            }
        }

        public static ServiceImage.ImageType OpenCVImageTypeServiceImageType(ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.GRAYSCALE_Unsigned8bits:
                    return ServiceImage.ImageType.Greyscale;
                case ImageType.GRAYSCALE_Unsigned16bits:
                    return ServiceImage.ImageType.Greyscale16Bit;
                case ImageType.RGB_Unsigned8bits:
                    return ServiceImage.ImageType.RGB;
                case ImageType.GRAYSCALE_Float32bits:
                    return ServiceImage.ImageType._3DA;
                default:
                    throw new ApplicationException("unknown image format");
            }
        }

        public static ServiceImage ConvertToGrayscaleServiceImage(ImageData imageData)
        {
            var serviceImg = new ServiceImage();

            //serviceImg.Data = new byte[imageData.Height * imageData.Width * (imageData.Depth / 8)];
            serviceImg.Data = new byte[imageData.ByteArray.Length];
            imageData.ByteArray.CopyTo(serviceImg.Data, 0);
            serviceImg.DataHeight = imageData.Height;
            serviceImg.DataWidth = imageData.Width;
            serviceImg.Type = OpenCVImageTypeServiceImageType(imageData.Type);

            return serviceImg;
        }

        public static ImageData CreateImageData(ServiceImage serviceImage)
        {
            var refImgHeight = serviceImage.DataHeight;
            var refImgWidth = serviceImage.DataWidth;
            var refImgType = ServiceImageTypeToOpenCVImageType(serviceImage.Type);
            byte[] refImgByteArray = null;
            if (serviceImage.Type != ServiceImage.ImageType._3DA)
            {
                refImgByteArray = new byte[serviceImage.DataHeight * serviceImage.DataWidth * (serviceImage.Depth / 8)];
                if (refImgByteArray.Length != serviceImage.Data.Length)
                    throw new Exception("CreateImageData byte array not correctly aligned with serviceimage data");
                serviceImage.Data.CopyTo(refImgByteArray, 0);
            }
            else
            {
                //3da - matrix float data
                using (var mff = new MatrixFloatFile(serviceImage.Data))
                {
                    // we assume we handle only moni chunk - some stuff still need to be done in case of very large buffers
                    float[] MonoBuffer = MatrixFloatFile.AggregateChunks(mff.GetChunkStatus(), mff);
                    refImgByteArray = new byte[MonoBuffer.Length * sizeof(float)];
                    Buffer.BlockCopy(MonoBuffer, 0, refImgByteArray, 0, refImgByteArray.Length);
                }
            }
            return new ImageData(refImgByteArray, refImgWidth, refImgHeight, refImgType);
        }

        public static ImageData CreateImageData(USPImage uspImage)
        {
            var imgByteArray = USPImageTools.GetByteArray(uspImage);
            var imgHeight = USPImageTools.GetHeight(uspImage);
            var imgWidth = USPImageTools.GetWidth(uspImage);
            var imgDepth = USPImageTools.GetDepth(uspImage);
            var imgType = USPImageDepthToOpenCVImageType(imgDepth);

            return new ImageData(imgByteArray, imgWidth, imgHeight, imgType);
        }

        public static ImageData CreateImageData(USPImageMil uspImage)
        {
            var imgByteArray = USPImageTools.GetByteArray(uspImage);
            var imgHeight = USPImageTools.GetHeight(uspImage);
            var imgWidth = USPImageTools.GetWidth(uspImage);
            var imgDepth = USPImageTools.GetDepth(uspImage);
            var imgType = USPImageDepthToOpenCVImageType(imgDepth);

            return new ImageData(imgByteArray, imgWidth, imgHeight, imgType);
        }

        public static ImageData CreateImageData(ExternalImage externalImage)
        {
            return CreateImageData(new ServiceImage(externalImage));
        }

        public static ImageType ServiceImageTypeToOpenCVImageType(ServiceImage.ImageType imageType)
        {
            switch (imageType)
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
                    throw new ApplicationException("unknown image format");
            }
        }

        public static ImageType USPImageDepthToOpenCVImageType(int uspImageDepth)
        {
            switch (uspImageDepth)
            {
                case 8:
                    return ImageType.GRAYSCALE_Unsigned8bits;
                case 16:
                    return ImageType.GRAYSCALE_Unsigned16bits;
                case 32:
                    return ImageType.GRAYSCALE_Float32bits;
                default:
                    throw new ApplicationException("unknown image format");
            }
        }

        public static UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest CreateRegionOfInterest(ServiceImage img, Interface.Algo.RegionOfInterest roi, Length pixelSize)
        {
            int refImgWidth = img.DataWidth;
            int refImgHeight = img.DataHeight;

            if (roi is null || roi?.X == null || roi?.Y == null || roi?.Width == null || roi?.Height == null)
            {
                return new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = 0, Y = 0, Width = refImgWidth, Height = refImgHeight };
            }

            int roiX = (int)(roi.X.ToPixels(pixelSize));
            int roiY = (int)(roi.Y.ToPixels(pixelSize));
            int roiWidth = (int)(roi.Width.ToPixels(pixelSize));
            int roiHeight = (int)(roi.Height.ToPixels(pixelSize));

            var regionOfInterest = new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = roiX, Y = roiY, Width = roiWidth, Height = roiHeight };
            var isValid = CheckRegionOfInterestValidity(img, regionOfInterest);
            if (!isValid)
            {
                throw new Exception("The region of interest is outside the image.");
            }

            return regionOfInterest;
        }

        private static bool CheckRegionOfInterestValidity(ServiceImage image, UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest roi)
        {
            bool topLeftCornerIsOutside = roi.X < 0 || roi.Y < 0;
            bool bottomRightCornerIsOutside = roi.X + roi.Width > image.DataWidth || roi.Y + roi.Height > image.DataHeight;
            if (topLeftCornerIsOutside || bottomRightCornerIsOutside)
            {
                return false;
            }

            return true;
        }
    }
}
