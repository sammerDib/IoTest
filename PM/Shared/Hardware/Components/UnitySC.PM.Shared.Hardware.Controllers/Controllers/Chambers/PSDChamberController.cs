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
    public class PSDChamberController : ChamberController, ISlitDoor, ICdaPneumaticValve, IChamberController, IChamberFFUControl
    {

        private OpcController _opcController;
        private PMAccessMode _lastChamberAccesMode = PMAccessMode.Unknown;
        private SlitDoorPosition _lastSlitdoorState = SlitDoorPosition.UnknownPosition;
        private bool Refreshing { get; set; }
        private PMAccessMode ChamberAccesMode { get; set; } = PMAccessMode.Unknown;
        private Dictionary<uint, InterlockMessage> _lastInterlockPanels { get; set; }
        public Dictionary<uint, InterlockMessage> InterlockPanels { get; set; }
        public SlitDoorPosition SlitDoorState { get; set; }
        public bool ValveIsOpened { get; }
        private bool _ffuState;

        private enum EFeedbackMsgPsdChamber
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
            SlitDoorClosePositionMsg,
            CdaValveIsOpenMsg,
            RefreshingMsg,
        }

        private enum PsdChamberCmds
        { MoveSlitDoorPosition, MoveCdaPneumaticValve, RaisePropertiesChanged }

        public PSDChamberController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
            InterlockPanels = new Dictionary<uint, InterlockMessage>();
            _lastInterlockPanels = new Dictionary<uint, InterlockMessage>();
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

        public void TurnOnFFU()
        {
            _ffuState = true;
            //TODO: trun On FFU
            // _opcController.SetMethodValueAsync(PsdChamberCmds.XXXXXXXXX.ToString());
        }

        public void TurnOffFFU()
        {
            _ffuState = false;
            //TODO: trun OFF FFU
            // _opcController.SetMethodValueAsync(PsdChamberCmds.XXXXXXXXX.ToString());
        }

        public bool GetFFUErrorState()
        {
            //TODO: update ffu state
            // return FFUStateOk;
            throw new NotImplementedException();
        }

        public bool CdaIsReady()
        {
            throw new NotImplementedException();
        }

        public bool IsInMaintenance()
        {
            return (ChamberAccesMode == PMAccessMode.Maintenance);
        }

        public bool PrepareToTransferState()
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
            _lastInterlockPanels.Clear();
            _lastChamberAccesMode = PMAccessMode.Unknown;
            _lastSlitdoorState = SlitDoorPosition.UnknownPosition;

            _opcController.SetMethodValueAsync(PsdChamberCmds.RaisePropertiesChanged.ToString());
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgPsdChamber index = 0;
            try
            {
                if (!EFeedbackMsgPsdChamber.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgPsdChamber.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPsdChamber.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPsdChamber.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgPsdChamber.IsInMaintenanceMsg:
                        if (Boolean.TryParse((string)value, out var isInMaintenance))
                        {
                            ChamberAccesMode = isInMaintenance ? PMAccessMode.Maintenance : PMAccessMode.Run;
                            if (_lastChamberAccesMode != ChamberAccesMode)
                            {
                                _lastChamberAccesMode = ChamberAccesMode;
                                Messenger.Send(new IsInMaintenanceMessage() { IsInMaintenance = isInMaintenance });
                            }
                        }
                        break;

                    case EFeedbackMsgPsdChamber.ArmNotExtendedMsg:
                        if (Boolean.TryParse((string)value, out var armNotExtended))
                            Messenger.Send(new ArmNotExtendedMessage() { ArmNotExtended = armNotExtended });
                        break;

                    case EFeedbackMsgPsdChamber.EfemSlitDoorOpenPositionMsg:
                        if (Boolean.TryParse((string)value, out var efemSlitDoorOpenPosition))
                            Messenger.Send(new EfemSlitDoorOpenPositionMessage() { EfemSlitDoorOpenPosition = efemSlitDoorOpenPosition });
                        break;

                    case EFeedbackMsgPsdChamber.IsReadyToLoadUnloadMsg:
                        if (Boolean.TryParse((string)value, out var isReadyToLoadUnload))
                            Messenger.Send(new IsReadyToLoadUnloadMessage() { IsReadyToLoadUnload = isReadyToLoadUnload });
                        break;

                    case EFeedbackMsgPsdChamber.InterlockPanelsMsg:
                        String[] message = ((string)value).Split(';');

                        if (uint.TryParse(message[2], out var interlockID))
                        {
                            if ((interlockID < 1) || (interlockID > 5)) return;

                            InterlockMessage interlock = new InterlockMessage();
                            interlock.Name = message[0];
                            interlock.Description = message[1];
                            interlock.InterlockID = interlockID;
                            interlock.State = message[3];

                            if (!InterlockPanels.ContainsKey(interlockID))
                            {
                                InterlockPanels.Add(interlockID, interlock);
                                Messenger.Send(interlock);
                            }
                            else
                                InterlockPanels[interlockID].State = interlock.State;

                            if (!_lastInterlockPanels.ContainsKey(interlockID))
                                _lastInterlockPanels.Add(interlockID, interlock);

                            if (_lastInterlockPanels[interlockID].State != InterlockPanels[interlockID].State || Refreshing)
                            {
                                _lastInterlockPanels[interlockID].State = InterlockPanels[interlockID].State;
                                Messenger.Send(interlock);
                            }
                        }

                        break;

                    case EFeedbackMsgPsdChamber.SlitDoorPositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            var position = (SlitDoorPosition)Enum.Parse(typeof(SlitDoorPosition), (string)value);
                            SlitDoorState = position;
                            if (SlitDoorState != _lastSlitdoorState)
                            {
                                _lastSlitdoorState = SlitDoorState;
                                Messenger.Send(new SlitDoorPositionMessage() { SlitDoorPosition = position });
                            }
                        }
                        break;

                    case EFeedbackMsgPsdChamber.SlitDoorOpenPositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var position))
                            {
                                Messenger.Send(new SlitDoorOpenPositionMessage() { SlitDoorOpenPosition = position });
                            }
                        }
                        break;

                    case EFeedbackMsgPsdChamber.SlitDoorClosePositionMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var position))
                            {
                                Messenger.Send(new SlitDoorClosePositionMessage() { SlitDoorClosePosition = position });
                            }
                        }
                        break;

                    case EFeedbackMsgPsdChamber.CdaValveIsOpenMsg:
                        if (!String.IsNullOrWhiteSpace((string)value))
                        {
                            if (Boolean.TryParse((string)value, out var valveIsOpened))
                            {
                                Messenger.Send(new CdaPneumaticValveMessage() { ValveIsOpened = valveIsOpened });
                            }
                        }
                        break;

                    case EFeedbackMsgPsdChamber.RefreshingMsg:
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

        public void OpenSlitDoor()
        {
            _opcController.SetMethodValueAsync(PsdChamberCmds.MoveSlitDoorPosition.ToString(), (int)SlitDoorPosition.OpenPosition);
        }

        public void CloseSlitDoor()
        {
            _opcController.SetMethodValueAsync(PsdChamberCmds.MoveSlitDoorPosition.ToString(), (int)SlitDoorPosition.ClosePosition);
        }

        public void OpenCdaPneumaticValve()
        {
            _opcController.SetMethodValueAsync(PsdChamberCmds.MoveCdaPneumaticValve.ToString(), true);
        }

        public void CloseCdaPneumaticValve()
        {
            _opcController.SetMethodValueAsync(PsdChamberCmds.MoveCdaPneumaticValve.ToString(), false);
        }

        public bool FFUState()
        {
            return _ffuState;
        }
    }
}
