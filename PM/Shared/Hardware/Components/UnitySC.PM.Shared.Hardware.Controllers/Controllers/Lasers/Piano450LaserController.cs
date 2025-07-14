using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser.Leukos;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser
{
    public class Piano450LaserController : LaserController
    {

        private OpcController _opcController;

        private enum Piano450Cmds
        { PowerOn, PowerOff, RaisePropertiesChanged, CustomCommand }

        private enum EFeedbackMsgPiano450
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            PowerMsg = 10,
            InterlockStatusMsg,
            LaserTemperatureMsg,
            CrystalTemperatureMsg,
            RunningTimeLaserOnMsg,
            RunningTimeElectroOnMsg,
            CustomMsg
        }

        public Piano450LaserController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
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

        public override void PowerOn()
        {
            _opcController.SetMethodValueAsync(Piano450Cmds.PowerOn.ToString());
        }

        public override void PowerOff()
        {
            _opcController.SetMethodValueAsync(Piano450Cmds.PowerOff.ToString());
        }

        public override void ReadPower()
        {
            throw new NotImplementedException();
        }

        public override void SetPower(double power)
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(Piano450Cmds.RaisePropertiesChanged.ToString());
        }

        public override void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(Piano450Cmds.CustomCommand.ToString(), custom);
        }

        public void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgPiano450 index = 0;
            try
            {
               if(! EFeedbackMsgPiano450.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgPiano450.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPiano450.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPiano450.StatusMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            bool laserStatus = Convert.ToBoolean(Convert.ToInt16((string)value));
                            Messenger.Send(new LaserStatusMessage() { LaserStatus = laserStatus });
                        }
                        break;

                    case EFeedbackMsgPiano450.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgPiano450.PowerMsg:
                        if (Double.TryParse((string)value, out var power))
                            Messenger.Send(new PowerMessage() { Power = power });
                        break;

                    case EFeedbackMsgPiano450.InterlockStatusMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                            Messenger.Send(new InterlockStatusMessage() { InterlockStatus = (string)value });
                        break;

                    case EFeedbackMsgPiano450.LaserTemperatureMsg:
                        if (Double.TryParse((string)value, out var laserTemperature))
                            Messenger.Send(new LaserTemperatureMessage() { LaserTemperature = laserTemperature });
                        break;

                    case EFeedbackMsgPiano450.CrystalTemperatureMsg:
                        if (Double.TryParse((string)value, out var crystalTemperature))
                            Messenger.Send(new CrystalTemperatureMessage() { CrystalTemperature = crystalTemperature });
                        break;

                    case EFeedbackMsgPiano450.RunningTimeLaserOnMsg:
                        if (Double.TryParse((string)value, out var runningTimeLaserOn))
                            Messenger.Send(new RunningTimeLaserOnMessage() { RunningTimeLaserOn = runningTimeLaserOn });
                        break;

                    case EFeedbackMsgPiano450.RunningTimeElectroOnMsg:
                        if (Double.TryParse((string)value, out var runningTimeElectroOn))
                            Messenger.Send(new RunningTimeElectroOnMessage() { RunningTimeElectroOn = runningTimeElectroOn });
                        break;

                    case EFeedbackMsgPiano450.CustomMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                            Messenger.Send(new CustomMessage() { Custom = (string)value });
                        break;

                    default:
                        Logger.Warning($"{ControllerConfig.DeviceID} - Unknown message: {msgName} {index}");
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
