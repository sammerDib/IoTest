using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class DummyLightController : LightController
    {
        private Dictionary<string, double> _lightIntensities = new Dictionary<string, double>();

        public DummyLightController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init LightController as dummy");
        }

        public override void Connect()
        {
            
        }

        public override void Connect(string deviceID)
        {
            Connect();
        }

        public override void Disconnect()
        {
           
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override bool ResetController()
        {
            return false;
        }

        public override double GetIntensity(string name)
        {
            return _lightIntensities[name];
        }

        public override void SetIntensity(string name, double intensity)
        {
            _lightIntensities[name] = intensity;
        }
    }
}
