using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public abstract class LightController : ControllerBase
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        protected readonly ControllerConfig ControllerConfig;

        internal readonly string DeviceName = "Light";

        protected LightController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            ControllerConfig = controllerConfig;
        }

        //Intensity varies between 0 and 100
        public abstract double GetIntensity(string lightID);

        //Intensity varies between 0 and 100
        public abstract void SetIntensity(string lightID, double intensity);

        protected string FormatMessage(string message)
        {
            return ($"[{DeviceName}]{message}").Replace('\r', ' ').Replace('\n', ' ');
        }
    }
}
