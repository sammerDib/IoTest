using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LightService : DuplexServiceBase<ILightServiceCallback>, ILightService, ILightServiceCallbackProxy
    {
        private HardwareManager _hardwareManager;
        private const string DeviceName = "Light";

        public LightService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
        }

        Response<double> ILightService.GetLightIntensity(string lightID)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    var light = _hardwareManager.Lights[lightID];
                    return light.GetIntensity();
                }
                catch (Exception e)
                {
                    ReformulationMessage(messageContainer, e.Message);
                    return double.NaN;
                }
            });
        }

        Response<VoidResult> ILightService.SetLightIntensity(string lightID, double intensity)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        var light = _hardwareManager.Lights[lightID];
                        light.SetIntensity(intensity);
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public void LightIntensityChanged(string lightID, double intensity)
        {
            InvokeCallback(lightServiceCallback => lightServiceCallback.LightIntensityChangedCallback(lightID, intensity));
        }

        Response<VoidResult> ILightService.SubscribeToLightChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Subscribed to light service change");
                Subscribe();
            });
        }

        Response<VoidResult> ILightService.UnsubscribeToLightChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("UnSubscribed to light service change");
                Unsubscribe();
            });
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
