using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public class RelianceAxis : MotorizedAxis
    {
        public override bool IsLandingUsed => false;
        public RelianceAxisConfig Config { get; }

        public RelianceAxis(RelianceAxisConfig config) : base(config, ClassLocator.Default.GetInstance<ILogger>())
        {
            Config = config;
        }
    }
}
