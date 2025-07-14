using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    /// <summary>
    /// Compute the offset of the wafer compared to Chunck Center position in stage referential:
    ///     - center (compared to the center of the referential)
    ///     - angle (compared to an expected angle)
    /// This is done with given camera.
    /// Prerequisites:
    ///     - make sure current Z position is relevant for camera focus (can be computed with AFLiseFlow).
    ///     - make sure light intensity is relevant, since this flow captures images (can be computed with AutolightFlow).
    /// </summary>
    public class BareWaferAlignmentFlow : FlowComponent<BareWaferAlignmentInput, BareWaferAlignmentChangeInfo, BareWaferAlignmentConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private BareWaferAlignmentImage _notchImageResult;
        private XYPosition _chuckCenter;

        public BareWaferAlignmentFlow(BareWaferAlignmentInput input) : base(input, "BareWaferAlignmentFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            FdcProvider = ClassLocator.Default.GetInstance<BareWaferAlignmentFlowFDCProvider>();
        }

        protected override void Process()
        {
            var iswaferclamped = Task.Run(async () => await IsWaferClampedAsync().ConfigureAwait(false)).Result;
            if (!iswaferclamped)
            {
                string errorMsg = "The BWA failed because it was impossible to clamp the wafer";
                throw new Exception($"Error during {Name} : {errorMsg}");
            }

            var slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(Input.Wafer.Diameter);
            if (slotConfig == null)
            {
                throw new Exception($"Error during {Name} : No slot config found for diameter {(int)Input.Wafer.Diameter.Millimeters}");
            }
            
            _chuckCenter = slotConfig.PositionChuckCenter.ToXYPosition();

            // We need to move bottom probe because it could be visible by the top one
            // and then skew the results
            MoveBottomAxisToAvoidCameraInterferences();

            // Adding light intensity to overexpose the images and make the edges more visible / well defined
            var originalLightIntensity = _hardwareManager.GetMainLight().GetIntensity();
            var intensityPercentageToAdd = (double)Configuration.LightBoostPercentage / 100.0;
            var newLightIntensity = originalLightIntensity + (originalLightIntensity * intensityPercentageToAdd);
            if (newLightIntensity > 100.0)
            {
                newLightIntensity = 100.0;
            }

            _hardwareManager.GetMainLight().SetIntensity(newLightIntensity);

            var takenImages = ProcessEachImage(Input.Wafer);

            var waferInfo = new WaferInfos
            {
                RadiusInMicrons = (int)(Input.Wafer.Diameter.Micrometers / 2.0),
                NotchWidthInMicrons = (int)Configuration.NotchWidth.Micrometers,
                Type = TypeConverter.ConvertWaferShape(Input.Wafer.WaferShape)
            };

            _hardwareManager.GetMainLight().SetIntensity(originalLightIntensity);

            var bwaResult = UnitySCSharedAlgosOpenCVWrapper.BareWaferAlignment.PerformBareWaferAlignment(takenImages, waferInfo, Configuration.EdgeDetectionVersion, Configuration.NotchDetectionVersion, Configuration.CannyThreshold, ReportFolder, Configuration.ReportOption);

            if (_notchImageResult != null)
            {
                if (bwaResult.Status.Code == StatusCode.OK)
                {
                    double threePiOnTwo = (3 * Math.PI / 2);
                    var rawAngleInRadian = bwaResult.Rotation + threePiOnTwo;

                    var xNotchLocationShifted = waferInfo.RadiusInMicrons * Math.Cos(rawAngleInRadian);
                    var xNotchLocationOnStage = xNotchLocationShifted + bwaResult.Shift.X - _chuckCenter.X.Millimeters().ToUnit(LengthUnit.Micrometer).Value;
                    var xNotchLocationOnStageInPixels = xNotchLocationOnStage / takenImages[0].Scale.X.Micrometers().Value;
                    var xNotchLocationInImage = (_notchImageResult.Image.DataWidth / 2) + xNotchLocationOnStageInPixels;

                    _notchImageResult.NotchLines = new List<(ServicePoint pt1, ServicePoint pt2)>
                    {
                        (new ServicePoint(xNotchLocationInImage, 0), new ServicePoint(xNotchLocationInImage, _notchImageResult.Image.DataHeight))
                    };
                }
                SetProgressMessage("Notch Location Detection Finished", _notchImageResult);
            }

            Result = new Interface.Algo.BareWaferAlignmentResult();
            ((Interface.Algo.BareWaferAlignmentResult)Result).Diameter = Input.Wafer.Diameter;
            ((Interface.Algo.BareWaferAlignmentResult)Result).Angle = bwaResult.Rotation.Radians().ToUnit(AngleUnit.Degree);
            ((Interface.Algo.BareWaferAlignmentResult)Result).ShiftX = bwaResult.Shift.X.Micrometers() - _chuckCenter.X.Millimeters().ToUnit(LengthUnit.Micrometer);
            ((Interface.Algo.BareWaferAlignmentResult)Result).ShiftY = bwaResult.Shift.Y.Micrometers() - _chuckCenter.Y.Millimeters().ToUnit(LengthUnit.Micrometer);
            ((Interface.Algo.BareWaferAlignmentResult)Result).ShiftStageX = bwaResult.Shift.X.Micrometers();
            ((Interface.Algo.BareWaferAlignmentResult)Result).ShiftStageY = bwaResult.Shift.Y.Micrometers();
            ((Interface.Algo.BareWaferAlignmentResult)Result).Confidence = bwaResult.Status.Confidence;

            if (StatusCode.OK != bwaResult.Status.Code)
            {
                throw new Exception(bwaResult.Status.Message);
            }

            (FdcProvider as BareWaferAlignmentFlowFDCProvider).CreateFDC((Interface.Algo.BareWaferAlignmentResult)Result);
        }

        private void MoveBottomAxisToAvoidCameraInterferences()
        {
            var zBottomAxisConfig = _hardwareManager.Axes.AxesConfiguration.AxisConfigs.First(axisConfig => axisConfig.MovingDirection == MovingDirection.ZBottom);
            var currentAxesPos = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);
            var ZBottomPosToAvoidInterferences = currentAxesPos.ZTop - Configuration.DistanceToAvoidCameraInterference.Millimeters;
            ZBottomPosToAvoidInterferences = Math.Max(ZBottomPosToAvoidInterferences, zBottomAxisConfig.PositionMin.Millimeters);

            var position = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, double.NaN, ZBottomPosToAvoidInterferences);
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, position);
        }

        private List<BareWaferAlignmentImageData> ProcessEachImage(WaferDimensionalCharacteristic waferType)
        {
            CheckCancellation();
         
            var imagePositions = ImageSetCentroidFactory.GetImageDataListFor(Input.Wafer);
            var takenImages = new List<BareWaferAlignmentImageData>();

            foreach (var imageToTake in imagePositions)
            {
                CheckCancellation();
                var image = ProcessOneImageAndSendStateUpdate(imageToTake);
                takenImages.Add(image);
            }
            return takenImages;
        }

        private BareWaferAlignmentImageData ProcessOneImageAndSendStateUpdate(BareWaferAlignmentImageData imageToTake)
        {
            var intermediateProgress = new BareWaferAlignmentImage()
            {
                ImageState = FlowState.InProgress,
                EdgePosition = TypeConverter.ConvertEdgePosition(imageToTake.EdgePosition)
            };

            BareWaferAlignmentImageData takenImageData;
            ServiceImage image;

            var manuallyAcquiredImage = GetManualImageForPosition(imageToTake.EdgePosition);
            if (manuallyAcquiredImage != null && manuallyAcquiredImage.Image != null)
            {
                SetProgressMessage("Reused manually acquired image", intermediateProgress);
                takenImageData = CreateImageDataFromPreacquiredImage(manuallyAcquiredImage, imageToTake.ExpectedShape, imageToTake.EdgePosition);
                image = manuallyAcquiredImage.Image;
            }
            else if (manuallyAcquiredImage != null && manuallyAcquiredImage.CenterPosition != null)
            {
                SetProgressMessage("Reused manually acquired position to acquire image", intermediateProgress);
                var imageCenter = manuallyAcquiredImage.CenterPosition.ToXYPosition();
                var centroid = new Point2d(imageCenter.X * 1000, imageCenter.Y * 1000);  // convert from mm (UI) to micrometer (algorithm)
                takenImageData = CreateImageDataFromCameraAcquisition(centroid, imageToTake.ExpectedShape, imageToTake.EdgePosition, imageToTake.StitchColumns, imageToTake.StitchRows);
                image = createServiceImageFromBareWaferAlignmentImageData(takenImageData);
            }
            else
            {
                SetProgressMessage("Acquiring Image", intermediateProgress);
                takenImageData = CreateImageDataFromCameraAcquisition(imageToTake.Centroid, imageToTake.ExpectedShape, imageToTake.EdgePosition, imageToTake.StitchColumns, imageToTake.StitchRows);
                image = createServiceImageFromBareWaferAlignmentImageData(takenImageData);
            }

            SaveImageIfReportIsEnabled(image, imageToTake);

            var intermediateResult = new BareWaferAlignmentImage()
            {
                ImageState = FlowState.Success,
                Image = image,
                Position = new XYZTopZBottomPosition(new WaferReferential(), takenImageData.Centroid.X, takenImageData.Centroid.Y, -1, -1),
                EdgePosition = TypeConverter.ConvertEdgePosition(takenImageData.EdgePosition),
                EdgePoints = GetWaferBorderPoints(takenImageData, (int) Input.Wafer.Diameter.Millimeters, Configuration.EdgeDetectionVersion, Configuration.CannyThreshold),
            };

            if (takenImageData.ExpectedShape != WaferEdgeShape.NOTCH)
            {
                SetProgressMessage("Contour Extraction finished", intermediateResult);
            }
            else
            {
                _notchImageResult = intermediateResult;
            }

            return takenImageData;
        }

        private ServiceImageWithPosition GetManualImageForPosition(EdgePosition edgePosition)
        {
            ServiceImageWithPosition inputImage;
            switch (edgePosition)
            {
                case EdgePosition.TOP:
                    inputImage = Input.ImageTop;
                    break;

                case EdgePosition.RIGHT:
                    inputImage = Input.ImageRight;
                    break;

                case EdgePosition.BOTTOM:
                    inputImage = Input.ImageBottom;
                    break;

                case EdgePosition.LEFT:
                    inputImage = Input.ImageLeft;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return inputImage;
        }

        private void SaveImageIfReportIsEnabled(ServiceImage image, BareWaferAlignmentImageData imageToTake)
        {
            if (Configuration.IsAnyReportEnabled())
            {
                string filename = imageToTake.EdgePosition.ToString() + ".jpg";
                ImageReport.SaveImage(image, Path.Combine(ReportFolder, filename));
            }
        }

        private static List<ServicePoint> GetWaferBorderPoints(BareWaferAlignmentImageData imageToTake, int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold)
        {
            if (imageToTake.ExpectedShape == WaferEdgeShape.NOTCH) return null;
            var edgeExtractionInfo = UnitySCSharedAlgosOpenCVWrapper.BareWaferAlignment.PerformEdgeImageContourExtraction(imageToTake, waferDiameterInMm, edgeDetectionVersion, cannyThreshold);
            return edgeExtractionInfo.Contour.Select(edgePoint => new ServicePoint(edgePoint.X, edgePoint.Y))
                .ToList();
        }

        private BareWaferAlignmentImageData CreateImageDataFromPreacquiredImage(ServiceImageWithPosition image, WaferEdgeShape expectedShape, EdgePosition edgePosition)
        {
            if (image?.Image == null) return null;

            var objectiveCalibration = HardwareUtils.GetObjectiveParametersUsedByCamera(_hardwareManager, _calibrationManager, Input.CameraId);

            var imageCenter = image.CenterPosition.ToXYPosition();
            var centroid = new Point2d(imageCenter.X * 1000, imageCenter.Y * 1000);  // convert from mm (UI) to micrometer (algorithm)
            var scale = new PixelSize(objectiveCalibration.Image.PixelSizeX.Micrometers, objectiveCalibration.Image.PixelSizeY.Micrometers);

            var imageData = new BareWaferAlignmentImageData
            {
                Type = AlgorithmLibraryUtils.ServiceImageTypeToOpenCVImageType(image.Image.Type),
                Width = image.Image.DataWidth,
                Height = image.Image.DataHeight,
                Centroid = centroid,
                Scale = scale,
                ByteArray = image.Image.Data,
                ExpectedShape = expectedShape,
                EdgePosition = edgePosition
            };

            return imageData;
        }

        private BareWaferAlignmentImageData CreateImageDataFromCameraAcquisition(Point2d centroid, WaferEdgeShape expectedShape, EdgePosition edgePosition, int nbColumns, int nbRows)
        {
            var pt = new XYPosition(new StageReferential(), centroid.X / 1000, centroid.Y / 1000);
            var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            var objectiveCalibration = HardwareUtils.GetObjectiveParametersUsedByCamera(_hardwareManager, _calibrationManager, Input.CameraId);
            var imageScale = new PixelSize(objectiveCalibration.Image.PixelSizeX.Micrometers, objectiveCalibration.Image.PixelSizeY.Micrometers);

            var serviceImageWithPositionList = HardwareUtils.AcquireNCameraImagesAroundXYPosition(_hardwareManager, cameraManager, Input.CameraId, pt, nbColumns, nbRows);
            var positionImageDataList = AlgorithmLibraryUtils.ConvertServiceImageWithPositionToPositionImageData(serviceImageWithPositionList, imageScale);
            var bareWaferAlignmentImageDataList = new List<BareWaferAlignmentImageData>();
            foreach (var positionImageData in positionImageDataList)
            {
                bareWaferAlignmentImageDataList.Add(
                    new BareWaferAlignmentImageData()
                    {
                        ByteArray = positionImageData.ByteArray,
                        Height = positionImageData.Height,
                        Width = positionImageData.Width,
                        Type = positionImageData.Type,
                        Scale = positionImageData.Scale,
                        Centroid = positionImageData.Centroid,
                        ExpectedShape = expectedShape,
                        EdgePosition = edgePosition,
                    });
            }

            BareWaferAlignmentImageData bwaImageData = null;

            if (bareWaferAlignmentImageDataList.Count == 1) bwaImageData = bareWaferAlignmentImageDataList[0];
            else if (bareWaferAlignmentImageDataList.Count > 1) bwaImageData = Stitcher.StitchImages(bareWaferAlignmentImageDataList);

            return bwaImageData;
        }

        protected async Task<bool> IsWaferClampedAsync()
        {
            //The wafer must be clamped before BWA.
            bool isWaferClamped = _hardwareManager.Chuck.GetState().WaferClampStates[Input.Wafer.Diameter];
            if (!isWaferClamped)
            {
                try
                {
                    _hardwareManager.ClampHandler.ClampWafer(Input.Wafer.Diameter);
                    await TaskExt.WaitUntil(() => (_hardwareManager.Chuck.GetState().WaferClampStates[Input.Wafer.Diameter]), 100, 20000);
                    isWaferClamped = _hardwareManager.Chuck.GetState().WaferClampStates[Input.Wafer.Diameter];
                }
                catch (Exception Ex)
                {
                    Logger?.Error($"{LogHeader} {Ex.Message}");
                    isWaferClamped = false;
                }
            }
            return isWaferClamped;
        }

        private ServiceImage createServiceImageFromBareWaferAlignmentImageData(BareWaferAlignmentImageData image)
        {
            var serviceImg = new ServiceImage
            {
                // OPTIMIZATION image pixels are most probably copied here
                Data = image.ByteArray,
                Type = ServiceImage.ImageType.Greyscale,
                DataWidth = image.Width,
                DataHeight = image.Height
            };
            return serviceImg;
        }
    }
}
