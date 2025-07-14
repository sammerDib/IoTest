using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Aerotech.Ensemble;
using Aerotech.Ensemble.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.IO
{
    public class DummyPSDIoMotionController : MotionControllerBase, IOpcMotion
    {
        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();

        private OpcControllerConfig _opcControllerConfig;
        private readonly Dictionary<string, AxisMask> _axisIdToAxisMasks = new Dictionary<string, AxisMask>();
        private readonly ILogger _logger;
        private double _position;
        private LinearPosition _linearPosition;

        private enum IoMotionCmds
        { MoveAbsPosition, RaisePropertiesChanged }

        private IMotionAxesServiceCallbackProxy _stageServiceCallback;

        private enum EFeedbackMsgIO
        {
            State = 1,
            PositionMsg = 10,
            IdMsg
        }

        public DummyPSDIoMotionController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) :
            base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
            _opcControllerConfig = opcControllerConfig;
             
            _stageServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            _linearPosition = new LinearPosition(new MotorReferential(), _position);
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init PSDIoMotionController as dummy");
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
            Thread.Sleep(500); 
        }

        public override void WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            Thread.Sleep(500); 
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
            return _linearPosition;
        }

        public override void RefreshAxisState(IAxis axis)
        {
            throw new NotImplementedException();
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgIO index = 0;

                EFeedbackMsgIO.TryParse(msgName, out index);
                switch (index)
                {
                    case EFeedbackMsgIO.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgIO.PositionMsg:
                        _position = Double.TryParse((string)value, out var tempPosition) ? tempPosition : -1;
                        if (_position != -1)
                        {
                            _linearPosition.Position = _position;
                            _stageServiceCallback.PositionChanged(_linearPosition);
                        }
                        break;

                    default:
                        _logger.Warning("EFeedbackMsgIO - Unknown message  : " + msgName);
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
            System.Threading.Tasks.Task.Run(() =>
            {
                DeliverMessages("PositionMsg", "0");
                Thread.Sleep(500);
                DeliverMessages("PositionMsg", moves[0].Position.Millimeters.ToString());
            });
        }

        public override void TriggerUpdateEvent()
        {
             
        }

        public void CustomCommand(string custom)
        {
             
        }

        public void RelativeMove(params PMAxisMove[] moves)
        { 
        }

        public override void RefreshCurrentPos(List<IAxis> axis)
        {
        }
    }
}
