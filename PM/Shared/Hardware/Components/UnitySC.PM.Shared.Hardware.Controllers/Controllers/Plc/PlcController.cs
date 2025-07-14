using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc
{
    public abstract class PlcController : ControllerBase
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        protected ControllerConfig ControllerConfig;

        internal string DeviceName = "Plc";

        protected PlcController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ControllerConfig = controllerConfig;
        }

        public abstract void Restart();

        public abstract void Reboot();

        public abstract void StartTriggerOutEmitSignal(double pulseDuration_ms = 1);

        public abstract void SmokeDetectorResetAlarm();

        public abstract void CustomCommand(string custom);
    }
}
