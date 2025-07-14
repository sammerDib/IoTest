using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

using RegionOfInterest = UnitySCSharedAlgosOpenCVWrapper.RegionOfInterest;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public class ImageRegistration
    {
        public virtual PatternRecResult Compute(RegistrationData registrationParameters, Length pixelSize,
            string reportPath, bool writeReportEnabled)
        {
            var refImgPreprocessedData =
                AlgorithmLibraryUtils.CreateImageData(registrationParameters.RefImgPreprocessed);
            var sensedImgPreprocessedData =
                AlgorithmLibraryUtils.CreateImageData(registrationParameters.SensedImgPreprocessed);
            var regionOfInterest = AlgorithmLibraryUtils.CreateRegionOfInterest(registrationParameters.RefImg,
                registrationParameters.ROI, pixelSize);
            
            var result = Registration.ImgRegistration(refImgPreprocessedData, sensedImgPreprocessedData, regionOfInterest,
                registrationParameters.AngleTolerance, registrationParameters.ScaleTolerance,
                registrationParameters.DilationMaskSize, !writeReportEnabled ? "" : reportPath);

            var patternRecResult = new PatternRecResult
            {
                ShiftX = new Length(pixelSize.Millimeters * result.PixelShiftX, LengthUnit.Millimeter),
                ShiftY = new Length(pixelSize.Millimeters * -result.PixelShiftY, LengthUnit.Millimeter) //converting from image referential to wafer referential reverses the y axis
            };

            var refImgData = AlgorithmLibraryUtils.CreateImageData(registrationParameters.RefImg);
            var sensedImgData = AlgorithmLibraryUtils.CreateImageData(registrationParameters.SensedImg);

            var realignedImage = Registration.RealignImages(refImgData, sensedImgData, result);
            patternRecResult.Confidence = result.Confidence >= 0 ? result.Confidence : 0;
            patternRecResult.ControlImage = CreateRealignmentCheckImage(registrationParameters.RefImg.Data,
                realignedImage.ByteArray, registrationParameters.RefImg.DataHeight,
                registrationParameters.RefImg.DataWidth, regionOfInterest);

            return patternRecResult;
        }

        private static ServiceImage CreateRealignmentCheckImage(byte[] refImg, byte[] registeredImg, int height,
            int width,
            RegionOfInterest roi)
        {
            var controlImg = new ServiceImage
            {
                Type = ServiceImage.ImageType.RGB, DataHeight = height, DataWidth = width
            };

            var hybridImg = new List<byte>();

            for (int row = 0; row < height; row++)
            {
                bool isRoiY = row == roi.Y || row == roi.Y + roi.Height;
                bool isRoiYB = row + 1 == roi.Y || row + 1 == roi.Y + roi.Height;
                for (int col = 0; col < width; col++)
                {
                    int i = width * row + col;
                    bool isInsideRoi = row >= roi.Y && col >= roi.X && row <= roi.Y + roi.Height &&
                                       col <= roi.X + roi.Width;
                    bool isInsideRoiB = row + 1 >= roi.Y && col + 1 >= roi.X && row + 1 <= roi.Y + roi.Height &&
                                        col + 1 <= roi.X + roi.Width;

                    bool isRoiX = col == roi.X || col == roi.X + roi.Width;
                    bool isRoiXB = col + 1 == roi.X || col + 1 == roi.X + roi.Width;
                    if (isInsideRoi && (isRoiY || isRoiX))
                    {
                        hybridImg.Add(255);
                        hybridImg.Add(255);
                        hybridImg.Add(255);
                    }
                    else if (isInsideRoiB && (isRoiYB || isRoiXB))
                    {
                        hybridImg.Add(1);
                        hybridImg.Add(1);
                        hybridImg.Add(1);
                    }
                    else
                    {
                        byte redChannel = refImg.ElementAt(i);
                        byte greenChannel = registeredImg.ElementAt(i);
                        byte blueChannel = (byte)((refImg.ElementAt(i) + registeredImg.ElementAt(i)) / 2);
                        hybridImg.Add(redChannel);
                        hybridImg.Add(greenChannel);
                        hybridImg.Add(blueChannel);
                    }
                }
            }

            controlImg.Data = hybridImg.ToArray();
            return controlImg;
        }

        public struct RegistrationData
        {
            public RegistrationData(ServiceImage refImg, ServiceImage refImgPreprocessed, ServiceImage sensedImage,
                ServiceImage sensedImagePreprocessed, Interface.Algo.RegionOfInterest roi, double angleTolerance,
                double scaleTolerance, int dilationMaskSize)
            {
                RefImg = refImg;
                RefImgPreprocessed = refImgPreprocessed;
                SensedImg = sensedImage;
                SensedImgPreprocessed = sensedImagePreprocessed;
                ROI = roi;
                AngleTolerance = angleTolerance;
                ScaleTolerance = scaleTolerance;
                DilationMaskSize = dilationMaskSize;
            }

            public ServiceImage RefImg { get; }
            public ServiceImage RefImgPreprocessed { get; }
            public ServiceImage SensedImg { get; }
            public ServiceImage SensedImgPreprocessed { get; }
            public Interface.Algo.RegionOfInterest ROI { get; }
            public double AngleTolerance { get; }
            public double ScaleTolerance { get; }
            public int DilationMaskSize { get; }
        }
    }
}
