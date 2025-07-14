using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class StageToWaferConverter : IReferentialConverter<XYZTopZBottomPosition>
    {
        private WaferReferentialSettings _waferReferentialSettings;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public bool IsEnabled { get; set; } = true;

        public StageToWaferConverter(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager, WaferReferentialSettings settings)
        {
            _waferReferentialSettings = settings;
            _hardwareManager = hardwareManager;
            _calibrationManager = calibrationManager;
        }

        public XYZTopZBottomPosition Convert(XYZTopZBottomPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZTopZBottomPosition;

            if (IsEnabled && _waferReferentialSettings != null && position.Referential.Tag == ReferentialTag.Stage)
            {
                position.X -= _waferReferentialSettings.ShiftX.Millimeters;
                position.Y -= _waferReferentialSettings.ShiftY.Millimeters;

                MathTools.ApplyAntiClockwiseRotation(-_waferReferentialSettings.WaferAngle, position, new XYPosition(position.Referential, 0, 0));

                var currentObjectiveCalibUp = GetObjectiveCalibration(ModulePositions.Up);
                if (!(_waferReferentialSettings.ZTopFocus is null) && !(currentObjectiveCalibUp is null))
                    position.ZTop -= (_waferReferentialSettings.ZTopFocus - currentObjectiveCalibUp.ZOffsetWithMainObjective).Millimeters;

                var currentObjectiveCalibDown = GetObjectiveCalibration(ModulePositions.Down);
                if (!(currentObjectiveCalibDown?.AutoFocus is null)) // Bottom focus never change
                    position.ZBottom -= currentObjectiveCalibDown.AutoFocus.ZFocusPosition.Millimeters;

                //System.Diagnostics.Debug.WriteLine($"Stage To Wafer : [{xyzPosition.X} {xyzPosition.Y}] => [{position.X} {position.Y}]");
            }

            position.Referential = new WaferReferential();
            return position;
        }

        private ObjectiveCalibration GetObjectiveCalibration(ModulePositions position)
        {
            var currentObjective = _hardwareManager.GetObjectiveInUseByPosition(position);
            return _calibrationManager.GetObjectiveCalibration(currentObjective.DeviceID);
        }

        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Stage && to == ReferentialTag.Wafer;
        }

        public void UpdateSettings(WaferReferentialSettings settings)
        {
            _waferReferentialSettings = settings;
        }
        public WaferReferentialSettings GetSettings()
        {
            return _waferReferentialSettings;
        }
    }
}
