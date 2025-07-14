using ACS.SPiiPlusNET;

using Aerotech.Ensemble.Status;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class AerotechAxis : MotorizedAxis
    {
        #region Fields

        public AerotechAxisConfig AerotechAxisConfig { get; set; }
        public new AxisConfig AxisConfiguration { get => AerotechAxisConfig; }

        #endregion Fields

        #region Constructors

        public AerotechAxis(AxisConfig axisConfig, ILogger logger)
            : base(axisConfig, logger)
        {
            CurrentPos = 0.Millimeters();
            if (axisConfig is AerotechAxisConfig)
                AerotechAxisConfig = (AerotechAxisConfig)axisConfig;
        }

        #endregion Constructors

        #region Properties

        public AxisStatus Status { get; set; }
        public AxisFault Fault { get; set; } 
        public override bool IsLandingUsed { get => false; }

        #endregion Properties
    }
}
