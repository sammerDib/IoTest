using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser
{
    public abstract class LaserController : ControllerBase
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        protected ControllerConfig ControllerConfig;

        internal string DeviceName = "Laser";

        protected LaserController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ControllerConfig = controllerConfig;
        }

        public abstract void PowerOn();

        public abstract void PowerOff();

        public abstract void TriggerUpdateEvent();

        public abstract void CustomCommand(string custom);

        public abstract void SetPower(double power);

        public abstract void ReadPower();
    }
}
