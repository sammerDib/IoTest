using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class CNCAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public CNCAxisConfig Config { get; }

        public CNCAxis(CNCAxisConfig config) : base(config, ClassLocator.Default.GetInstance<ILogger>())
        {
            Config = config;
        }
    }
}
