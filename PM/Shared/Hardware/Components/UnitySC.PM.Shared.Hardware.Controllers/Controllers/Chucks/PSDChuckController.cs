using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck
{
    public class PSDChuckController : USPChuckControllerBase, IChuckLoadingPosition
    {
        private ChuckState _chuckState;

        private OpcController _opcController;
        private Length _diameter = new Length(0, LengthUnit.Millimeter);
        private Dictionary<Length, MaterialPresence> _materialPresences;
        private Dictionary<Length, bool> _clampStates; // PSD = no clamp => always false
        private bool _lastChuckIsInLoadingPosition = false;
        private Dictionary<EFeedbackMsgPSDChuck, bool> _msgFeedbackReceived = new Dictionary<EFeedbackMsgPSDChuck, bool>();
        private bool LoadingPosition { get; set; }

        public Dictionary<Length, bool> ClampStates { get => _clampStates; set => _clampStates = value; }
        public Dictionary<Length, MaterialPresence> MaterialPresences { get => _materialPresences; set => _materialPresences = value; }
        public bool IsMaterialPresenceRefreshed { get => _msgFeedbackReceived[EFeedbackMsgPSDChuck.WaferPresenceMsg]; }
        
        private enum EChuckCmds
        { ChuckInLoadingPosition, RaisePropertiesChanged }

        private enum EFeedbackMsgPSDChuck
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            WaferPresenceMsg = 10,
            IsInLoadingPositionMsg
        }

        public PSDChuckController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _opcController = new OpcController(opcControllerConfig, logger, new DeliverMessagesDelegate(DeliverMessages));
            foreach (EFeedbackMsgPSDChuck key in Enum.GetValues(typeof(EFeedbackMsgPSDChuck)))
            {
                _msgFeedbackReceived.Add(key, false);
            }
            
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

        public void TriggerUpdateEvent()
        {
            // Reset feedback detection
            foreach (EFeedbackMsgPSDChuck key in Enum.GetValues(typeof(EFeedbackMsgPSDChuck)))
            {
                if(_msgFeedbackReceived.ContainsKey(key))
                    _msgFeedbackReceived[key] = false;
            }
            _opcController.SetMethodValueAsync(EChuckCmds.RaisePropertiesChanged.ToString());

        }        

        public void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgPSDChuck index = 0;
            try
            {
                if (!EFeedbackMsgPSDChuck.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgPSDChuck.State:
                        if (int.TryParse(value.ToString(), out var valueState))
                        {
                            if (valueState >= 0)
                            {
                                State = new DeviceState((DeviceStatus)valueState);
                                Messenger.Send(new StateMessage() { State = State });
                                _msgFeedbackReceived[index] = true;
                            }
                        }
                        break;

                    case EFeedbackMsgPSDChuck.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                            _msgFeedbackReceived[index] = true;
                        }
                        break;

                    case EFeedbackMsgPSDChuck.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgPSDChuck.WaferPresenceMsg:                        
                        if (!String.IsNullOrWhiteSpace((string)value) && MaterialPresences != null)
                        {
                            string[] fullMessages = new string[3];
                            fullMessages = ((string)value).Split(';');
                            double.TryParse(fullMessages[0], out var diameterValue);

                            var diameter = new Length(diameterValue, LengthUnit.Millimeter);
                            var presence = (MaterialPresence)Enum.Parse(typeof(MaterialPresence), fullMessages[1]);

                            if (MaterialPresences.ContainsKey(diameter))
                            {
                                if (MaterialPresences[diameter] != presence)
                                {
                                    MaterialPresences[diameter] = presence;
                                    if (presence != MaterialPresence.Unknown) // Filter unknown value => PLC use it to simulate a state changed, then Unknwon has no sense for material presence
                                    {
                                        Messenger.Send(new WaferPresenceMessage() { Diameter = diameter, WaferPresence = presence });
                                        _chuckState = new ChuckState(ClampStates, MaterialPresences);
                                        RaiseStateChangedEvent(_chuckState);
                                        _msgFeedbackReceived[index] = true;
                                    }
                                }
                            }
                        }
                        break;

                    case EFeedbackMsgPSDChuck.IsInLoadingPositionMsg:
                        if (Boolean.TryParse((string)value, out var loadingPosition))
                        {
                            if (_lastChuckIsInLoadingPosition != loadingPosition)
                            {
                                _lastChuckIsInLoadingPosition = loadingPosition;
                                LoadingPosition = _lastChuckIsInLoadingPosition;
                                Messenger.Send(new ChuckIsInLoadingPositionMessage() { IsInLoadingPosition = LoadingPosition });
                                _msgFeedbackReceived[index] = true;
                            }
                        }
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

        public override ChuckState GetState()
        {
            return _chuckState;
        }

        public void InitStatesWithChuckConfiguration(List<SubstrateSlotConfig> substrateSlotConfigs)
        {
            ClampStates = new Dictionary<Length, bool>();
            MaterialPresences = new Dictionary<Length, MaterialPresence>();

            foreach (var slotConfig in substrateSlotConfigs)
            {
                MaterialPresences.Add(slotConfig.Diameter, MaterialPresence.Unknown);
                ClampStates.Add(slotConfig.Diameter, false);
            }
        }

        public bool IsInLoadingPosition()
        {
            return _lastChuckIsInLoadingPosition;
        }

        public void SetChuckInLoadingPosition(bool loadingPosition)
        {
            _opcController.SetMethodValueAsync(EChuckCmds.ChuckInLoadingPosition.ToString(), loadingPosition);
        }
    }
}
