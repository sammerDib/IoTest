using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Aerotech.Ensemble;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Owis;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Owis
{
    public class OwisMotionController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private OpcControllerConfig _opcControllerConfig;
        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ILogger _logger;
        private OpcController _opcController;

        private UnknownPosition _position;
        public bool IsInMoving { get; set; } 

        private enum OwisMotionCmds
        { ClearError, MotorInit, Reset, StopMotion, HomePosition, MoveAbsPosition, CustomCommand, RaisePropertiesChanged }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;

        private enum EFeedbackMsgOwisAxes
        {
            State = 0,
            StateMsg = 1,
            AxisStateMsg = 2,
            IsAliveMsg = 3,
            DrivingCurrentMsg = 10,
            HoldingCurrentMsg,
            LimitSwitchMsg,
            ErrMsg,
            PositionMsg = 20,
            IsInMovingMsg,
            CurrentPositionCounterMsg,
            CustomMsg = 50
        }

        public OwisMotionController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            _opcControllerConfig = opcControllerConfig;
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));

            _stageServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            _position = new UnknownPosition(new MotorReferential(), 0.0);
        }

        public override void Init(List<Message> initErrors)
        {
            _opcController.Init(initErrors);
        }

        public override bool ResetController()
        {
            try
            {
                if (_opcController != null)
                {
                    Disconnect();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void Connect()
        {
            _opcController.Connect();
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
            _opcController.Disconnect();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override void CheckControllerIsConnected()
        {
        }

        public override void StopAllMotion()
        {
            Thread.Sleep(500);
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            // TODO: Replace Thread.Sleep(2000) with proper motion detection logic using WaitMotionEnd.
            // The current temporary fix allows the filter wheel to be selected, 
            // but a proper implementation of WaitMotionEnd is needed to handle motion completion efficiently.
            Thread.Sleep(2000);
        }

        public override void InitializeAllAxes(List<Message> initErrors)
        {
        }

        public override bool IsAxisManaged(IAxis axis)
        {
            return _axisIdToAxisMasks.Keys.Any(x => x == axis.AxisID);
        }

        public override void HomeAllAxes()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.HomePosition.ToString());
            ChangeAxisToHomePosition();
        }

        public override PositionBase GetPosition()
        {
            return _position;
        }

        public override void RefreshAxisState(IAxis axis)
        {
            axis.Enabled = true; // TODO: fetch value from hardware
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgOwisAxes index = 0;
                EFeedbackMsgOwisAxes.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgOwisAxes.State:
                        if (int.TryParse(value.ToString(), out var valueState))
                        {
                            if (valueState >= 0)
                            {
                                State = new DeviceState((DeviceStatus)valueState);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }
                        break;

                    case EFeedbackMsgOwisAxes.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgOwisAxes.AxisStateMsg:
                        if (int.TryParse((string)value, out var statusId))
                        {
                            //_statusCode = (StatusCode)statusId;
                            //Messenger.Send(new StatusMessage() { StatusId = statusId, StatusCode = _statusCode });
                        }
                        break;

                    case EFeedbackMsgOwisAxes.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgOwisAxes.DrivingCurrentMsg :

                        break;

                    case EFeedbackMsgOwisAxes.HoldingCurrentMsg :

                        break;

                    case EFeedbackMsgOwisAxes.LimitSwitchMsg :

                        break;

                    case EFeedbackMsgOwisAxes.ErrMsg:
                        if (int.TryParse((string)value, out var errorId))
                        {
                        }
                        break;

                    case EFeedbackMsgOwisAxes.PositionMsg:
                        if (Double.TryParse((string)value, out var position))
                        {
                            _position.Position = position;
                            _stageServiceCallback.PositionChanged(_position);
                        }
                        break;

                    case EFeedbackMsgOwisAxes.IsInMovingMsg:
                        if (Boolean.TryParse((string)value, out var isInMoving))
                        {
                            IsInMoving = isInMoving;
                        }
                        break;

                    case EFeedbackMsgOwisAxes.CustomMsg:
                        Messenger.Send(new CustomMessage() { Custom = (string)value });
                        break;

                    default:
                        _logger.Warning($"{_opcControllerConfig.DeviceID} - Unknown message: {msgName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{_opcControllerConfig.DeviceID} - {ex.Message}: {(string)value}");
            }
        }

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                var normalizedAngle = move.Position.Value % 360;
                _opcController.SetMethodValueAsync(OwisMotionCmds.MoveAbsPosition.ToString(), normalizedAngle);
                ChangeAxisPosition(move.AxisId, move.Position.Value);
                IsInMoving = true;
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            throw new NotImplementedException("RelativeMove is not implemented");
        }

        public void ClearError()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.ClearError.ToString());
        }

        public void MotorInit()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.MotorInit.ToString());
        }

        public void Reset()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.Reset.ToString());
        }

        public void StopMotion()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.StopMotion.ToString());
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.RaisePropertiesChanged.ToString());
        }

        public void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(OwisMotionCmds.CustomCommand.ToString(), custom);
            Thread.Sleep(1000);
        }

        private void ChangeAxisToHomePosition()
        {
            foreach (var axis in AxisList)
            {
                //TODO: Call the controller directly to return the current position (it's missing or I haven't found it). 
                ChangeAxisPosition(axis.AxisID, 0.0);
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
