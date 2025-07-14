using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;

using UnitySCSharedAlgosOpenCVWrapper;

using ImageOperators = UnitySCSharedAlgosOpenCVWrapper.ImageOperators;
using RegionOfInterest = UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest;

namespace UnitySC.PM.EME.Service.Core.Recipe
{
    public class ImageProcessing
    {
        private readonly DistortionData _distortion;

        public ImageProcessing(DistortionData distortion)
        {
            _distortion = distortion;
        }

        private const int ThumbnailHeight = 100;

        public const double Scale = 0.25;

        public ServiceImage Process(HashSet<ImageProcessingType> settings, ServiceImage image)
        {
            if (settings.Contains(ImageProcessingType.NormalizePixelValue))
            {
                image = NormalizePixelValue(image);
            }

            if (settings.Contains(ImageProcessingType.ConvertTo8Bits))
            {
                image = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(image);
            }

            if (settings.Contains(ImageProcessingType.ReduceResolution))
            {
                image = Resize(image, Scale);
            }

            if (settings.Contains(ImageProcessingType.CorrectDistortion))
            {
                image = CorrectDistortion(image);
            }

            return image;
        }

        private ServiceImage NormalizePixelValue(ServiceImage image)
        {
            var openCvImage = AlgorithmLibraryUtils.CreateImageData(image);
            
            int median = ImageOperators.GrayscaleMedianComputation(openCvImage);
            double std = ImageOperators.StandardDeviation(openCvImage);
            
            int lowerIntensity = Math.Max((int)(median - 2 * std), 0);
            int maximumPixelValue = image.Type == ServiceImage.ImageType.Greyscale16Bit ? 65535 : 255;
            int upperIntensity = Math.Min((int)(median + 2 * std), maximumPixelValue);
            
            var normalizedImage = ImageOperators.SaturatedNormalization(openCvImage, lowerIntensity, upperIntensity);
            return AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(normalizedImage);
        }

        private ServiceImage CorrectDistortion(ServiceImage image)
        {
            var openCvImage = AlgorithmLibraryUtils.CreateImageData(image);
            var distortion = new DistortionCalibration.DistoMatrices(_distortion.NewOptimalCameraMat,
                _distortion.CameraMat, _distortion.DistortionMat, _distortion.RotationVec, _distortion.TranslationVec);
            var undistortedImage = DistortionCalibration.UndistortImage(openCvImage, distortion);
            return AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(undistortedImage);
        }

        private ServiceImage Resize(ServiceImage image, double scale)
        {
            var openCvImage = AlgorithmLibraryUtils.CreateImageData(image);
            var croppingRoi = new RegionOfInterest { X = 0, Y = 0, Width = 0, Height = 0 };
            var resizedOpenCVImage = ImageOperators.Resize(openCvImage, croppingRoi, scale);
            return AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(resizedOpenCVImage);
        }

        public static ServiceImage CreateThumbnail(ServiceImage image)
        {
            var image8bits = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(image);
            var imgData = AlgorithmLibraryUtils.CreateImageData(image8bits);
            var croppingRoi = new RegionOfInterest { X = 0, Y = 0, Width = 0, Height = 0 };
            double thumbnailScale = (double)ThumbnailHeight / imgData.Height;
            var thumbnailImgData = ImageOperators.Resize(imgData, croppingRoi, thumbnailScale);
            var thumbnail = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(thumbnailImgData);
            return thumbnail;
        }
    }
}
