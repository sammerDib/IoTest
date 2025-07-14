using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeLightsSupervisor : IEMELightService
    {
        private readonly IMessenger _messenger;

        public FakeLightsSupervisor(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public List<EMELightConfig> Configs { get; set; } = new List<EMELightConfig>
        {
            new EMELightConfig
            {
                Name = "ddf0deg",
                DeviceID = "3",
                Description = "Directional Dark Field source at 0 degree"
            },
            new EMELightConfig
            {
                Name = "ddf90deg",
                DeviceID = "4",
                Description = "Directional Dark Field source at 90 degree"
            }
        };

        public Response<List<EMELightConfig>> GetLightsConfig()
        {
            return new Response<List<EMELightConfig>> { Result = Configs };
        }

        public Response<VoidResult> InitLightSources()
        {
            return new Response<VoidResult> { Result = new VoidResult() };            
        }

        public Response<VoidResult> RefreshLightSource(string lightID)
        {
            return new Response<VoidResult> { Result = new VoidResult() };
        }

        public Response<VoidResult> RefreshPower(string lightID)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> RefreshSwitchOn(string lightID)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SetLightPower(string lightID, double power)
        {
            _messenger.Send(new LightSourceChangedMessage
            {
                LightID = lightID,
                SwitchOn = true,
                Power = power,
                Intensity = 42.0,
                Temperature = 24.0
            });
            return new Response<VoidResult>();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SwitchOn(string lightID, bool powerOn)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new NotImplementedException();
        }
    }
}
