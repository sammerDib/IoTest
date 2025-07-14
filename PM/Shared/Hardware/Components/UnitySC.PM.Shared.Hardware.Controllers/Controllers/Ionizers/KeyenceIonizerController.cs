using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ionizers
{
    public class KeyenceIonizerController : IonizerController
    {

        private OpcController _opcController;
        private bool Refreshing { get; set; }
        private PMAccessMode ChamberAccesMode { get; set; } = PMAccessMode.Unknown;
        public Dictionary<uint, AlarmMessage> Alarms { get; set; }
        public bool ValveIsOpened { get; }

        private IGlobalStatusServer _globalStatus;

        private enum EFeedbackMsgKeyenceIonizer
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            AlarmMsg = 10,
            AirValveIsOpenMsg,
            StaticEliminationInterruptMsg,
            RefreshingMsg
        }

        private enum KeyenceIonizerCmds
        { MoveAirPneumaticValve, EnableStaticEliminationInterrupt, RaisePropertiesChanged }

        public KeyenceIonizerController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
            Alarms = new Dictionary<uint, AlarmMessage>();

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

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(KeyenceIonizerCmds.RaisePropertiesChanged.ToString());
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgKeyenceIonizer index = 0;
            try
            {
                if (!EFeedbackMsgKeyenceIonizer.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgKeyenceIonizer.State:
                        if (int.TryParse(value.ToString(), out var valueState))
                        {
                            if (valueState >= 0)
                            {
                                State = new DeviceState((DeviceStatus)valueState);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }
                        break;

                    case EFeedbackMsgKeyenceIonizer.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgKeyenceIonizer.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgKeyenceIonizer.AlarmMsg:
                        if (String.IsNullOrWhiteSpace((string)value))
                            return;

                        String[] message = ((string)value).Split(';');

                        if (uint.TryParse(message[2], out var alarmID))
                        {
                            bool alarmIsTriggered;

                            AlarmMessage alarm = new AlarmMessage();
                            alarm.Name = message[0];
                            alarm.Description = message[1];
                            alarm.AlarmID = alarmID;

                            Boolean.TryParse(message[3], out alarmIsTriggered);
                            alarm.Triggered = alarmIsTriggered;

                            if (!Alarms.ContainsKey(alarmID))
                            {
                                Alarms.Add(alarmID, alarm);
                                Messenger.Send(alarm);

                                if (alarm.Triggered)
                                    _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(ErrorID.IonizerError, MessageLevel.Error, Alarms[alarmID].Description)));
                            }

                            if (Alarms[alarmID].Triggered != alarm.Triggered || Refreshing)
                            {
                                Alarms[alarmID].Triggered = alarm.Triggered;
                                Messenger.Send(alarm);

                                if (alarm.Triggered)
                                    _globalStatus.SetGlobalStatus(new GlobalStatus(new Message(ErrorID.IonizerError, MessageLevel.Error, Alarms[alarmID].Description)));
                            }
                        }

                        break;

                    case EFeedbackMsgKeyenceIonizer.AirValveIsOpenMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var valveIsOpened))
                            {
                                Messenger.Send(new AirPneumaticValveMessage() { ValveIsOpened = valveIsOpened });
                            }
                        }
                        break;

                    case EFeedbackMsgKeyenceIonizer.StaticEliminationInterruptMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var isEnabled))
                            {
                                Messenger.Send(new StaticEliminationInterruptMessage() { IsEnabledStaticEliminationInterrupt = isEnabled });
                            }
                        }
                        break;

                    case EFeedbackMsgKeyenceIonizer.RefreshingMsg:
                        if (Boolean.TryParse((string)value, out var refreshing))
                            Refreshing = refreshing;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }

        public override void OpenAirPneumaticValve()
        {
            _opcController.SetMethodValueAsync(KeyenceIonizerCmds.MoveAirPneumaticValve.ToString(), true);
        }

        public override void CloseAirPneumaticValve()
        {
            _opcController.SetMethodValueAsync(KeyenceIonizerCmds.MoveAirPneumaticValve.ToString(), false);
        }
    }
}
