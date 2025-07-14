using System;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class ExposureCalibration
    {
        public ExposureMatchingGoldenValues ExposureMatchingGoldenValues { get; set; }
        public string CameraSerialNumber { get; set; }

        public double ExposureCorrectionCoef { get; set; }
    }
}
