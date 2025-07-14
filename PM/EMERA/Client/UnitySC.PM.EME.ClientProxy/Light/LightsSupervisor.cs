using System.Collections.Generic;
using System.ServiceModel;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Light
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class LightsSupervisor : IEMELightService, IEMELightServiceCallback
    {
        private IMessenger _messenger;
        private readonly DuplexServiceInvoker<IEMELightService> _lightService;
        public LightsSupervisor(ILogger<IEMELightService> serviceLogger, IMessenger messenger)
        {
            _messenger = messenger;          
            var instanceContext = new InstanceContext(this);
            var serviceAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _lightService = new DuplexServiceInvoker<IEMELightService>(instanceContext, "EMERALightService", serviceLogger, messenger, s => s.SubscribeToChanges(), serviceAddress);         
        }              
        public Response<VoidResult> SubscribeToChanges()
        {
            return _lightService.TryInvokeAndGetMessages(l => l.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _lightService.TryInvokeAndGetMessages(l => l.UnSubscribeToChanges());
        }

        public Response<List<EMELightConfig>> GetLightsConfig()
        {
            return _lightService.TryInvokeAndGetMessages(l => l.GetLightsConfig());
        }

        public Response<VoidResult> InitLightSources()
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.InitLightSources());
            }
        }

        public Response<VoidResult> SwitchOn(string lightID, bool powerOn)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.SwitchOn(lightID, powerOn));
            }
        }

        public Response<VoidResult> SetLightPower(string lightID, double powerInPercent)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.SetLightPower(lightID, powerInPercent));
            }
        }

        public Response<VoidResult> RefreshPower(string lightID)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.RefreshPower(lightID));
            }
        }

        public Response<VoidResult> RefreshSwitchOn(string lightID)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.RefreshSwitchOn(lightID));
            }
        }

        public Response<VoidResult> RefreshLightSource(string lightID)
        {
            lock (this)
            {
                return _lightService.TryInvokeAndGetMessages(l => l.RefreshLightSource(lightID));
            }
        }

        public void UpdateLightSourceCallback(LightSourceMessage lightSource)
        {
            _messenger.Send(new LightSourceChangedMessage()
            {
                LightID = lightSource.LightID,
                SwitchOn = lightSource.SwitchOn,
                Power = lightSource.Power,
                Intensity = lightSource.Intensity,
                Temperature = lightSource.Temperature,
                OperatingTime = lightSource.OperatingTime
            });
        }
    }
}
