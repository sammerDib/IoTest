using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Wheel
{
    public abstract class WheelBase : DeviceBase
    {
        protected WheelBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
        }

        public override DeviceFamily Family => DeviceFamily.Wheel;

        public abstract void Init();

        public abstract double GetCurrentPosition();

        public abstract void Move(double targetPosition);

        public abstract void WaitMotionEnd(int timeout_ms, bool waitStabilization = true);
    }
}
