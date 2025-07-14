using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters
{
    public class Sh10pilShutterDummyController : ShutterController
    {
        private readonly ILogger _logger;

        private enum EFeedbackMsgShutter
        {
            State = 1,
            ShutterIrisPositionMsg = 10
        }

        public Sh10pilShutterDummyController(OpcControllerConfig opcControllerConfig, IGlobalStatusServer globalStatusServer,
            ILogger logger) : base(opcControllerConfig, globalStatusServer, logger)
        {
            _logger = logger;
        }

        public override void Init(List<Message> initErrors)
        {
            Logger.Information("Init Sh10pilShutterController as dummy");
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

        public override void OpenIris()
        {
            Task.Delay(1000).Wait();
            DeliverMessages(EFeedbackMsgShutter.ShutterIrisPositionMsg.ToString(), "Open");
        }

        public override void CloseIris()
        {
            Task.Delay(1000).Wait();
            DeliverMessages(EFeedbackMsgShutter.ShutterIrisPositionMsg.ToString(), "Close");
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void DeliverMessages(string msgName, object value)
        {
            try
            {
                EFeedbackMsgShutter index = 0;
                Enum.TryParse(msgName, out index);

                switch (index)
                {
                    case EFeedbackMsgShutter.State:
                        State = new DeviceState((DeviceStatus)(int)value);
                        break;

                    case EFeedbackMsgShutter.ShutterIrisPositionMsg:
                        string sMsg = (string)value;
                        if (!String.IsNullOrWhiteSpace(sMsg))
                            Messenger.Send(new ShutterIrisPositionMessage() { ShutterIrisPosition = sMsg });
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
