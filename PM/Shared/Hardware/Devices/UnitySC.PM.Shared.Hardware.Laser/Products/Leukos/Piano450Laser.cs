using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Laser
{
    public class Piano450Laser : LaserBase
    {
        private Piano450LaserController _controller;

        public Piano450Laser(IGlobalStatusServer globalStatusServer, ILogger logger, LaserConfig config, LaserController laserController) :
            base(globalStatusServer, logger, config, laserController)
        {
            _controller = (Piano450LaserController)laserController;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void PowerOn()
        {
            _controller.PowerOn();
        }

        public override void PowerOff()
        {
            _controller.PowerOff();
        }

        public override void SetPower(double power)
        {
            _controller.SetPower(power);
        }

        public override void ReadPower()
        {
            _controller.ReadPower();
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
