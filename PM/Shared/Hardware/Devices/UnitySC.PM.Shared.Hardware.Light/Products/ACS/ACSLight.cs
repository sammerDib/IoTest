using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Light
{
    public class ACSLight : LightBase
    {
        public override DeviceFamily Family => DeviceFamily.Light;

        private new ACSLightConfig Config => (ACSLightConfig)base.Config;

        private ACSLightController _controller;

        public ACSLight(LightConfig config, LightController lightController, IGlobalStatusServer globalStatusServer, ILogger logger) :
           base(config, globalStatusServer, logger)
        {
            _controller = (ACSLightController)lightController;
        }

        public override double GetIntensity()
        {
            return _controller.GetIntensity(Config.Control);
        }

        public override void SetIntensity(double intensity)
        {
            if (intensity > 0)
                _controller.SetPower(Config.Power, 1);
            else
                _controller.SetPower(Config.Power, 0);
            _controller.SetIntensity(Config.Control, intensity);
            base.SetIntensity(intensity);
        }
    }
}
