using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

using static UnitySC.PM.ANA.Service.Core.Shared.ShapeDetector;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public static class AlgorithmLibraryUtils
    {
        public static ServiceImage ConvertToGrayscaleServiceImage(ImageData imageData)
        {
            var serviceImg = new ServiceImage();

            //serviceImg.Data = new byte[imageData.Height * imageData.Width * (imageData.Depth / 8)];
            serviceImg.Data = new byte[imageData.ByteArray.Length];
            imageData.ByteArray.CopyTo(serviceImg.Data, 0);
            serviceImg.DataHeight = imageData.Height;
            serviceImg.DataWidth = imageData.Width;
            serviceImg.Type = ServiceImage.ImageType.Greyscale;

            return serviceImg;
        }

        public static ServiceImage ConvertTo3DAServiceImage(ImageData imageData, MatrixFloatFile.AdditionnalHeaderData headerAddData = null)
        {
            if (imageData.Type != ImageType.GRAYSCALE_Float32bits)
                throw new Exception("Imagedata depth is not 32 bit, impossible to convert");

            if ((imageData.ByteArray.Length % 4) != 0)
                throw new Exception("Imagedata bytearray is not aligned with float buffer, impossible to convert");

            var serviceImg = new ServiceImage();

            serviceImg.Type = ServiceImage.ImageType._3DA;
            serviceImg.DataHeight = imageData.Height;
            serviceImg.DataWidth = imageData.Width;

            //3da - matrix float data
            bool useCompression = true;
            var header = new MatrixFloatFile.HeaderData(imageData.Height, imageData.Width, headerAddData);
            // all additionnal data such as pixelsizeX&Y, unit X&Y&Z are still should be accessble in headerAddData if not null
            using (var mff = new MatrixFloatFile())
            {
                // we assume here that we handle monochunk buffer
                // some stuff need to be done if we want to handle multi chunk in case of very large buffer

                // convert byte array to float array
                uint lFloatDataSize = (uint)(imageData.ByteArray.Length / sizeof(float));
                var dataFloat = new float[lFloatDataSize];
                Buffer.BlockCopy(imageData.ByteArray, 0, dataFloat, 0, imageData.ByteArray.Length);

                serviceImg.Data = mff.WriteInMemory(header, dataFloat, useCompression);
            }

            return serviceImg;
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

        public static ImageType USPImageTypeToOpenCVImageType(ImageFormat imageType)
        {
            switch (imageType)
            {
                case ImageFormat.GreyLevel:
                    return ImageType.GRAYSCALE_Unsigned8bits;
                case ImageFormat.Height3D:
                    return ImageType.GRAYSCALE_Float32bits;
                default:
                    throw new ApplicationException("unknown image format");
            }
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
                refImgType = ImageType.GRAYSCALE_Float32bits;
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
            var imgType = USPImageTypeToOpenCVImageType(uspImage.Format);

            return new ImageData(imgByteArray, imgWidth, imgHeight, imgType);
        }

        public static ImageData CreateImageData(ExternalImage externalImage)
        {
            return CreateImageData(new ServiceImage(externalImage));
        }

        public static EllipseFinderParams CreateEllipseFinderParams(Length length, Length width, Length lengthTolerance, Length widthTolerance, Length pixelSize, int cannyThreshold)
        {
            double approximateMajorAxisPixel = length.Millimeters / pixelSize.Millimeters;
            double approximateMinorAxisPixel = width.Millimeters / pixelSize.Millimeters;
            double lengthTolerancePixel = lengthTolerance.Millimeters / pixelSize.Millimeters;
            double widthTolerancePixel = widthTolerance.Millimeters / pixelSize.Millimeters;

            var param = new EllipseFinderParams(new Tuple<double, double>(approximateMajorAxisPixel, approximateMinorAxisPixel), lengthTolerancePixel, widthTolerancePixel, cannyThreshold);
            return param;
        }

        public static CircleFinderParams CreateCircleFinderParams(Length diameter, Length diameterTolerance, Length pixelSize, int cannyThreshold, bool useScharrAlgorithm, bool useMorphologicalOperations)
        {
            double approximateDiameterPixel = diameter.Millimeters / pixelSize.Millimeters;
            double diameterTolerancePixel = diameterTolerance.Millimeters / pixelSize.Millimeters;

            var param = new CircleFinderParams(approximateDiameterPixel, approximateDiameterPixel, diameterTolerancePixel, cannyThreshold, useScharrAlgorithm, useMorphologicalOperations);
            return param;
        }

        public static MetroCDCircleInput CreateMetroCDCircleInput(ServiceImage img, CircleCentralFinderParams parameters)
        {
            double approximateRadiusPixel = 0.5 * parameters.Diameter.Micrometers / parameters.PixelSize.Micrometers;
            double radiusTolerancePixel = 0.5 * parameters.DiameterTolerance.Micrometers / parameters.PixelSize.Micrometers;

            var inputs = new MetroCDCircleInput(new Point2d(img.DataWidth * 0.5, img.DataHeight * 0.5), approximateRadiusPixel, radiusTolerancePixel);

            if (parameters.SeekerNumber.HasValue)
            {
                inputs.seekerNumber = parameters.SeekerNumber.Value;
            }
            if (parameters.SeekerWidth.HasValue)
            {
                inputs.seekerWidth = parameters.SeekerWidth.Value;
            }
            if (parameters.KernelSize.HasValue)
            {
                inputs.KernelSize = parameters.KernelSize.Value;
            }
            if (parameters.Mode.HasValue && Enum.IsDefined(typeof(MetroSeekerMode), parameters.Mode.Value))    
            {
                inputs.mode = (MetroSeekerMode)parameters.Mode.Value;
            }
            if (parameters.EdgeLocaliz.HasValue && Enum.IsDefined(typeof(MetroSeekerLocInEdge), parameters.EdgeLocaliz.Value))
            {
                inputs.EdgeLocalizePreference = (MetroSeekerLocInEdge)parameters.EdgeLocaliz.Value;
            }
            if (parameters.SigAnalysisThreshold.HasValue)
            {
                inputs.SigAnalysisThreshold = parameters.SigAnalysisThreshold.Value;
            }
            if (parameters.SigAnalysisPeakWindowSize.HasValue)
            { 
                inputs.SigAnalysisPeakWindowSize = parameters.SigAnalysisPeakWindowSize.Value; 
            }
            return inputs;
        }
      
        public static UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest CreateRegionOfInterest(ServiceImage img, Interface.Algo.RegionOfInterest roi, ImageParameters imgParams)
        {
            int refImgWidth = img.DataWidth;
            int refImgHeight = img.DataHeight;

            if (roi is null)
            {
                return new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = 0, Y = 0, Width = refImgWidth, Height = refImgHeight };
            }

            var PixelSizeX = imgParams.PixelSizeX;
            var PixelSizeY = imgParams.PixelSizeY;

            int roiX = (int)(roi.X.ToPixels(PixelSizeX));
            int roiY = (int)(roi.Y.ToPixels(PixelSizeY));
            int roiWidth = (int)(roi.Width.ToPixels(PixelSizeX));
            int roiHeight = (int)(roi.Height.ToPixels(PixelSizeY));

            var regionOfInterest = new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = roiX, Y = roiY, Width = roiWidth, Height = roiHeight };
            var isValid = CheckRegionOfInterestValidity(img, regionOfInterest);
            if (!isValid)
            {
                throw new Exception("The region of interest is outside the image.");
            }

            return regionOfInterest;
        }

        public static UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest CreateRegionOfInterest(ServiceImage img, CenteredRegionOfInterest roi, Length pxSize)
        {
            int refImgWidth = img.DataWidth;
            int refImgHeight = img.DataHeight;

            if (roi is null)
            {
                return new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = 0, Y = 0, Width = refImgWidth, Height = refImgHeight };
            }

            var PixelSizeX = pxSize;
            var PixelSizeY = pxSize;

            int roiWidth = (int)(roi.Width.ToPixels(PixelSizeX));
            int roiHeight = (int)(roi.Height.ToPixels(PixelSizeY));
            int roiX = refImgWidth / 2 - roiWidth / 2 + (int)(roi.OffsetX.ToPixels(PixelSizeX));
            int roiY = refImgHeight / 2 - roiHeight / 2 + (int)(roi.OffsetY.ToPixels(PixelSizeY));
            var regionOfInterest = new UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest { X = roiX, Y = roiY, Width = roiWidth, Height = roiHeight };

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

        public static List<PositionImageData> ConvertServiceImageWithPositionToPositionImageData(List<ServiceImageWithPosition> imagesList, PixelSize imageScale)
        {
            var positionImageDataList = new List<PositionImageData>();
            foreach (var image in imagesList)
            {
                var imagePosition = image.CenterPosition.ToXYPosition();
                positionImageDataList.Add(
                    new PositionImageData()
                    {
                        ByteArray = image.Image.Data,
                        Height = image.Image.DataHeight,
                        Width = image.Image.DataWidth,
                        Type = ServiceImageTypeToOpenCVImageType(image.Image.Type),
                        Scale = imageScale,
                        Centroid = new Point2d(imagePosition.X * 1000, imagePosition.Y * 1000)
                    });
            }
            return positionImageDataList;
        }

        public static double ComputeMeanPixel(ServiceImage img)
        {
            double meanpixel = 0.0;
            if (img != null && img.Data != null)
            {
                int bpp = 0; //bit per pixel
                switch (img.Type)
                {
                    case ServiceImage.ImageType.Greyscale:
                        bpp = 8;
                        break;
                    case ServiceImage.ImageType.Greyscale16Bit:
                        bpp = 16;
                        break;
                    case ServiceImage.ImageType.RGB:
                        bpp = 24;
                        break;
                    case ServiceImage.ImageType._3DA:
                        bpp = 32;
                        break;
                    default:
                        return 0.0;
                }

                var width = img.DataWidth;
                var height = img.DataHeight;
                var bytesPerPixel = (bpp + 7) / 8;
                var bufstride = width * bytesPerPixel;
                long sumtotal = 0;
                unsafe
                {
                    var totalPixels = width * height;
                    if (totalPixels <= 0)
                        return 0.0;

                    fixed (byte* pixels = img.Data)
                    {
                        byte* p = pixels;
                        if (bytesPerPixel == 1)
                        {
                            for (int k = 0; k < totalPixels; k++)
                            {
                                sumtotal += *p;
                                ++p;
                            }
                        }
                        else
                        {
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    sumtotal += p[(y * bufstride) + x * bytesPerPixel];
                                }
                            }
                        }
                    }
                    meanpixel = (double)sumtotal / (double)totalPixels;
                }
            }
            return meanpixel;
        }
    }
}
