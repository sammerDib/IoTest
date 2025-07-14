using System;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Parallax;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.FastAttenuation
{
    public class Std900FastAttenuation : FastAttenuationBase
    {
        private const double NOTIFICATION_INTERVAL = 20;
        private ILogger _logger;

        private enum Std900Cmds
        { MoveAbsPosition, RaisePropertiesChanged }

        private enum EFeedbackMsgStd900
        {
            State = 1,
            PositionMsg = 10,
            IdMsg
        }

        private Std900FastAttenuationConfig _fastAttenuationConfig;
        private OpcDevice _opcDevice;

        public Std900FastAttenuation(string name, string id)
        {
            Name = name;
            DeviceID = id;
        }

        public override void Init(FastAttenuationConfig config)
        {
            if (!(config is Std900FastAttenuationConfig))
                throw new Exception("Invalid configuration type");

            _fastAttenuationConfig = (Std900FastAttenuationConfig)config;
            Configuration = config;

            _logger = new HardwareLogger(config.LogLevel.ToString(), Family.ToString(), config.Name);
            _logger.Information("Init the device Std900FastAttenuationConfig");
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        private void DeliverMessages(string msgName, object value)
        {
            EFeedbackMsgStd900 index = 0;

            EFeedbackMsgStd900.TryParse(msgName, out index);
            switch (index)
            {
                case EFeedbackMsgStd900.State:
                    State = new DeviceState((DeviceStatus)(int)value);
                    break;

                case EFeedbackMsgStd900.PositionMsg:
                    double position = Double.TryParse((string)value, out var tempPosition) ? tempPosition : -1;
                    if (position != -1)
                        Messenger.Send(new FastAttenuationPositionMessage() { Position = position });
                    break;

                case EFeedbackMsgStd900.IdMsg:
                    Messenger.Send(new IdMessage() { Id = (string)value });
                    break;

                default:
                    _logger.Warning("EFeedbackMsgStd900 - Unknown message  : " + msgName);
                    break;
            }
        }

        public override void MoveAbsPosition(double position)
        {
            throw new NotImplementedException();
        }

        public override void TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }
    }
}
