using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class StageToMotorConverter : IReferentialConverter<XYZTopZBottomPosition>
    {
        private StageReferentialSettings _stageReferentialSettings;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;

        public bool IsEnabled { get; set; } = true;

        public StageToMotorConverter(AnaHardwareManager hardwareManager, CalibrationManager calibrationManager)
        {
            _hardwareManager = hardwareManager;
            _calibrationManager = calibrationManager;
        }

        public XYZTopZBottomPosition Convert(XYZTopZBottomPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZTopZBottomPosition;

            if (IsEnabled && position.Referential.Tag == ReferentialTag.Stage)
            {
                // Objective centricity
                var objectiveCalibration = _calibrationManager.GetObjectiveCalibration(_hardwareManager.GetObjectiveInUseByPosition(ModulePositions.Up).DeviceID);
                if (!(objectiveCalibration?.Image?.XOffset is null))
                {
                    if (!double.IsNaN(position.X))
                        position.X += objectiveCalibration.Image.XOffset.Millimeters;
                    if (!double.IsNaN(position.Y))
                        position.Y += objectiveCalibration.Image.YOffset.Millimeters;
                }

                var xyCalibration = _calibrationManager.GetXYCalibrationData();
                if (!(xyCalibration is null))
                {
                    var correction = XYCalibrationHelper.ComputeCorrection(position, xyCalibration);
                    if (!double.IsNaN(position.X))
                        position.X += correction.Item1.Millimeters;
                    if (!double.IsNaN(position.Y))
                        position.Y += correction.Item2.Millimeters;
                }

                if (_stageReferentialSettings?.EnableProbeSpotOffset == true)
                {
                    var probeCalibration = _calibrationManager.GetLiseHFObjectiveSpotPosition(_hardwareManager.GetObjectiveInUseByPosition(ModulePositions.Up).DeviceID);
                    if (!(probeCalibration?.XOffset is null) && !(probeCalibration?.YOffset is null))
                    {
                        position.X -= probeCalibration.XOffset.Millimeters;
                        position.Y -= probeCalibration.YOffset.Millimeters;
                    }
                }

                //System.Diagnostics.Debug.WriteLine($"Stage To Motor : [{xyzPosition.X} {xyzPosition.Y}] => [{position.X} {position.Y}]");
            }

            position.Referential = new MotorReferential();
            return position;
        }

        public void UpdateSettings(StageReferentialSettings settings)
        {
            _stageReferentialSettings = settings;
        }

        public StageReferentialSettings GetSettings()
        {
            return _stageReferentialSettings;
        }
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Stage && to == ReferentialTag.Motor;
        }
    }
}
