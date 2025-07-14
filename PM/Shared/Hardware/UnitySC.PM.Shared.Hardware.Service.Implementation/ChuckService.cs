using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChuckService : DuplexServiceBase<IChuckServiceCallback>, IChuckService, IChuckServiceCallbackProxy
    {
        private readonly HardwareManager _hardwareManager;
        private readonly object _lock = new object();

        private const string DeviceName = "Chuck";

        public ChuckService(ILogger<ChuckService> logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
        }

        public void StateChanged(ChuckState chuckState)
        {
            InvokeCallback(chuckServiceCallback => chuckServiceCallback.StateChangedCallback(chuckState));
        }

        public Response<VoidResult> SubscribeToChuckChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to chuck change");
                    Subscribe();
                }
            });
        }

        /// <returns>Returns null if failed, ChuckState if success</returns>
        public Response<ChuckState> GetCurrentState()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Chuck == null)
                {
                    return null;
                }

                ChuckState chuckState = null;
                try
                {
                    chuckState = _hardwareManager.Chuck.GetState();
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }

                return chuckState;
            });
        }

        public Response<VoidResult> UnsubscribeToChuckChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to chuck change");
                    Unsubscribe();

                    // Message to client exemple
                    ReformulationMessage(messageContainer, "Unsubscribed to chuck change", MessageLevel.Information);
                }
            });
        }

        public Response<ChuckBaseConfig> GetChuckConfiguration()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Chuck == null) // Initialization not terminated
                {
                    return null;
                }

                ChuckBaseConfig config = null;
                try
                {
                    config = _hardwareManager.Chuck.Configuration;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }

                return config;
            });
        }

        public Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _hardwareManager.ClampHandler.ClampWafer(wafer.Diameter);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _hardwareManager.ClampHandler.ReleaseWafer(wafer.Diameter);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            string userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            //if (!string.IsNullOrEmpty(userContent))
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }

        public async Task<Response<VoidResult>> ResetAirbearing()
        {
            lock (_lock)
            {
                _logger.Information("Reset Airbearing");
            }

            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _hardwareManager.AirBearingHandler.InitAirbearing());
            });
        }

        public async Task<Response<VoidResult>> ResetWaferStage()
        {
            lock (_lock)
            {
                _logger.Information("Reset WaferStage");
            }

            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() => _hardwareManager.InitializationHandler.InitWaferStage());
            });
        }

        public Response<Dictionary<string, float>> GetSensorValues()
        {
            return InvokeDataResponse(messaVgeContainer =>
            {
                var pressuresValues = _hardwareManager.AirBearingHandler.GetAirBearingPressuresValues();
                return pressuresValues;
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Chuck is USPChuckBase chuckBase)
                {
                    chuckBase.TriggerUpdateEvent();
                }
            });
        }

        public Response<MaterialPresence> GetWaferPresence()
        {
            throw new NotImplementedException();
        }
    }
}
