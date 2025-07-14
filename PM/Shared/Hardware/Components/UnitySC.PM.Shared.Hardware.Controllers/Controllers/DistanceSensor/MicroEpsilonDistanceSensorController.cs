using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor
{
    public class MicroEpsilonDistanceSensorController : DistanceSensorController
    {

        private OpcController _opcController;
        private double _distance = double.NaN;

        private enum MicroEpsilonCmds
        { RaisePropertiesChanged, CustomCommand }

        private enum EFeedbackMsgMicroEpsilonDistanceSensor
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            DistanceMsg = 10,
            IdMsg,
            CustomMsg
        }

        public MicroEpsilonDistanceSensorController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
          ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
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

        public override double GetDistanceSensorHeight()
        {
            return _distance;
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(MicroEpsilonCmds.RaisePropertiesChanged.ToString());
        }        

        public override void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(MicroEpsilonCmds.CustomCommand.ToString(), custom);
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgMicroEpsilonDistanceSensor index = 0;
            try
            {
                if (!EFeedbackMsgMicroEpsilonDistanceSensor.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgMicroEpsilonDistanceSensor.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgMicroEpsilonDistanceSensor.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgMicroEpsilonDistanceSensor.DistanceMsg:
                        if (Double.TryParse((string)value, out var distance))
                        {
                            _distance = distance;
                            Messenger.Send(new DistanceMessage() { Distance = distance });
                        }
                        break;

                    case EFeedbackMsgMicroEpsilonDistanceSensor.IdMsg:
                        Messenger.Send(new IdMessage() { Id = (string)value });
                        break;

                    case EFeedbackMsgMicroEpsilonDistanceSensor.CustomMsg:
                        Messenger.Send(new CustomMessage() { Custom = (string)value });
                        break;

                    default:
                        Logger.Warning("EFeedbackMsgIFD2461 - Unknown message  : " + msgName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }
    }
}
