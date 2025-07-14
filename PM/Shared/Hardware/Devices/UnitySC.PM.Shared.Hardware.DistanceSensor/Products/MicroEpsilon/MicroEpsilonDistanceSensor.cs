using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.DistanceSensor
{
    public class MicroEpsilonDistanceSensor : DistanceSensorBase
    {
        private MicroEpsilonDistanceSensorController _controller;

        public MicroEpsilonDistanceSensor(IGlobalStatusServer globalStatusServer, ILogger logger, DistanceSensorConfig config, DistanceSensorController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _controller = (MicroEpsilonDistanceSensorController)controller;
        }

        public override void Init()
        {
            base.Init();
        }
                
        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public override double GetDistanceSensorHeight()
        {
            return _controller.GetDistanceSensorHeight();
        }

        public override void CustomCommand(string custom)
        {
            _controller.CustomCommand(custom);
        }
    }
}
