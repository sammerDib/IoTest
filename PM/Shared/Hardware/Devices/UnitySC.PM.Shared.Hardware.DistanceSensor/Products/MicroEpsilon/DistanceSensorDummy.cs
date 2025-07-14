using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.DistanceSensor
{
    public class DistanceSensorDummy : DistanceSensorBase
    {
        private DistanceSensorDummyController _controller;
        
        public DistanceSensorDummy(IGlobalStatusServer globalStatusServer, ILogger logger, DistanceSensorConfig config, DistanceSensorController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _controller = (DistanceSensorDummyController)controller;
        }
        

        public override void Init()
        {
            base.Init();    
            Logger.Information("Init DistanceSensor as a Dummy");
        }

        public override double GetDistanceSensorHeight()
        {
            return _controller.GetDistanceSensorHeight();
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public override void CustomCommand(string custom)
        {
            _controller.CustomCommand(custom);
        }
    }
}
