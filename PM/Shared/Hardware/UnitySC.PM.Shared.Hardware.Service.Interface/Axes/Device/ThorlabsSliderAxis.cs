using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class ThorlabsSliderAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public ThorlabsSliderAxisConfig Config { get; }

        public ThorlabsSliderAxis(ThorlabsSliderAxisConfig config, ILogger logger) : base(config, logger)
        {
            Config = config;
            CurrentPos = 0.Millimeters();
        }
    }
}
