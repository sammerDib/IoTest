using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class OwisAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public OwisAxisConfig Config { get; }

        public OwisAxis(OwisAxisConfig config, ILogger logger) : base(config, logger)
        {
            Config = config;
            CurrentPos = 0.Millimeters();
        }
    }
}
