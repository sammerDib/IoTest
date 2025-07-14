using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Light
{
    public class DummyLight : LightBase
    {
        public override DeviceFamily Family => DeviceFamily.Light;

        private DummyLightController _controller;

        public DummyLight(LightConfig config, LightController lightController, IGlobalStatusServer globalStatusServer, ILogger logger) :
           base(config, globalStatusServer, logger)
        {
            _controller = (DummyLightController)lightController;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init light as dummy");
        }
        public override double GetIntensity()
        {
            return _controller.GetIntensity(DeviceID);
        }

        public override void SetIntensity(double intensity)
        {
            _controller.SetIntensity(DeviceID, intensity);
            base.SetIntensity(intensity);
        }
    }
}
