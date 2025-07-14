using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MotionAxesService : DuplexServiceBase<IMotionAxesServiceCallback>, IMotionAxesService, IMotionAxesServiceCallbackProxy
    {
        private readonly object _lock = new object();
        private readonly HardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;
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

        public MotionAxesService(ILogger<AxesService> logger, HardwareManager hardwareManager, IReferentialManager referentialManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            _referentialManager = referentialManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
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
                    return null;

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
                    return null;

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
                    return null;

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
