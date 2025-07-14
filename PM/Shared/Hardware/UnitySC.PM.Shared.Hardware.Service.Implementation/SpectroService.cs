using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SpectroService : DuplexServiceBase<ISpectroServiceCallback>, ISpectroService, ISpectroServiceCallbackProxy
    {
        private readonly HardwareManager _hardwareManager;
        private const string DeviceName = "Spectrometer";
        private readonly object _lockObj = new object ();


        public SpectroService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
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

        public Response<SpectroSignal> DoMeasure(SpectrometerParamBase param)
        {
            return InvokeDataResponse(messageContainer =>
            {
                SpectroSignal signal = new SpectroSignal();
                lock (_lockObj)
                {
                    try
                    {
                        signal = _hardwareManager.Spectrometers.Values.FirstOrDefault()?.DoMeasure(param);
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
                return signal;
            });
        }

        public Response<VoidResult> StartContinuousAcquisition(SpectrometerParamBase param)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lockObj)
                {
                    try
                    {
                        _hardwareManager?.Spectrometers.Values.FirstOrDefault()?.StartContinuousAcquisition(param);
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> StopContinuousAcquisition()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lockObj)
                {
                    try
                    {
                        _hardwareManager?.Spectrometers.Values.FirstOrDefault()?.StopContinuousAcquisition();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public void RawMeasuresCallback(SpectroSignal spectroSignal)
        {
            InvokeCallback(spectroServiceCallback => spectroServiceCallback.RawMeasuresCallback(spectroSignal));
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            string userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
