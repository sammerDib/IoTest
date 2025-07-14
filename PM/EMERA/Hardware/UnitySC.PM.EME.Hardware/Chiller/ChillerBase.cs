using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Hardware.Chiller
{
    public abstract class ChillerBase : DeviceBase
    {
        protected ChillerBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
            
        }

        public abstract void SetTemperature(double value);
        public abstract void SetFanSpeed(double value);
        public abstract void SetMaxCompressionSpeed(int value);
        public abstract void SetConstantFanSpeedMode(ConstFanSpeedMode value);
    }
}
