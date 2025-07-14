using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Core.Referentials.Converters
{
    public class MotorToStageConverter : IReferentialConverter<XYZPosition>
    {
        private StageReferentialSettings Settings { get; set; }
        private readonly CalibrationManager _calibrationManager;
        public bool IsEnabled { get; set; } = true;

        public MotorToStageConverter(CalibrationManager calibrationManager)
        {
            _calibrationManager = calibrationManager;
        }
        
        public XYZPosition Convert(XYZPosition xyzPosition)
        {
            var position = xyzPosition.Clone() as XYZPosition;

            if (IsEnabled && position.Referential.Tag == ReferentialTag.Motor)
            {
                if (Settings?.EnableDistanceSensorOffset == true)
                {
                    var distanceSensorCalibration = _calibrationManager.GetDistanceSensorCalibrationData();
                    if (!(distanceSensorCalibration?.OffsetX is null) && !(distanceSensorCalibration?.OffsetY is null))
                    {
                        position.X += distanceSensorCalibration.OffsetX.Millimeters;
                        position.Y += distanceSensorCalibration.OffsetY.Millimeters;
                    }
                }
            }

            position.Referential = new StageReferential();
            return position;
        }
        
        public void UpdateSettings(StageReferentialSettings settings)
        {
            Settings = settings;
        }
        
        public bool Accept(ReferentialTag from, ReferentialTag to)
        {
            return from == ReferentialTag.Motor && to == ReferentialTag.Stage;
        }
    }
}
