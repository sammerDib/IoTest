using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public abstract class MotorizedAxis : IAxis
    {
        #region Fields

        private AxisConfig _axisConfig;

        #endregion Fields

        public MotorizedAxis(AxisConfig axisConfig, ILogger logger)
        {
            _axisConfig = axisConfig;
            Logger = logger;
        }

        #region Properties

        public Length LastNotifiedPosition { get; set; }

        public Length DistanceThresholdForNotification => _axisConfig.DistanceThresholdForNotification;

        public double ComputePositionInControllerFrame(double position)
        {
            return AxisConfiguration.PositionZero.Millimeters + position;
        }

        public bool ArePredifinedPositionsConfiguredValid()
        {
            return IsParkPosConfiguredValid() && IsManualLoadPosConfiguredValid() && IsHomePosConfiguredValid();
        }

        public bool IsParkPosConfiguredValid()
        {
            if ((AxisConfiguration.PositionPark < AxisConfiguration.PositionMin) || (AxisConfiguration.PositionPark > AxisConfiguration.PositionMax))
            {
                Logger?.Warning("Wrong definition for this axis " + AxisConfiguration.Name);
                Logger?.Warning(AxisConfiguration.PositionPark.ToString() + " for the Position park is not ranged between " + AxisConfiguration.PositionMin.ToString() + " and " + AxisConfiguration.PositionMax.ToString());
                return false;
            }
            return true;
        }

        public bool IsManualLoadPosConfiguredValid()
        {
            if ((AxisConfiguration.PositionManualLoad < AxisConfiguration.PositionMin) || (AxisConfiguration.PositionManualLoad > AxisConfiguration.PositionMax))
            {
                Logger?.Warning(AxisConfiguration.PositionManualLoad.ToString() + " for the Position ManualLoad is not ranged between " + AxisConfiguration.PositionMin.ToString() + " and " + AxisConfiguration.PositionMax.ToString());
                return false;
            }

            Logger?.Information("Good definition for this axis " + AxisConfiguration.Name);
            return true;
        }

        public bool IsHomePosConfiguredValid()
        {
            if ((AxisConfiguration.PositionHome < AxisConfiguration.PositionMin) || (AxisConfiguration.PositionHome > AxisConfiguration.PositionMax))
            {
                Logger?.Warning(AxisConfiguration.PositionHome.ToString() + " for the Home Position is not ranged between " + AxisConfiguration.PositionMin.ToString() + " and " + AxisConfiguration.PositionMax.ToString());
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

        public double CurrentSpeed { get; set; }

        public double CurrentAccel { get; set; }

        public Length CurrentPos { get; set; }

        public bool Enabled { get; set; }

        public bool EnabledPrev { get; set; }

        public bool Moving { get; set; }

        public bool MovingPrev { get; set; }

        public Message DeviceError { get; set; }
        public string Name { get => AxisConfiguration.Name; }
        public virtual string AxisID { get => AxisConfiguration.AxisID; }

        public MovingDirection MovingDirection => _axisConfig.MovingDirection;

        public Length PositionMin => _axisConfig.PositionMin;
        public Length PositionMax => _axisConfig.PositionMax;

        #endregion Properties
    }
}
