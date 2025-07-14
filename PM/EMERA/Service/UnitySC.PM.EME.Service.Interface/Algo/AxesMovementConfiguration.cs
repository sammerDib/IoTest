using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class AxesMovementConfiguration : DefaultConfiguration
    {
        public AxisSpeed Speed { get; set; } = AxisSpeed.Normal;
    }
}
