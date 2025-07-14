using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class DummyEvosensLightController : DummyArduinoLightController
    {
        public DummyEvosensLightController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : 
            base(opcControllerConfig, globalStatusServer, logger)
        {
        }
    }
}
