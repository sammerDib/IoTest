using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.Interlock;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers
{
    public class EMEChamberController : ChamberController, ISlitDoor, IChamberController
    {

        private OpcController _opcController;

        public Dictionary<uint, InterlockMessage> InterlockPanels { get; set; }
        public SlitDoorPosition SlitDoorState { get; set; }
        public bool ValveIsOpened { get; }

        private enum EFeedbackMsgEMEChamber
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            IsInMaintenanceMsg = 10,
            ArmNotExtendedMsg,
            EfemSlitDoorOpenPositionMsg,
            IsReadyToLoadUnloadMsg,
            InterlockPanelsMsg = 20,
            SlitDoorPositionMsg = 30,
            SlitDoorOpenPositionMsg,
            SlitDoorClosePositionMsg
        }

        private enum EMEChamberCmds
        { MoveSlitDoorPosition, StageInLoadingPosition, RaisePropertiesChanged }

        public EMEChamberController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
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

        public bool IsInMaintenance()
        {
            throw new NotImplementedException();
        }

        public bool PrepareToTransferState()
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
            _opcController.SetMethodValueAsync(EMEChamberCmds.RaisePropertiesChanged.ToString());
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgEMEChamber index = 0;
            try
            {
                if (!EFeedbackMsgEMEChamber.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgEMEChamber.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgEMEChamber.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgEMEChamber.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgEMEChamber.IsInMaintenanceMsg:
                        if (Boolean.TryParse((string)value, out var isInMaintenance))
                            Messenger.Send(new IsInMaintenanceMessage() { IsInMaintenance = isInMaintenance });
                        break;

                    case EFeedbackMsgEMEChamber.ArmNotExtendedMsg:
                        if (Boolean.TryParse((string)value, out var armNotExtended))
                            Messenger.Send(new ArmNotExtendedMessage() { ArmNotExtended = armNotExtended });
                        break;

                    case EFeedbackMsgEMEChamber.EfemSlitDoorOpenPositionMsg:
                        if (Boolean.TryParse((string)value, out var efemSlitDoorOpenPosition))
                            Messenger.Send(new EfemSlitDoorOpenPositionMessage() { EfemSlitDoorOpenPosition = efemSlitDoorOpenPosition });
                        break;

                    case EFeedbackMsgEMEChamber.IsReadyToLoadUnloadMsg:
                        if (Boolean.TryParse((string)value, out var isReadyToLoadUnload))
                            Messenger.Send(new IsReadyToLoadUnloadMessage() { IsReadyToLoadUnload = isReadyToLoadUnload });
                        break;

                    case EFeedbackMsgEMEChamber.InterlockPanelsMsg:
                        String[] message = ((string)value).Split(';');

                        if (uint.TryParse(message[2], out var interlockID))
                        {
                            InterlockMessage interlock = new InterlockMessage()
                            {
                                Name = message[0],
                                Description = message[1],
                                InterlockID = interlockID,
                                State = message[3]
                            };

                            Messenger.Send(interlock);

                            if (InterlockPanels != null)
                                InterlockPanels[interlockID] = interlock;
                        }

                        break;

                    case EFeedbackMsgEMEChamber.SlitDoorPositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            var position = (SlitDoorPosition)Enum.Parse(typeof(SlitDoorPosition), (string)value);
                            //if (SlitDoorState != position)
                            {
                                SlitDoorState = position;
                                Messenger.Send(new SlitDoorPositionMessage() { SlitDoorPosition = position });
                            }
                        }                        
                        break;

                    case EFeedbackMsgEMEChamber.SlitDoorOpenPositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var position))
                            {
                                Messenger.Send(new SlitDoorOpenPositionMessage() { SlitDoorOpenPosition = position });
                            }
                        }
                        break;

                    case EFeedbackMsgEMEChamber.SlitDoorClosePositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var position))
                            {
                                Messenger.Send(new SlitDoorClosePositionMessage() { SlitDoorClosePosition = position });
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }

        public void OpenSlitDoor()
        {
            _opcController.SetMethodValueAsync(EMEChamberCmds.MoveSlitDoorPosition.ToString(), (int)SlitDoorPosition.OpenPosition);
        }

        public void CloseSlitDoor()
        {
            _opcController.SetMethodValueAsync(EMEChamberCmds.MoveSlitDoorPosition.ToString(), (int)SlitDoorPosition.ClosePosition);
        }

        public void StageInLoadingPosition()
        {
            _opcController.SetMethodValueAsync(EMEChamberCmds.StageInLoadingPosition.ToString());
        }
    }
}
