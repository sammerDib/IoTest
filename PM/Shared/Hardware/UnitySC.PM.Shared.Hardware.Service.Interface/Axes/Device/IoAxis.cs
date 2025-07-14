using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class IoAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public IoAxisConfig Config { get; }

        public IoAxis(IoAxisConfig config) : base(config, ClassLocator.Default.GetInstance<ILogger>())
        {
            Config = config;
            CurrentPos = 0.Millimeters();
        }
    }
}
