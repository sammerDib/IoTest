using System.Collections.Generic;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    public class BareWaferAlignmentImageFlow : FlowComponent<BareWaferAlignmentImageInput, BareWaferAlignmentImage, BareWaferAlignmentImageConfiguration>
    {
        public BareWaferAlignmentImageFlow(BareWaferAlignmentImageInput input) : base(input, "BareWaferAlignmentImageFlow")
        {
        }

        protected override void Process()
        {
            Result.ImageState = FlowState.InProgress;
            Result.EdgePosition = Input.EdgePosition;

            Logger.Debug($"{LogHeader} Acquiring Image");

            var edgeShape = ImageShapeFromImagePosition(Input.Wafer, Input.EdgePosition);
            var hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            var imageXYPosition = Input.Position.ToXYPosition();
            var cameraImageWithPosition = HardwareUtils.AcquireCameraImageAtXYPosition(hardwareManager, cameraManager, Input.CameraId, imageXYPosition);

            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            var objectiveCalibration = HardwareUtils.GetObjectiveParametersUsedByCamera(hardwareManager, calibrationManager, Input.CameraId);
            var bwaImageData = new BareWaferAlignmentImageData()
            {
                //Centroid position needs to be in micrometers
                Centroid = new Point2d(imageXYPosition.X * 1000, imageXYPosition.Y * 1000),
                ExpectedShape = edgeShape,
                Width = cameraImageWithPosition.Image.DataWidth,
                Height = cameraImageWithPosition.Image.DataHeight,
                Type = AlgorithmLibraryUtils.ServiceImageTypeToOpenCVImageType(cameraImageWithPosition.Image.Type),
                ByteArray = cameraImageWithPosition.Image.Data,
                Scale = new PixelSize(objectiveCalibration.Image.PixelSizeX.Micrometers, objectiveCalibration.Image.PixelSizeY.Micrometers)
            };
            var edgeExtractionInfo = UnitySCSharedAlgosOpenCVWrapper.BareWaferAlignment.PerformEdgeImageContourExtraction(bwaImageData, (int) Input.Wafer.Diameter.Millimeters, Configuration.EdgeDetectionVersion, Configuration.CannyThreshold);

            var image = cameraImageWithPosition.Image;

            var edgePointsList = new List<ServicePoint>();
            foreach (var edgePoint in edgeExtractionInfo.Contour)
            {
                edgePointsList.Add(new ServicePoint(edgePoint.X, edgePoint.Y));
            }

            Result.ImageState = FlowState.Success;
            Result.Image = image;
            Result.Position = Input.Position;
            Result.EdgePosition = Input.EdgePosition;
            Result.EdgePoints = edgePointsList;

            Logger.Debug($"{LogHeader} Image acquired");
        }

        internal static WaferEdgeShape ImageShapeFromImagePosition(WaferDimensionalCharacteristic wafer, WaferEdgePositions edgePosition)
        {
            var result = WaferEdgeShape.EDGE;

            if (wafer.WaferShape == WaferShape.Notch)
            {
                if (edgePosition == WaferEdgePositions.Bottom)
                {
                    result = WaferEdgeShape.EDGE;
                }
            }
            return result;
        }
    }
}
