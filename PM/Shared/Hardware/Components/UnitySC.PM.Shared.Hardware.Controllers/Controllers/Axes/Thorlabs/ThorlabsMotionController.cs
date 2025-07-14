using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
    public class ThorlabsMotionController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private OpcControllerConfig _opcControllerConfig;
        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private OpcController _opcController;

        private StatusCode _statusCode;
        private UnknownPosition _position;

        private enum ThorlabsMotionCmds
        { JogBackwardPosition, JogForwardPosition, MoveAbsPosition, MoveRelPosition, HomePosition, RaisePropertiesChanged, CustomCommand }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;

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

        public ThorlabsMotionController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
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
            //throw new NotImplementedException();
        }

        public override void WaitMotionEnd(int timeout_ms, bool waitStabilization = true)
        {
            bool isSuccess = SpinWait.SpinUntil(() =>
            {
                Thread.Sleep(500);
                return _statusCode == StatusCode.NoError;
            }, timeout_ms);

            if (!isSuccess)
            {
                throw new Exception($"Error with slider position : {_statusCode}");
            }
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
            _opcController.SetMethodValueAsync(ThorlabsMotionCmds.HomePosition.ToString());
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
                            _opcController.NewMeterSubscription = isAlive;
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
                        Logger.Warning($"{_opcControllerConfig.DeviceID} - Unknown message: {msgName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{_opcControllerConfig.DeviceID} - {ex.Message}: {(string)value}");
            }
        }

        public void Move(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _opcController.SetMethodValueAsync(ThorlabsMotionCmds.MoveAbsPosition.ToString(), move.Position.Value);
                ChangeAxisPosition(move.AxisId, move.Position.Value);
            }
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            foreach (var move in moves)
            {
                _opcController.SetMethodValueAsync(ThorlabsMotionCmds.MoveRelPosition.ToString(), move.Position.Value);
            }
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(ThorlabsMotionCmds.RaisePropertiesChanged.ToString());
        }

        public void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(ThorlabsMotionCmds.CustomCommand.ToString(), custom);
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
