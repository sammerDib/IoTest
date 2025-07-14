using ACS.SPiiPlusNET;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class ACSAxis : MotorizedAxis
    {
        #region Fields

        public ACSAxisConfig ACSAxisConfig { get; set; }
        public new AxisConfig AxisConfiguration { get => ACSAxisConfig; }

        #endregion Fields

        #region Constructors

        public ACSAxis(AxisConfig axisConfig, ILogger logger)
            : base(axisConfig, logger)
        {
            if (axisConfig is ACSAxisConfig)
                ACSAxisConfig = (ACSAxisConfig)axisConfig;
        }

        #endregion Constructors

        #region Properties

        public MotorStates MotorState { get; set; }
        public SafetyControlMasks Fault { get; set; } = 0;
        public override bool IsLandingUsed { get => ACSAxisConfig.UsedInLanding; }

        #endregion Properties
    }
}
