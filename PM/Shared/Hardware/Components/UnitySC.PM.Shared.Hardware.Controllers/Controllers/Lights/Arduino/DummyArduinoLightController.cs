using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class DummyArduinoLightController : LightController, IPowerLightController
    {
        private readonly Dictionary<string, SimulatedLightSource> _lightSources =
            new Dictionary<string, SimulatedLightSource>();

        private const double DummyTemperature = 20.0;

        public DummyArduinoLightController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(opcControllerConfig, globalStatusServer, logger)
        {
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init ArduinoLightController as dummy");
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

        public void SetPower(string lightID, double powerInPercent)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                _lightSources[lightID] = CreateSimulatedLight(lightID);
            }

            _lightSources[lightID].Power = powerInPercent;
            SendLightMessage(lightID);
        }

        public double GetPower(string lightID)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                return 0;
            }
            return _lightSources[lightID].Power;
        }

        public void RefreshPower(string lightID)
        {
            SendLightMessage(lightID);
        }

        public void InitLightSources()
        {
        }

        public void SwitchOn(string lightID, bool powerOn)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                _lightSources[lightID] = CreateSimulatedLight(lightID);
            }

            _lightSources[lightID].SwitchOn = powerOn;
            SendLightMessage(lightID);
        }

        public void RefreshSwitchOn(string lightID)
        {
            SendLightMessage(lightID);
        }

        public void RefreshLightSource(string lightID)
        {
            SendLightMessage(lightID);
        }

        public override double GetIntensity(string lightID)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                _lightSources[lightID] = CreateSimulatedLight(lightID);
            }

            return _lightSources[lightID].Intensity;
        }

        public override void SetIntensity(string lightID, double intensity)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                _lightSources[lightID] = CreateSimulatedLight(lightID);
            }

            _lightSources[lightID].Intensity = intensity;
            SendLightMessage(lightID);
        }

        private void SendLightMessage(string lightID)
        {
            if (!_lightSources.ContainsKey(lightID))
            {
                _lightSources[lightID] = CreateSimulatedLight(lightID);
            }

            var message = new LightSourceMessage
            {
                LightID = lightID,
                SwitchOn = _lightSources[lightID].SwitchOn,
                Power = _lightSources[lightID].Power,
                Intensity = _lightSources[lightID].Intensity,
                Temperature = DummyTemperature
            };
            Messenger.Send(message);
        }

        private SimulatedLightSource CreateSimulatedLight(string lightID)
        {
            return new SimulatedLightSource
            {
                LightID = lightID,
                SwitchOn = false,
                Power = 0,
                Intensity = 0,
                Temperature = DummyTemperature
            };
        }
    }

    internal class SimulatedLightSource
    {
        public string LightID { get; set; }
        public bool SwitchOn { get; set; }
        public double Power { get; set; }
        public double Intensity { get; set; }
        public double Temperature { get; set; }
    }
}
