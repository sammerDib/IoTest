using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ionizers
{
    public class KeyenceIonizerDummyController : IonizerController
    {
        private readonly ILogger _logger;

        private enum EFeedbackMsgIonizer
        {
            State = 1,
            MoveOpenPositionMsg = 10
        }

        public KeyenceIonizerDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init KeyenceIonizerController as dummy");
        }

        public override bool ResetController()
        {
            throw new NotImplementedException();
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

        public override void OpenAirPneumaticValve()
        {
            DeliverMessages(EFeedbackMsgIonizer.MoveOpenPositionMsg.ToString(), "Opened");
        }

        public override void CloseAirPneumaticValve()
        {
            DeliverMessages(EFeedbackMsgIonizer.MoveOpenPositionMsg.ToString(), "Closed");
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgIonizer index = 0;
                Enum.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgIonizer.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgIonizer.MoveOpenPositionMsg:
                        if (Boolean.TryParse((string)value, out var valveIsOpened))
                            Messenger.Send(new AirPneumaticValveMessage() { ValveIsOpened = valveIsOpened });
                        break;

                    default:
                        _logger.Warning($"{ControllerConfig.DeviceID} - Unknown message: {msgName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ControllerConfig.DeviceID} - {ex.Message}: {(string)value}");
            }
        }
    }
}
