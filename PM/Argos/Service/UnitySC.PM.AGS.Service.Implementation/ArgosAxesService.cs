using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.AGS.Service.Interface.AxesService;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AxesService : DuplexServiceBase<IAxesServiceCallback>, IArgosAxesService, IAxesServiceCallbackProxy
    {
        private readonly object _lock = new object();
        private readonly ArgosHardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;
        private ArgosAxesBase Axes
        { get { return _hardwareManager.Axes; } }

        private const string DeviceName = "Axes";

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to Axes change");
                    Subscribe();
                    // Message to client exemple
                    ReformulationMessage(messageContainer, "Subscribed to Axes change", MessageLevel.Information);
                }
            });
        }

        public Response<VoidResult> UnsubscribeToAxesChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to Axes change");
                    Unsubscribe();

                    // Message to client exemple
                    ReformulationMessage(messageContainer, "Unsubscribed to Axes change", MessageLevel.Information);
                }
            });
        }

        public AxesService(ILogger<AxesService> logger, ArgosHardwareManager hardwareManager, IReferentialManager referentialManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            _referentialManager = referentialManager;
        }

        public Response<bool> StopAllMoves()
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

        /// <returns>Returns null if failed, AxesPosition if success</returns>
        public Response<PositionBase> GetCurrentPosition()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (Axes == null)
                    return null;

                PositionBase axesPosition = null;
                try
                {
                    axesPosition = Axes.GetPositon();
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }
                return axesPosition;
            });
        }

        public void PositionChanged(PositionBase position)
        {
            position = _referentialManager.ConvertTo(position, ReferentialTag.Wafer);
            InvokeCallback(axesServiceCallback => axesServiceCallback.PositionChangedCallback(position));
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

        public Response<bool> GoToHome(AxisSpeed speed)
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

        public Response<bool> GoToLoadUnload(AxisSpeed speed)
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                try
                {
                    Axes.GoToLoadUnload(speed);
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
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
