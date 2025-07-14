using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.CurvatureDynamics
{
    [Serializable]
    public class CurvatureDynamicsCalibrationData
    {
        public double DynamicsCoefficient { get; set; } = 1;
        public int FringePeriod { get; set; }
        public Side Side { get; set; }
    }
}
