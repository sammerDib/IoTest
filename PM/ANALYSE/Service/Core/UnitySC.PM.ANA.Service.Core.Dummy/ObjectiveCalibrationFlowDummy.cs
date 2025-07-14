using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class ObjectiveCalibrationFlowDummy : ObjectiveCalibrationFlow
    {
        private AnaHardwareManager _hardwareManager;

        public ObjectiveCalibrationFlowDummy(ObjectiveCalibrationInput input) : base(input)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            Result.ObjectiveId = Input.ObjectiveId;
            Thread.Sleep(1000);
            CheckCancellation();

            SetProgressMessage("In Progess 1");

            if (!Input.ObjectiveId.Contains("INT"))
            {
                Thread.Sleep(1000);
                CheckCancellation();

                SetProgressMessage("In Progress 2");

                Thread.Sleep(1000);
                CheckCancellation();
            }

            var pos = (XYZTopZBottomPosition)_hardwareManager.Axes.GetPos();
            Result.AutoFocus.ZFocusPosition = pos.ZTop.Millimeters();

            // Use contains instead of check objective type to simplify tests (no need to create an objective config)
            if (!Input.ObjectiveId.Contains("INT"))
            {
                Result.AutoFocus.Lise.AirGap = 2400.Micrometers();
                Result.AutoFocus.Lise.MinGain = 1.2;
                Result.AutoFocus.Lise.MaxGain = 1.9;
                Result.AutoFocus.Lise.ZStartPosition = 11.Millimeters();
            }
            else
            {
                Result.AutoFocus.Lise = null;
            }
            if (Input.PreviousCalibration?.Image != null)
            {
                Result.Image.PixelSizeX = Input.PreviousCalibration.Image.PixelSizeX;
                Result.Image.PixelSizeY = Input.PreviousCalibration.Image.PixelSizeY;
                Result.Image.XOffset = Input.PreviousCalibration.Image.XOffset;
                Result.Image.YOffset = Input.PreviousCalibration.Image.YOffset;
            }
            else
            {
                Result.Image.PixelSizeX = 0.Micrometers();
                Result.Image.PixelSizeY = 0.Micrometers();
                Result.Image.XOffset = 0.Micrometers();
                Result.Image.YOffset = 0.Micrometers();
            }

            if (Input.OpticalReferenceElevationFromStandardWafer != null)
            {
                Result.OpticalReferenceElevationFromStandardWafer = Input.PreviousCalibration.OpticalReferenceElevationFromStandardWafer;
            }
            else
            {
                Result.OpticalReferenceElevationFromStandardWafer = 20.Micrometers();
            }
        }
    }
}
