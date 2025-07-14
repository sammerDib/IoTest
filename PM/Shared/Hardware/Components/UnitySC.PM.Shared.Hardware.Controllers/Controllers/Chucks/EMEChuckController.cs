using System;
using System.Collections.Generic;
using System.Linq;

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
    public class EMEChuckController : USPChuckControllerBase, IChuckClamp, IChuckLoadingPosition
    {
        private ChuckState _chuckState;

        private OpcController _opcController;
        private Length _diameter = new Length(0, LengthUnit.Millimeter);
        private Dictionary<Length, MaterialPresence> _materialPresences;
        private Dictionary<Length, bool> _clampStates; // EME = no clamp => always false
        private bool _lastChuckIsInLoadingPosition = false;
        private bool LoadingPosition { get; set; }

        public Dictionary<Length, bool> ClampStates { get => _clampStates; set => _clampStates = value; }
        public Dictionary<Length, MaterialPresence> MaterialPresences { get => _materialPresences; set => _materialPresences = value; }
        public bool IsMaterialPresenceRefreshed { get => true; } // TODO: as DEMETER 

        private enum EChuckCmds
        { ClampWafer, ReleaseWafer, ChuckInLoadingPosition, RaisePropertiesChanged }

        private enum EFeedbackMsgChuck
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            WaferPresenceMsg = 10,
            IsInLoadingPositionMsg

        }

        public EMEChuckController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
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

        public void TriggerUpdateEvent()
        {
            _lastChuckIsInLoadingPosition = false;

            _opcController.SetMethodValueAsync(EChuckCmds.RaisePropertiesChanged.ToString());
        }        

        public void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgChuck index = 0;
            try
            {
                if (!EFeedbackMsgChuck.TryParse(msgName, out index))
                    Logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgChuck.State:
                        if (int.TryParse(value.ToString(), out var valueState))
                        {
                            if (valueState >= 0)
                            {
                                State = new DeviceState((DeviceStatus)valueState);
                                Messenger.Send(new StateMessage() { State = State });
                            }
                        }
                        break;

                    case EFeedbackMsgChuck.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgChuck.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;                    

                    case EFeedbackMsgChuck.WaferPresenceMsg:                        
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
                                    Messenger.Send(new WaferPresenceMessage() { Diameter = diameter, WaferPresence = presence });
                                    _chuckState = new ChuckState(ClampStates, MaterialPresences);
                                    RaiseStateChangedEvent(_chuckState);
                                }
                            }                                                                                   
                        }
                        break;

                    case EFeedbackMsgChuck.IsInLoadingPositionMsg:
                        if (Boolean.TryParse((string)value, out var loadingPosition))
                        {
                            if (_lastChuckIsInLoadingPosition != loadingPosition)
                            {
                                _lastChuckIsInLoadingPosition = loadingPosition;
                                LoadingPosition = _lastChuckIsInLoadingPosition;
                                Messenger.Send(new ChuckIsInLoadingPositionMessage() { IsInLoadingPosition = LoadingPosition });
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

        public void InitStatesWithChuckConfiguration(List<SubstSlotWithPositionConfig> substSlotWithPositionConfigs)
        {
            ClampStates = new Dictionary<Length, bool>();
            MaterialPresences = new Dictionary<Length, MaterialPresence>();

            foreach (var slotConfig in substSlotWithPositionConfigs)
            {
                MaterialPresences.Add(slotConfig.Diameter, MaterialPresence.Unknown);
                ClampStates.Add(slotConfig.Diameter, false);
            }
        }

        public void ClampWafer(Length wafer)
        {
            _opcController.SetMethodValueAsync(EChuckCmds.ClampWafer.ToString(), wafer.Millimeters);
            //TODO : update clamp state via controller instead of this weird method
            _chuckState.WaferClampStates[wafer] = true;
            _chuckState.WaferClampStates.Keys.Where(key => key != wafer).ToList()
                .ForEach(key => _chuckState.WaferClampStates[key] = false);
            RaiseStateChangedEvent(GetState());
        }

        public void ReleaseWafer(Length wafer)
        {
            _opcController.SetMethodValueAsync(EChuckCmds.ReleaseWafer.ToString(), wafer.Millimeters);
            //TODO : update clamp state via controller instead of this weird method
            _chuckState.WaferClampStates[wafer] = false;
            RaiseStateChangedEvent(GetState());
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
