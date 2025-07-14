using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EMELightService : DuplexServiceBase<IEMELightServiceCallback>, IEMELightService
    {
        private const string DeviceName = "Light";
        private EmeHardwareManager _hardwareManager;

        public EMELightService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();

            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<LightSourceMessage>(this, (r, m) => { UpdateLightSource(m); });
        }

        public override void Init()
        {
            base.Init();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
            });
        }

        public Response<List<EMELightConfig>> GetLightsConfig()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                try
                {
                    if (_hardwareManager.EMELights.Count <= 0)
                        return null;

                    var lightList = new List<EMELightConfig>();
                    foreach (var light in _hardwareManager.EMELights)
                        lightList.Add(light.Value.Config);

                    return lightList;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messagesContainer, ex.Message, MessageLevel.Error);
                    return null;
                }
            });
        }

        public Response<VoidResult> InitLightSources()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                foreach (var light in _hardwareManager.EMELights)
                {
                    _hardwareManager.EMELights[light.Value.Config.DeviceID].InitLightSources();
                }
            });
        }

        public Response<VoidResult> SwitchOn(string lightID, bool powerOn)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.EMELights[lightID].SwitchOn(powerOn);
            });
        }

        public Response<VoidResult> SetLightPower(string lightID, double power)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.EMELights[lightID].SetPower(power);
            });
        }

        public Response<VoidResult> RefreshPower(string lightID)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.EMELights[lightID].RefreshPower();
            });
        }

        public Response<VoidResult> RefreshSwitchOn(string lightID)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.EMELights[lightID].RefreshSwitchOn();
            });
        }

        public Response<VoidResult> RefreshLightSource(string lightID)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.EMELights[lightID].RefreshLightSource();
            });
        }

        public void UpdateLightSource(LightSourceMessage lightSource)
        {
            InvokeCallback(i => i.UpdateLightSourceCallback(lightSource));
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var content = "Subscribed to light change";
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, content, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, content, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
