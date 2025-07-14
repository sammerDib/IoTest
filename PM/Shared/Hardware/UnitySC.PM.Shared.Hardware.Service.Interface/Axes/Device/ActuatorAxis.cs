using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Device
{
    public abstract class ActuatorAxis : IAxis
    {
        #region Fields
        private AxisConfig _axisConfig;

        #endregion

        public ActuatorAxis(AxisConfig axisConfig, ILogger logger)
        {
            _axisConfig = axisConfig;
            Logger = logger;
        }

        #region Properties

        public bool ArePredifinedPositionsConfiguredValid()
        {
            return IsParkPosConfiguredValid() && IsManualLoadPosConfiguredValid() && IsHomePosConfiguredValid();
        }

        public bool IsParkPosConfiguredValid()
        {
            if ((AxisConfiguration.PositionPark.Millimeters != 1) && (AxisConfiguration.PositionPark.Millimeters != 0))
            {
                Logger?.Warning("Wrong definition for this axis " + AxisConfiguration.Name);
                Logger?.Warning(AxisConfiguration.PositionPark.ToString() + " for the Position park is not ranged between " + AxisConfiguration.PositionMin.ToString() + " and " + AxisConfiguration.PositionMax.ToString());
                return false;
            }
            return true;
        }


        public bool IsManualLoadPosConfiguredValid()
        {
            if ((AxisConfiguration.PositionManualLoad.Millimeters != 1) && (AxisConfiguration.PositionManualLoad.Millimeters != 0))
            {
                Logger?.Warning(AxisConfiguration.PositionManualLoad.ToString() + " for the Position ManualLoad is not 0 or 1");
                return false;
            }

            Logger?.Information("Good definition for this axis " + AxisConfiguration.Name);
            return true;
        }

        public bool IsHomePosConfiguredValid()
        {
            if ((AxisConfiguration.PositionHome.Millimeters != 1) && (AxisConfiguration.PositionHome.Millimeters != 0))
            {
                Logger?.Warning(AxisConfiguration.PositionManualLoad.ToString() + " for the Position ManualLoad is not 0 or 1");
                return false;
            }

            Logger?.Information("Good definition for this axis " + AxisConfiguration.Name);
            return true;
        }
        public abstract bool IsLandingUsed { get; }


        public AxisConfig AxisConfiguration { get => _axisConfig; }
        public int MotorError { get; set; } = 0;
        public int MotionError { get; set; } = 0;
        public ILogger Logger { get; set; }

        public bool Initialized { get; set; }

        public Length CurrentPos { get; set; }

        public bool Enabled { get; set; }

        public bool EnabledPrev { get; set; }

        public bool Moving { get; set; }

        public bool MovingPrev { get; set; }

        public Message DeviceError { get; set; }
        public string Name { get => AxisConfiguration.Name; }
        public string AxisID { get => AxisConfiguration.AxisID; }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion
    }
}
