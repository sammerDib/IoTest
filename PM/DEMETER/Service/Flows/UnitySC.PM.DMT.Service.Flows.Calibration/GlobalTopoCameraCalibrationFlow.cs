using System;
using System.IO;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Calibration
{
    public class GlobalTopoCameraCalibrationFlow : FlowComponent
        <GlobalTopoCameraCalibrationInput,
        GlobalTopoCameraCalibrationResult,
        GlobalTopoCameraCalibrationConfiguration>
    {
        private const bool UseAllCheckerBoards = true; // Not used by PSDCalibration.CalibrateCamera

        public GlobalTopoCameraCalibrationFlow(GlobalTopoCameraCalibrationInput input)
            : base(input, "GlobalTopoCameraCalibrationFlow")
        {
        }

        protected override void Process()
        {
            SetProgressMessage($"Starting {Name} flow");

            var waferDefinition = Input.WaferDefinition;
            if (waferDefinition == null)
            {
                throw new Exception("Wafer definition for global topo calibration is missing");
            }

            CheckerBoardsOrigins checkerBoardsOrigins =
                new CheckerBoardsOrigins(
                    new Point2f(waferDefinition.LeftCheckerBoardPositionX, waferDefinition.LeftCheckerBoardPositionY),
                    new Point2f(waferDefinition.TopCheckerBoardPositionX, waferDefinition.TopCheckerBoardPositionY),
                    new Point2f(waferDefinition.RightCheckerBoardPositionX, waferDefinition.RightCheckerBoardPositionY),
                    new Point2f(waferDefinition.BottomCheckerBoardPositionX, waferDefinition.BottomCheckerBoardPositionY));

            CheckerBoardsSettings checkerBoardsSettings =
                new CheckerBoardsSettings(
                    checkerBoardsOrigins,
                    waferDefinition.SquareXNumber,
                    waferDefinition.SquareYNumber,
                    waferDefinition.SquareSizeMm,
                    Input.PixelSize,
                    UseAllCheckerBoards);

            ImageData[] cameraCalibrationImages = new ImageData[Input.CameraCalibrationImages.Length];
            for (int i = 0; i < Input.CameraCalibrationImages.Length; i++)
            {
                var uspImageMil = Input.CameraCalibrationImages[i];
                cameraCalibrationImages[i] = ImageUtils.CreateImageDataFromUSPImageMil(uspImageMil);
            }

            ReportImagesIfNeeded();

            SetProgressMessage($"Starting camera calibration");
            CalibrationParameters intrinsicCameraParams = PSDCalibration.CalibrateCamera(cameraCalibrationImages, checkerBoardsSettings, false);
            SetProgressMessage($"Camera calibration done");
            Result.CameraIntrinsic = new Matrix<double>(intrinsicCameraParams.CameraIntrinsic);
            Result.Distortion = intrinsicCameraParams.Distortion;
            Result.RMS = intrinsicCameraParams.RMS;
        }

        private void ReportImagesIfNeeded()
        {
            if (Configuration.IsAnyReportEnabled())
            {
                for (int i = 0; i < Input.CameraCalibrationImages.Length; i++)
                {
                    var image = Input.CameraCalibrationImages[i];
                    image.Save(Path.Combine(ReportFolder, $"Image{i}.tif"));
                }
            }
        }
    }
}
