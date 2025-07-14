using System;
using System.IO;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Calibration
{
    public class GlobalTopoSystemCalibrationFlow : FlowComponent
        <GlobalTopoSystemCalibrationInput,
        GlobalTopoSystemCalibrationResult,
        GlobalTopoSystemCalibrationConfiguration>
    {
        private readonly DMTHardwareManager _hardwareManager;

        public GlobalTopoSystemCalibrationFlow(GlobalTopoSystemCalibrationInput input, DMTHardwareManager hardwareManager)
            : base(input, "GlobalTopoSystemCalibrationFlow")
        {
            _hardwareManager = hardwareManager;
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
                    new Point2f(waferDefinition.BottomCheckerBoardPositionX, waferDefinition.BottomCheckerBoardPositionY),
                    new Point2f(waferDefinition.RightCheckerBoardPositionX, waferDefinition.RightCheckerBoardPositionY),
                    new Point2f(waferDefinition.TopCheckerBoardPositionX, waferDefinition.TopCheckerBoardPositionY));

            CheckerBoardsSettings checkerBoardsSettings =
                new CheckerBoardsSettings(
                    checkerBoardsOrigins,
                    waferDefinition.SquareXNumber,
                    waferDefinition.SquareYNumber,
                    waferDefinition.SquareSizeMm,
                    Input.PixelSize,
                    Configuration.UseAllCheckerBoards);

            var phaseMapX = Input.UnwrappedPhaseMapX;
            var phaseMapY = Input.UnwrappedPhaseMapY;
            var checkerBoardImg = Input.CheckerBoardImg;
            var cameraCalib = Input.GlobalTopoCameraCalibResult;
            var cameraIntrinsic = ArrayUtils.JaggedArrayTo2D(cameraCalib.CameraIntrinsic.Values);
            float screenPixelSizeInUm = (float)_hardwareManager.ScreensBySide[Input.Side].PixelPitchHorizontal;
            int smallestPeriod = Input.Periods[0];

            InputSystemParameters parameters = new InputSystemParameters(
                                                    checkerBoardsSettings,
                                                    Configuration.EdgeExclusionInMm,
                                                    waferDefinition.WaferRadiusMm,
                                                    Configuration.NbPtsScreen,
                                                    smallestPeriod,
                                                    screenPixelSizeInUm);

            ReportImagesIfNeeded();

            SetProgressMessage($"Starting system calibration");
            SystemParameters systemCalib = PSDCalibration.CalibrateSystem(phaseMapX, phaseMapY,
                                                                        checkerBoardImg,
                                                                        cameraIntrinsic,
                                                                        cameraCalib.Distortion,
                                                                        parameters);
            SetProgressMessage($"System calibration done");

            Result.ExtrinsicCamera = new ExtrinsicCameraCalibration
                (new Matrix<double>(systemCalib.ExtrinsicCamera.RWaferToCamera), systemCalib.ExtrinsicCamera.TWaferToCamera);
            Result.ExtrinsicScreen = new ExtrinsicScreenCalibration
                (new Matrix<double>(systemCalib.ExtrinsicScreen.RScreenToWafer), systemCalib.ExtrinsicScreen.TScreenToWafer);
            Result.ExtrinsicSystem = new ExtrinsicSystemCalibration
                (new Matrix<double>(systemCalib.ExtrinsicSystem.RScreenToCamera), systemCalib.ExtrinsicSystem.TScreenToCamera);
        }

        private void ReportImagesIfNeeded()
        {
            if (Configuration.IsAnyReportEnabled())
            {
                using (var reportCheckerBoardImg = Input.CheckerBoardImg.ConvertToUSPImageMil(false))
                {
                    reportCheckerBoardImg.Save(Path.Combine(ReportFolder, $"CheckerBoard.tif"));
                }
                using (var reportUnwrappedPhaseMapX = Input.UnwrappedPhaseMapX.ConvertToUSPImageMil(false))
                {
                    reportUnwrappedPhaseMapX.Save(Path.Combine(ReportFolder, $"UnwrappedPhaseMapX.tif"));
                }
                using (var reportUnwrappedPhaseMapY = Input.UnwrappedPhaseMapY.ConvertToUSPImageMil(false))
                {
                    reportUnwrappedPhaseMapY.Save(Path.Combine(ReportFolder, $"UnwrappedPhaseMapY.tif"));
                }
            }
        }
    }
}
