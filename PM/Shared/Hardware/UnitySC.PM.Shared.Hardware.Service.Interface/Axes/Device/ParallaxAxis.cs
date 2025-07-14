using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class ParallaxAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public ParallaxAxisConfig Config { get; }

        public ParallaxAxis(ParallaxAxisConfig config) : base(config, ClassLocator.Default.GetInstance<ILogger>())
        {
            Config = config;
            CurrentPos = 0.Millimeters();
        }
    }
}
