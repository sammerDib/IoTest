using System;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class BlackDeadPixelExposureInputs
    {
        public double DefaultCalibrationExposureTimeMsFS { get; set; }

        public double DefaultCalibrationExposureTimeMsBS { get; set; }
    }
}
