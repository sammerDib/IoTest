using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AxesService : DuplexServiceBase<IAxesServiceCallback>, IAxesService, IAxesServiceCallbackProxy
    {
        private readonly object _lock = new object();
        private readonly HardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;
        private IAxes _axes => _hardwareManager.Axes;

        private const string DeviceName = "Axes";

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to Axes change");
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
                    _logger.Information("Unsubscribed to Axes change");
                    Unsubscribe();
                }
            });
        }

        public AxesService(ILogger<AxesService> logger, HardwareManager hardwareManager, IReferentialManager referentialManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            _referentialManager = referentialManager;
        }

        public Response<bool> MoveIncremental(IncrementalMoveBase move, AxisSpeed speed)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.MoveIncremental(move, speed);
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "MoveIncremental");
                    return false;
                }
            });
        }

        public Response<bool> GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom);
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "GotoPointCustomSpeedAccel");
                    return false;
                }
            });
        }

        public Response<bool> GotoPosition(PositionBase position, AxisSpeed speed)
        {
            if (!(position is XYPosition) && !(position is XYZTopZBottomPosition))
            {
                _logger.Error($"Received position is not a XYZTopZBottomPosition, neither a XYPosition");
                return new Response<bool>
                {
                    Result = false,
                    Messages = new List<Message>
                    {
                        new Message(MessageLevel.Error, "Received type is not supported"),
                    },
                };
            }

            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.GotoPosition(position, speed);
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    return false;
                }
            });
        }

        public Response<bool> StopAllMoves()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.StopAllMoves();
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "StopAllMoves");
                    return false;
                }
            });
        }

        public Response<bool> WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.WaitMotionEnd(timeout, waitStabilization);
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "WaitMotionEnd");
                    return false;
                }
            });
        }

        /// <returns>Returns null if failed, AxesPosition if success</returns>
        public Response<PositionBase> GetCurrentPosition()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_axes == null)
                {
                    return null;
                }

                PositionBase axesPosition = null;
                try
                {
                    axesPosition = _axes.GetPos();
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "GetCurrentPosition");
                }

                return axesPosition;
            });
        }

        /// <returns>Returns null if failed, AxesState if success</returns>
        public Response<AxesState> GetCurrentState()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_axes == null)
                {
                    return null;
                }

                AxesState axesState = null;
                try
                {
                    axesState = _axes.GetState();
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(exception, "GetCurrentState");
                }

                return axesState;
            });
        }

        public void PositionChanged(PositionBase position)
        {
            //Console.WriteLine("Position changed Before : " + position.ToString());
            position = _referentialManager.ConvertTo(position, ReferentialTag.Wafer);
            //Console.WriteLine("Position changed After : " + position.ToString());
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
                if (_axes == null) // Initialization not terminated
                {
                    return null;
                }

                AxesConfig axesConfig = null;
                try
                {
                    axesConfig = _axes.AxesConfiguration;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error("GetAxesConfiguration", exception);
                }

                return axesConfig;
            });
        }

        public Response<bool> GotoSpecificPosition(SpecificPositions positionRefId, Length waferDiameter, AxisSpeed speed)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    SubstrateSlotConfig slotConfig = null;
                    if (waferDiameter != null)
                    {
                        slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
                        if (slotConfig == null)
                        {
                            throw new Exception($"No slot config found for diameter {waferDiameter}");
                        }
                    }

                    switch (positionRefId)
                    {
                        case SpecificPositions.PositionPark:
                            _axes.GotoPosition(slotConfig.PositionPark, speed);
                            return true;
                        case SpecificPositions.PositionManualLoad:
                            _axes.GotoPosition(slotConfig.PositionManualLoad, speed);
                            return true;
                        case SpecificPositions.PositionChuckCenter:
                            _axes.GotoPosition(slotConfig.PositionChuckCenter, speed);
                            return true;
                        case SpecificPositions.PositionWaferCenter:
                            var waferCenterPosition = new XYPosition(new WaferReferential(), 0d, 0d);
                            _axes.GotoPosition(waferCenterPosition, speed);
                            return true;
                        case SpecificPositions.PositionHome:
                            _axes.GotoHomePos(speed);
                            return true;
                        default:
                            throw new Exception($"Unknown position reference");
                    }
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error(positionRefId.GetDescription(), exception);
                    return false;
                }
            });
        }

        public Response<bool> GoToHome(AxisSpeed speed)
        {
            return GotoSpecificPosition(SpecificPositions.PositionHome, null, speed);
        }

        public Response<bool> GoToPark(Length waferDiameter, AxisSpeed speed)
        {
            return GotoSpecificPosition(SpecificPositions.PositionPark, waferDiameter, speed);
        }

        public Response<bool> GoToChuckCenter(Length waferDiameter, AxisSpeed speed)
        {
            return GotoSpecificPosition(SpecificPositions.PositionChuckCenter, waferDiameter, speed);
        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            return GotoSpecificPosition(SpecificPositions.PositionManualLoad, waferDiameter, speed);
        }

        public Response<PositionBase> GetChuckCenterPosition(Length waferDiameter)
        {
            return InvokeDataResponse<PositionBase>(messageContainer =>
            {
                try
                {
                    var slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(waferDiameter);
                    if (slotConfig == null)
                    {
                        throw new Exception($"No slot config found for diameter {(int)waferDiameter.Millimeters}");
                    }
                    return slotConfig.PositionChuckCenter;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error($"GetChuckCenterPosition ({(int)waferDiameter.Millimeters} mm)", exception);
                    return (new XYPosition( new StageReferential(), 0.0, 0.0));
                }
            });
        }

        public Response<bool> StopLanding()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.StopLanding();
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error("StopLanding", exception);
                    return false;
                }
            });
        }

        public Response<bool> Land()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _axes.Land();
                    return true;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, exception.Message);
                    _logger.Error("Land", exception);
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

        public Response<bool> ResetAxis()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _logger.Information("Reset Axis");

                    _axes.ResetAxis();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error("ResetAxis error : ", ex);
                    return false;
                }
            });
        }

        public async Task<Response<VoidResult>> ResetZTopFocus()
        {
            _logger.Information("Reset ZBottom focus (LOH)");
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                _axes.InitZTopFocus();
            });
        }

        public Response<bool> AcknowledgeResetAxis()
        {
            return InvokeDataResponse(messageContainer =>
            {
                _logger.Information("Acknowledge Axis");

                _axes.AcknowledgeResetAxis();
                return true;
            });
        }

        public async Task<Response<VoidResult>> ResetZBottomFocus()
        {
            _logger.Information("Reset ZBottom focus (LOH)");
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                _axes.InitZBottomFocus();
            });
        }
    }
}
