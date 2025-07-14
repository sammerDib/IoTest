using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Core.Referentials.Converters
{
    public class StageToMotorConverter : IReferentialConverter<XYZPosition>
    {
        public StageReferentialSettings Settings { get; private set; }
        private readonly CalibrationManager _calibrationManager;
        public bool IsEnabled { get; set; } = true;

        public StageToMotorConverter(CalibrationManager calibrationManager)
        {
            _calibrationManager = calibrationManager;
        }

        public XYZPosition Convert(XYZPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZPosition;

            if (IsEnabled && position.Referential.Tag == ReferentialTag.Stage)
            {
                if (Settings?.EnableDistanceSensorOffset == true)
                {
                    var distanceSensorCalibration = _calibrationManager.GetDistanceSensorCalibrationData();
                    if (!(distanceSensorCalibration?.OffsetX is null) && !(distanceSensorCalibration.OffsetY is null))
                    {
                        position.X -= distanceSensorCalibration.OffsetX.Millimeters;
                        position.Y -= distanceSensorCalibration.OffsetY.Millimeters;
                    }
                }
            }

            position.Referential = new MotorReferential();
            return position;
        }
        public void UpdateSettings(StageReferentialSettings settings)
        {
            Settings = settings;
        }
        
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Stage && to == ReferentialTag.Motor;
        }
    }
}
