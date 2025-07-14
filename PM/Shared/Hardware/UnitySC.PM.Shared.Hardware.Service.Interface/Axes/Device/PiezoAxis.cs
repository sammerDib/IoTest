using System;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class PiezoAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;

        public PiezoAxisConfig PiezoAxisConfig { get; set; }

        public PiezoAxis(AxisConfig axisConfig, ILogger logger) : base(axisConfig, logger)
        {
            if (axisConfig is PiezoAxisConfig piezoAxisConfig) PiezoAxisConfig = piezoAxisConfig;
            else throw new InvalidCastException("Configuration is not a PiezoAxisConfig.");
        }
    }
}
