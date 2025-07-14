using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus
{
    public class Astrofan612FfuController : FfuController
    {

        private OpcController _opcController;
        private bool Refreshing { get; set; }
        public WarningMessage Warning { get; set; } = new WarningMessage();
        public AlarmMessage Alarm { get; set; } = new AlarmMessage();

        private IGlobalStatusServer _globalStatus;

        private enum EFfuCmds
        { PowerOn, PowerOff, SetSpeed, RaisePropertiesChanged, CustomCommand }

        private enum EFeedbackMsgFfu
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            CurrentSpeedMsg = 10,
            TemperatureMsg,
            WarningRaisedMsg = 20,
            AlarmRaisedMsg,
            CustomMsg = 50,
            RefreshingMsg = 60
        }

        public Astrofan612FfuController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));            

            _globalStatus = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
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
            _opcController.SetMethodValueAsync(EFfuCmds.PowerOn.ToString());
        }

        public override void PowerOff()
        {
            _opcController.SetMethodValueAsync(EFfuCmds.PowerOff.ToString());
        }

        public override void SetSpeed(ushort speedPercent)
        {
            _opcController.SetMethodValueAsync(EFfuCmds.SetSpeed.ToString(), speedPercent);
        }

        public override void CustomCommand(string custom)
        {
            _opcController.SetMethodValueAsync(EFfuCmds.CustomCommand.ToString(), custom);
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(EFfuCmds.RaisePropertiesChanged.ToString());
        }

        public void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgFfu index = 0;
            try
            {
                if (!EFeedbackMsgFfu.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgFfu.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgFfu.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgFfu.StatusMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            Messenger.Send(new StatusMessage() { Status = (string)value });
                        }
                        break;

                    case EFeedbackMsgFfu.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgFfu.CurrentSpeedMsg:
                        if (UInt16.TryParse((string)value, out var currentSpeed))
                            Messenger.Send(new CurrentSpeedMessage() { CurrentSpeed_percentage = currentSpeed });
                        break;

                    case EFeedbackMsgFfu.TemperatureMsg:
                        if (Double.TryParse((string)value, out var temperature))
                            Messenger.Send(new TemperatureMessage() { Temperature = temperature });
                        break;

                    case EFeedbackMsgFfu.WarningRaisedMsg:
                        if (Boolean.TryParse((string)value, out var warningRaised))
                        {
                            var warning = new WarningMessage();
                            warning.Triggered = warningRaised;

                            if (Warning.Triggered != warning.Triggered || Refreshing)
                            {
                                Warning.Triggered = warning.Triggered;
                                Messenger.Send(warning);

                                if (warning.Triggered)
                                    _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(ErrorID.FFUWarningError, MessageLevel.Error, "FFU pressure is detected under warning threshold")));
                            }
                        }
                        break;

                    case EFeedbackMsgFfu.AlarmRaisedMsg:
                        if (Boolean.TryParse((string)value, out var alarmRaised))
                        {
                            var alarm = new AlarmMessage();
                            alarm.Triggered = alarmRaised;

                            if (Alarm.Triggered != alarm.Triggered || Refreshing)
                            {
                                Alarm.Triggered = alarm.Triggered;
                                Messenger.Send(alarm);

                                if (alarm.Triggered)
                                    _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(ErrorID.FFUMajorError, MessageLevel.Error, "FFU pressure is detected below the threshold beyond the authorized period")));
                            }                            
                        }
                        break;

                    case EFeedbackMsgFfu.CustomMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                            Messenger.Send(new CustomMessage() { Custom = (string)value });
                        break;

                    case EFeedbackMsgFfu.RefreshingMsg:
                        if (Boolean.TryParse((string)value, out var refreshing))
                            Refreshing = refreshing;
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
