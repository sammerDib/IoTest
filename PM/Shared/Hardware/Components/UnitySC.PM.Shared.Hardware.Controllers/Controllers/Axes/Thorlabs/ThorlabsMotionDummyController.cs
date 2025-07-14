using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Aerotech.Ensemble;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Thorlabs;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Thorlabs
{
    public class ThorlabsMotionDummyController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ILogger _logger;
        private UnknownPosition _position;

        private enum ThorlabsMotionCmds
        {
            JogBackwardPosition, JogForwardPosition, MoveAbsPosition, MoveRelPosition, HomePosition,
            RaisePropertiesChanged, CustomCommand
        }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;
        private StatusCode _statusCode;

        private enum EFeedbackMsgThorlabsAxes
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            PositionMsg = 10,
            IdMsg,
            CustomMsg,
        }

        public ThorlabsMotionDummyController(OpcControllerConfig opcControllerConfig,
            IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;

            _stageServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            _position = new UnknownPosition(new MotorReferential(), 0.0);
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init ThorlabsMotionController as dummy");
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void Connect()
        {
        }

        public override void Connect(string deviceId)
        {
        }

        public override void Disconnect()
        {
        }

        public override void Disconnect(string deviceID)
        {
        }

        public override void CheckControllerIsConnected()
        {
        }

        public override void StopAllMotion()
        {
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            Task.Delay(500).Wait();
        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
            try
            {
                foreach (var axis in AxisList)
                {
                    if (axis is ThorlabsSliderAxis thorlabsAxis)
                    {
                        thorlabsAxis.Enabled = true;
                        thorlabsAxis.Initialized = true;
                        thorlabsAxis.Moving = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                Logger?.Error("InitializationAllAxes - ThorlabsMotionDummyController: " + Ex.Message);
                throw;
            }
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public override void HomeAllAxes()
        {
            _position.Position = 0.0;
        }

        public override PositionBase GetPosition()
        {
            return _position;
        }

        public override void RefreshAxisState(IAxis axis)
        {
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgThorlabsAxes index = 0;
                EFeedbackMsgThorlabsAxes.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgThorlabsAxes.State:
                        if (int.TryParse(value.ToString(), out var valueState))
                        {
                            if (valueState >= 0)
                            {
                                State = new DeviceState((DeviceStatus)valueState);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }

                        break;

                    case EFeedbackMsgThorlabsAxes.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }

                        break;

                    case EFeedbackMsgThorlabsAxes.StatusMsg:
                        if (int.TryParse((string)value, out var statusId))
                        {
                            _statusCode = (StatusCode)statusId;
                            Messenger.Send(new StatusMessage() { StatusId = statusId, StatusCode = _statusCode });
                        }

                        break;

                    case EFeedbackMsgThorlabsAxes.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            // Nothing
                        }

                        break;

                    case EFeedbackMsgThorlabsAxes.PositionMsg:
                        if (Double.TryParse((string)value, out var position))
                        {
                            _position.Position = position;
                            _stageServiceCallback.PositionChanged(_position);
                        }

                        break;

                    case EFeedbackMsgThorlabsAxes.IdMsg:
                        Messenger.Send(new IdMessage() { Id = (string)value });
                        break;

                    case EFeedbackMsgThorlabsAxes.CustomMsg:
                        Messenger.Send(new CustomMessage() { Custom = (string)value });
                        break;

                    default:
                        _logger.Warning($"{ControllerConfiguration.DeviceID} - Unknown message: {msgName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ControllerConfiguration.DeviceID} - {ex.Message}: {(string)value}");
            }
        }

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                ChangeAxisPosition(move.AxisId, move.Position.Value);
            }
        }

        private void ChangeAxisPosition(string axisId, double position)
        {
            switch (axisId)
            {
                case "Linear":
                    RaisePositionChangedEvent(new LinearPosition(new MotorReferential(), position));
                    break;
                case "Rotation":
                    RaisePositionChangedEvent(new RotationPosition(new MotorReferential(), position));
                    break;
                default:
                    break;
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _position.Position += move.Position.Millimeters;
            }
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void CustomCommand(string custom)
        {
        }

        public override void RefreshCurrentPos(List<IAxis> axesList)
        {
            foreach (var axis in axesList)
            {
                switch (axis.AxisID)
                {
                    case "Linear":
                        axis.CurrentPos = _position.Position.Millimeters();
                        break;
                    case "Rotation":
                        axis.CurrentPos = _position.Position.Millimeters();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
