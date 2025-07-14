using UnitySC.PM.EME.Hardware.Chiller.Controller;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Hardware.Chiller
{
    public class Chiller : ChillerBase
    {
        private readonly IChillerController _controller;
        private double _temperature;

        public Chiller(IChillerController controller, IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
            _controller = controller;
        }

        public override DeviceFamily Family => DeviceFamily.Other;

        public override void SetTemperature(double value)
        {
            _controller.SetTemperature(value);
        }
        
        public override void SetFanSpeed(double value)
        {
            _controller.SetFanSpeed(value);
        }

        public override void SetMaxCompressionSpeed(int value)
        {
            _controller.SetMaxCompressionSpeed(value);
        }

        public override void SetConstantFanSpeedMode(ConstFanSpeedMode value)
        {
            _controller.SetConstantFanSpeedMode(value);
        }

        public void Reset()
        {
            _controller.Reset();
        }

        public void SetChillerMode(ChillerMode value)
        {
            _controller.SetChillerMode(value);
        }
    }
}
