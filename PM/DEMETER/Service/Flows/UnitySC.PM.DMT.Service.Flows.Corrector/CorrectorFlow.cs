using System;
using System.IO;

using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;



namespace UnitySC.PM.DMT.Service.Flows.Corrector
{
    public class CorrectorFlow : FlowComponent<CorrectorInput, CorrectorResult, CorrectorConfiguration>
    {

        public const int MaximumNumberOfSteps = 4;
        
        public CorrectorFlow(CorrectorInput input) : base(input, "CorrectorFlow")
        {
        }

        protected override void Process()
        {
            CheckCancellation();
            if (Input.ProductCharacteristics.WaferShape == WaferShape.Notch)
            {
                SetProgressMessage($"Starting {Input.Side}side Corrector computation");
                var correctorImageData =
                    CreateImageDataFromUSPImageMilAndApplyPerspectiveCalibration(Input.AcquiredImage);
                try
                {
                    ComputeWaferAngle(correctorImageData);
                }
                finally
                {
                    if (Configuration.IsAnyReportEnabled())
                    {
                        ServiceImage.ImageType destImgType;
                        switch (correctorImageData.Type)
                        {
                            case ImageType.RGB_Unsigned8bits:
                                destImgType = ServiceImage.ImageType.RGB;
                                break;
                            case ImageType.GRAYSCALE_Float32bits:
                                destImgType = ServiceImage.ImageType._3DA;
                                break;
                            case ImageType.GRAYSCALE_Unsigned8bits:
                            default:
                                destImgType = ServiceImage.ImageType.Greyscale;
                                break;
                        }

                        var transformedImage = new ServiceImage()
                        {
                            Data = correctorImageData.ByteArray,
                            DataHeight = correctorImageData.Height,
                            DataWidth = correctorImageData.Height,
                            Type = destImgType
                        };
                        transformedImage.SaveToFile(Path.Combine(ReportFolder, "Corrector_transformed_image.tif"));
                    }
                }
            }
        }

        private ImageData CreateImageDataFromUSPImageMilAndApplyPerspectiveCalibration(USPImageMil sourceImg)
        {
            byte[] imageByteArray;
            ImageData resultImageData = null;
            using (var calibratedImage = Input.Transform.Transform(sourceImg))
            {
                var correctorServiceImage = calibratedImage.ToServiceImage();
                imageByteArray = new byte[correctorServiceImage.Data.Length];
                correctorServiceImage.Data.CopyTo(imageByteArray, 0);
                ImageType imageDataType;
                switch (correctorServiceImage.Type)
                {
                    case ServiceImage.ImageType.Greyscale:
                        imageDataType = ImageType.GRAYSCALE_Unsigned8bits;
                        break;

                    case ServiceImage.ImageType.RGB:
                        imageDataType = ImageType.RGB_Unsigned8bits;
                        break;

                    default:
                        imageDataType = ImageType.GRAYSCALE_Float32bits;
                        break;
                }

                resultImageData = new ImageData(imageByteArray, correctorServiceImage.DataWidth,
                    correctorServiceImage.DataHeight, imageDataType);
            }

            return resultImageData;
        }

        private void ComputeWaferAngle(ImageData waferImage)
        {
            var waferCharacteristics = Input.ProductCharacteristics;
            double pixelSize = Input.Transform.PixelSize;

            float waferDiameterInMicrons = (float)waferCharacteristics.Diameter.Micrometers;
            float diameterToleranceInMicrons;
            if (null != waferCharacteristics.DiameterTolerance)
            {
                diameterToleranceInMicrons = (float)waferCharacteristics.DiameterTolerance.Micrometers;
            }
            else
            {
                diameterToleranceInMicrons =
                    (float)(0.01 * Configuration.WaferDiameterTolerancePercent * waferDiameterInMicrons);
            }

            SetProgressMessage("Starting wafer circle detection in image");
            var waferCircle = WaferDetector.FasterDetectWaferCircle(waferImage, waferDiameterInMicrons,
                diameterToleranceInMicrons, pixelSize);
            if (waferCircle.Diameter == 0)
            {
                throw new Exception("Couldn't find wafer inside image, correctors cannot be applied");
            }

            Result.WaferXShift = ((waferCircle.Center.X - (double)waferImage.Width / 2) * pixelSize).Micrometers();
            Result.WaferYShift = ((waferCircle.Center.Y - (double)waferImage.Height / 2) * pixelSize).Micrometers();

            double notchWidth = Configuration.NotchWidth.Micrometers;
            int notchWidthInPixels = (int)(notchWidth / pixelSize);
            int widthFactor = Configuration.NotchDetectionWidthFactor;
            double deviationFactor = Configuration.NotchDetectionDeviationFactor;
            double similarityThreshold = Configuration.NotchDetectionSimilarityThreshold;

            SetProgressMessage("Starting notch detection in image");
            double angle = NotchDetector.DetectNotchAngle(waferImage, waferCircle, NotchLocation.Bottom,
                notchWidthInPixels,
                widthFactor, deviationFactor, similarityThreshold);
            if (angle.IsNaN())
            {
                throw new Exception("Couldn't find wafer notch inside image, wafer angle cannot be computed");
            }
            Result.WaferAngle = angle.Degrees();
        }
    }
}
