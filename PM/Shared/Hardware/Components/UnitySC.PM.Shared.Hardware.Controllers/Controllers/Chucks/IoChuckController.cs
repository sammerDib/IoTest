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
    public class IoChuckController : USPChuckControllerBase
    {
        private readonly ILogger _logger;

        private OpcController _opcController;
        private Length _diameter = new Length(0, LengthUnit.Millimeter);
        private Dictionary<Length, MaterialPresence> _waferPresenceSensors;

        private enum EChuckCmds
        { RaisePropertiesChanged }

        private enum EFeedbackMsgPSDChuck
        {
            State = 0,
            StateMsg = 1,
            StatusMsg = 2,
            IsAliveMsg = 3,
            WaferPresenceMsg = 10
        }

        public IoChuckController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
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
            _opcController.SetMethodValueAsync(EChuckCmds.RaisePropertiesChanged.ToString());
        }        

        public void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgPSDChuck index = 0;
            try
            {
                if (!EFeedbackMsgPSDChuck.TryParse(msgName, out index))
                    _logger.Error($"{ControllerConfig.DeviceID} - unable to parse enum value: <{msgName}>");

                switch (index)
                {
                    case EFeedbackMsgPSDChuck.State:
                        if ((int)value >= 0)
                        {
                            State = new DeviceState((DeviceStatus)(int)value);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPSDChuck.StateMsg:
                        if (int.TryParse((string)value, out var state))
                        {
                            State = new DeviceState((DeviceStatus)state);
                            Messenger.Send(new StateMessage() { State = State });
                        }
                        break;

                    case EFeedbackMsgPSDChuck.IsAliveMsg:
                        if (int.TryParse((string)value, out var isAlive))
                        {
                            _opcController.NewMeterSubscription = isAlive;
                        }
                        break;

                    case EFeedbackMsgPSDChuck.WaferPresenceMsg:                        
                        if (!String.IsNullOrWhiteSpace((string)value) && _waferPresenceSensors != null)
                        {
                            string[] fullMessages = new string[3];
                            fullMessages = ((string)value).Split(';');
                            double.TryParse(fullMessages[0], out var diameterValue);

                            var diameter = new Length(diameterValue, LengthUnit.Millimeter);
                            var presence = (MaterialPresence)Enum.Parse(typeof(MaterialPresence), fullMessages[1]);

                            if (_waferPresenceSensors.ContainsKey(diameter))
                            {
                                _waferPresenceSensors[diameter] = presence;
                                Messenger.Send(new WaferPresenceMessage() { Diameter = diameter, WaferPresence = presence });
                            }                                                                                   
                        }
                        break;

                    default:
                        _logger.Warning($"{ControllerConfig.DeviceID} - Unknown message: {msgName} {index}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value} {index}");
            }
        }

        public Dictionary<Length, MaterialPresence> CheckWafersPresence()
        {
            return _waferPresenceSensors;
        }

        public void InitWaferPresenceSensors(Dictionary<Length, MaterialPresence> waferPresenceSensors)
        {
            _waferPresenceSensors = waferPresenceSensors;
        }

        public override ChuckState GetState()
        {
            throw new NotImplementedException();
        }
    }
}
