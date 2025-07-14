using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class DummyAxis : IAxis
    {
        #region Fields

        public AxisConfig AxisConfiguration { get; set; }
        public ILogger Logger { get; set; }

        public string Name { get => AxisConfiguration.Name; }

        public string AxisID { get => AxisConfiguration.AxisID; }

        public bool Initialized { get; set; }
        public Length CurrentPos { get; set; }
        public bool Enabled { get; set; }
        public bool EnabledPrev { get; set; }
        public bool Moving { get; set; }
        public bool MovingPrev { get; set; }
        public Message DeviceError { get; set; }

        public bool IsLandingUsed { get => false; }

        #endregion Fields

        #region Constructors

        public DummyAxis(AxisConfig axisConfig, ILogger logger)

        {
            AxisConfiguration = axisConfig;
        }

        public bool ArePredifinedPositionsConfiguredValid()
        {
            return true;
        }

        public bool IsParkPosConfiguredValid()
        {
            return true;
        }

        public bool IsManualLoadPosConfiguredValid()
        {
            return true;
        }

        public bool IsHomePosConfiguredValid()
        {
            return true;
        }

        #endregion Constructors
    }
}
