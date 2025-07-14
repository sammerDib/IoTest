//using TcEventLoggerAdsProxyLib;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Plc
{
    public class BeckhoffPlcDummy : PlcBase
    {
        private BeckhoffPlcDummyController _controller;

        public BeckhoffPlcDummy(IGlobalStatusServer globalStatusServer, ILogger logger, PlcConfig config, PlcController plcController) :
            base(globalStatusServer, logger, config, plcController)
        {
            _controller = (BeckhoffPlcDummyController)plcController;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init Plc as a Dummy");
        }

        public override void TriggerUpdateEvent()
        {            
        }

        public override void Restart()
        {
            _controller.Restart();
        }

        public override void Reboot()
        {
            _controller.Reboot();
        }

        public override void CustomCommand(string custom)
        {
            _controller.CustomCommand(custom);
        }

        public override void StartTriggerOutEmitSignal(double pulseDuration_ms = 1)
        {
            _controller.StartTriggerOutEmitSignal(pulseDuration_ms);
        }

        public override void SmokeDetectorResetAlarm()
        {
            _controller.SmokeDetectorResetAlarm();
        }
    }
}
