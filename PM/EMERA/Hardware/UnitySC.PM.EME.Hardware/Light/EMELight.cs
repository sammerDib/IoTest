using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Hardware.Light
{
    public class EMELight : EMELightBase
    {
        public override DeviceFamily Family => DeviceFamily.Light;

        private readonly IPowerLightController _controller;

        public EMELight(EMELightConfig config, IPowerLightController lightController,
            IGlobalStatusServer globalStatusServer, ILogger logger) :
           base(config, globalStatusServer, logger)
        {
            _controller = lightController;
        }

        public override void SetPower(double power)
        {
            _controller.SetPower(Config.DeviceID, power);
        }

        public override double GetPower()
        {
            return _controller.GetPower(Config.DeviceID);
        }

        public override void RefreshPower()
        {
            _controller.RefreshPower(Config.DeviceID);
        }

        public override void InitLightSources()
        {
            _controller.InitLightSources();
        }        

        public override void SwitchOn(bool powerOn)
        {
            _controller.SwitchOn(Config.DeviceID, powerOn);
        }
        
        public override void RefreshSwitchOn()
        {
            _controller.RefreshSwitchOn(Config.DeviceID);
        }

        public override void RefreshLightSource()
        {
            _controller.RefreshLightSource(Config.DeviceID);
        }
    }
}
