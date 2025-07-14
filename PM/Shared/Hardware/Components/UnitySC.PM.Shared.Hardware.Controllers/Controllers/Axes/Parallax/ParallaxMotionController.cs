using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Aerotech.Ensemble;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Parallax;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Parallax
{
    public class ParallaxMotionController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private OpcControllerConfig _opcControllerConfig;
        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ILogger _logger;
        private OpcController _opcController;
        private double _position;
        private RotationPosition _rotPosition;

        private enum Std900Cmds
        { MoveAbsPosition, RaisePropertiesChanged }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;

        private enum EFeedbackMsgStd900
        {
            State = 1,
            PositionMsg = 10,
            IdMsg
        }

        public ParallaxMotionController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            _opcControllerConfig = opcControllerConfig;
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));

            _stageServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            _rotPosition = new RotationPosition(new MotorReferential(), _position);
        }

        public override void Init(List<Message> initErrors)
        {
            _opcController.Init(initErrors);
        }

        public override bool ResetController()
        {
            throw new NotImplementedException();
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

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            Thread.Sleep(500);
            //throw new NotImplementedException();
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
        }

        public override PositionBase GetPosition()
        {
            return _rotPosition;
        }

        public override void RefreshAxisState(IAxis axis)
        {
            throw new NotImplementedException();
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgStd900 index = 0;

                EFeedbackMsgStd900.TryParse(msgName, out index);
                switch (index)
                {
                    case EFeedbackMsgStd900.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgStd900.PositionMsg:
                        _position = Double.TryParse((string)value, out var tempPosition) ? tempPosition : -1;
                        if (_position != -1)
                        {
                            _rotPosition.Position = _position;
                            _stageServiceCallback.PositionChanged(_rotPosition);
                        }
                        break;

                    case EFeedbackMsgStd900.IdMsg:
                        Messenger.Send(new IdMessage() { Id = (string)value });
                        break;

                    default:
                        _logger.Warning("EFeedbackMsgStd900 - Unknown message  : " + msgName);
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
                _opcController.SetMethodValueAsync(Std900Cmds.MoveAbsPosition.ToString(), move.Position.Value);
            }
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(Std900Cmds.RaisePropertiesChanged.ToString());
        }

        public void CustomCommand(string custom)
        {
            throw new NotImplementedException();
        }

        public void RelativeMove(params PMAxisMove[] moves)
        {
            throw new NotImplementedException();
        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {
            throw new NotImplementedException();
        }
    }
}
