using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;

using Workstation.ServiceModel.Ua;

namespace UnitySC.PM.Shared.Hardware.FiberSwitch
{
    public class EOLFiberSwitch : FiberSwitchBase
    {
        private const double NOTIFICATION_INTERVAL = 20;
        private ILogger _logger;

        private enum EOLCmds
        { SetPosition, RaisePropertiesChanged, CustomCommand }

        private enum EFeedbackMsgEOL
        {
            State = 1,
            PositionMsg = 10,
            IdMsg,
            CustomMsg
        }

        private EOLFiberSwitchConfig _fiberSwitchConfig;

        public EOLFiberSwitch(string name, string id)
        {
            Name = name;
            DeviceID = id;
        }

        public override void Init(FiberSwitchConfig config)
        {
            if (!(config is EOLFiberSwitchConfig))
                throw new Exception("Invalid configuration type");

            _fiberSwitchConfig = (EOLFiberSwitchConfig)config;
            Configuration = config;

            _logger = new HardwareLogger(config.LogLevel.ToString(), Family.ToString(), config.Name);
            _logger.Information("Init the device EOLFiberSwitch");
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgEOL index = 0;
            EFeedbackMsgEOL.TryParse(msgName, out index);

            switch (index)
            {
                case EFeedbackMsgEOL.State:
                    State = new DeviceState((DeviceStatus)(int)value);
                    break;

                case EFeedbackMsgEOL.PositionMsg:
                    Messenger.Send(new PositionMessage() { Position = (string)value });
                    break;

                case EFeedbackMsgEOL.IdMsg:
                    Messenger.Send(new IdMessage() { Id = (string)value });
                    break;

                case EFeedbackMsgEOL.CustomMsg:
                    Messenger.Send(new CustomMessage() { Custom = (string)value });
                    break;

                default:
                    _logger.Warning("EFeedbackMsgEOL - Unknown message  : " + msgName);
                    break;
            }
        }

        public override void SetPosition(int position)
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }

        public override void CustomCommand(string customCmd)
        {
            throw new NotImplementedException();
        }

        public override void GetPosition()
        {
            throw new NotImplementedException();
        }
    }
}
