using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Light
{
    public class ENTTECLight : LightBase
    {
        public override DeviceFamily Family => DeviceFamily.Light;

        private new ENTTECLightConfig Config => (ENTTECLightConfig)base.Config;

        private ENTTECLightController _controller;
        private readonly NICouplerController _ioController;

        private readonly DigitalOutput _linkedOutput;

        private readonly Dictionary<string, int> _lightIdToAddress = new Dictionary<string, int>();
        private LightBase _antagonistLight;

        public ENTTECLight(LightConfig config, LightController lightController, ControllerBase ioController, IGlobalStatusServer globalStatusServer, ILogger logger) :
           base(config, globalStatusServer, logger)
        {
            _controller = (ENTTECLightController)lightController;

            if (Config.LinkedIo is null) return;

            _ioController = (NICouplerController)ioController;
            _linkedOutput = (DigitalOutput)_ioController.GetOutput(Config.LinkedIo.Name);
        }

        public override void Init()
        {
            if (!(Config.AntagonistLightId is null))
            {
                _antagonistLight = this;
            }
            base.Init();
        }

        public override double GetIntensity()
        {
            return _controller.GetIntensity(Config.DeviceID);
        }

        public override void SetIntensity(double intensity)
        {
            bool hasLinkedIo = !(Config.LinkedIo is null);
            bool turningLightOn = intensity != 0;
            if (hasLinkedIo && turningLightOn)
            {
                _ioController.DigitalWrite(_linkedOutput, Config.OutputActivationValue);
            }
            bool hasAntagonistLight = !(Config.AntagonistLightId is null);
            if (hasAntagonistLight && turningLightOn)
            {
                _antagonistLight.SetIntensity(0);
            }

            _controller.SetIntensity(Config.DeviceID, intensity);
            base.SetIntensity(intensity);
        }
    }
}
