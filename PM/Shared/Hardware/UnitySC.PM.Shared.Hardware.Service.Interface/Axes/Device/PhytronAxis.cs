using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class PhytronAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public PhytronAxisConfig Config { get; }

        public PhytronAxis(PhytronAxisConfig config) : base(config, ClassLocator.Default.GetInstance<ILogger>())
        {
            Config = config;
        }
    }
}
