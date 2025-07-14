using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EmeraMotionAxesService : DuplexServiceBase<IMotionAxesServiceCallback>, IEmeraMotionAxesService, IMotionAxesServiceCallbackProxy
    {
        private readonly object _lock = new object();
        private readonly HardwareManager _hardwareManager;
        private MotionAxesBase Axes => _hardwareManager.MotionAxes;

        private const string DeviceName = "Axes";

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to Motion axes change");
                    Subscribe();
                }
            });
        }

        public Response<VoidResult> UnsubscribeToAxesChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to Motion axes change");
                    Unsubscribe();
                }
            });
        }

        public EmeraMotionAxesService(ILogger<AxesService> logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
        }

        public Response<VoidResult> GoToPosition(XYZPosition targetPosition, AxisSpeed speed)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    Axes.GoToPosition(targetPosition, speed);
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }
            });
        }

        public Response<bool> Move(params PMAxisMove[] moves)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    Axes.Move(moves);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<bool> RelativeMove(params PMAxisMove[] moves)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    Axes.RelativeMove(moves);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<bool> GoToHome(AxisSpeed speed = 0)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    Axes.Home(speed);                    
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<bool> WaitMotionEnd(int timeout)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    Axes.WaitMotionEnd(timeout);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (Axes == null)
                {
                    return null;
                }

                PositionBase axesPosition = null;
                try
                {
                    axesPosition = Axes.GetPosition();
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }
                return axesPosition;
            });
        }
        public Response<AxesState> GetCurrentState()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (Axes == null)
                {
                    return null;
                }

                AxesState axesState = null;
                try
                {
                    axesState = Axes.GetCurrentState();
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }

                return axesState;
            });
        }

        public Response<bool> StopAllMotion()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    Axes.StopAllMotion();
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        if (Axes == null)
                        {
                            return;
                        }

                        Axes.TriggerUpdateEvent();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<bool> GoToEfemLoad(Length waferDiameter, AxisSpeed speed)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {                   
                    var slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
                    if (slotConfig == null)
                    {
                        throw new Exception($"No slot config found for diameter {waferDiameter}");
                    }
                    var targetPosition = slotConfig.PositionPark;
                    Axes.GoToPosition(targetPosition, speed);

                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    _logger.Error("GoToEfemLoad", ex);
                    return false;
                }
            });

        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    var slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
                    if (slotConfig == null)
                    {
                        throw new Exception($"No slot config found for diameter {waferDiameter}");
                    }
                    var targetPosition = slotConfig.PositionManualLoad;
                    Axes.GoToPosition(targetPosition, speed);

                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    _logger.Error("GoToManualLoad", ex);
                    return false;
                }
            });
        }

        public void PositionChanged(PositionBase position)
        {
            InvokeCallback(axesServiceCallback => axesServiceCallback.PositionChangedCallback(position));
        }

        public void DeviceStateChanged(DeviceState deviceState)
        {
            InvokeCallback(axesServiceCallback => axesServiceCallback.DeviceStateChangedCallback(deviceState));
        }

        public void StateChanged(AxesState axesState)
        {
            InvokeCallback(axesServiceCallback => axesServiceCallback.StateChangedCallback(axesState));
        }

        public void EndMove(bool targetReached)
        {
            InvokeCallback(axesServiceCallback => axesServiceCallback.EndMoveCallback(targetReached));
        }

        public Response<AxesConfig> GetAxesConfiguration()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (Axes == null) // Initialization not terminated
                {
                    return null;
                }

                AxesConfig axesConfig = null;
                try
                {
                    axesConfig = Axes.AxesConfiguration;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }
                return axesConfig;
            });
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            string userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }          
    }
}
