using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

using Workstation.ServiceModel.Ua;

namespace UnitySC.PM.Shared.Hardware.Mppc
{
    public class C13336Mppc : MppcBase
    {
        private const double NOTIFICATION_INTERVAL = 20;

        private enum C13336Cmds
        {
            TempCorrectionFactorSetting, SwitchTempCompensationMode, RefVoltageTempSetting, PowerFctSetting,
            OutputVoltageOn, OutputVoltageOff, PowerReset, RaisePropertiesChanged, ManageRelays, SetOutputVoltage
        }

        private enum EFeedbackMsgC13336
        {
            State = 1,
            MonitorInfoStatusMsg = 10,
            OutputCurrentMsg,
            OutputVoltageMsg,
            OutputVoltageSettingMsg,
            HighVoltageStatusMsg,
            TemperatureMsg,
            SensorTemperatureMsg,
            PowerFctReadMsg,
            TempCorrectionFactorReadMsg,
            FirmwareMsg,
            IdentifierMsg,
            CustomMsg,
            StateSignalsMsg
        }

        private C13336MppcConfig _mppcConfig;

        public C13336Mppc(string name, string id,ILogger logger)
        {
            Name = name;
            DeviceID = id;
            Logger = logger;
        }

        public override void Init(MppcConfig config)
        {
            if (!(config is C13336MppcConfig))
                throw new Exception("Invalid configuration type");

            _mppcConfig = (C13336MppcConfig)config;
            Configuration = config;

            Logger.Information("Init the device C13336Mppc");
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        private void DeliverMessages(string msgName, object value)
        {
            string[] fullMessages = new string[2];
            EFeedbackMsgC13336 index = 0;
            EFeedbackMsgC13336.TryParse(msgName, out index);

            MppcCollector collector = MppcCollector.Unknown;

            if (index != EFeedbackMsgC13336.State)
            {
                fullMessages = ((string)value).Split(';');
                collector = (fullMessages.First() == MppcCollector.WIDE.ToString()) ? MppcCollector.WIDE : MppcCollector.NARROW;
            }

            switch (index)
            {
                case EFeedbackMsgC13336.State:
                    State = new DeviceState((DeviceStatus)(int)value);
                    break;

                case EFeedbackMsgC13336.MonitorInfoStatusMsg:
                    string sSMsg = fullMessages.Last();
                    if (!String.IsNullOrWhiteSpace(sSMsg))
                    {
                        Messenger.Send(new MonitorInfoStatusMessage() { Collector = collector, MonitorInfoStatus = fullMessages.Last() });
                    }
                    break;

                case EFeedbackMsgC13336.OutputCurrentMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double outputCurrent = Double.TryParse(fullMessages.Last(), out var tempOutputCurrent) ? tempOutputCurrent : 0;
                        Messenger.Send(new OutputCurrentMessage() { Collector = collector, OutputCurrent = outputCurrent });
                    }
                    break;

                case EFeedbackMsgC13336.OutputVoltageMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double outputVoltage = Double.TryParse(fullMessages.Last(), out var tempOutputVoltage) ? tempOutputVoltage : 0;
                        Messenger.Send(new OutputVoltageMessage() { Collector = collector, OutputVoltage = outputVoltage });
                    }
                    break;

                case EFeedbackMsgC13336.OutputVoltageSettingMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double outputVoltageSetting = Double.TryParse(fullMessages.Last(), out var tempOutputVoltageSetting) ? tempOutputVoltageSetting : 0;
                        Messenger.Send(new OutputVoltageSettingMessage() { Collector = collector, OutputVoltageSetting = outputVoltageSetting });
                    }
                    break;

                case EFeedbackMsgC13336.HighVoltageStatusMsg:
                    sSMsg = (string)value;
                    if (!String.IsNullOrWhiteSpace(sSMsg))
                    {
                        Messenger.Send(new HighVoltageStatusMessage() { Collector = collector, HighVoltageStatus = sSMsg });
                    }
                    break;

                case EFeedbackMsgC13336.StateSignalsMsg:
                    sSMsg = (string)fullMessages.Last();
                    if (!String.IsNullOrWhiteSpace(sSMsg))
                    {
                        MppcStateModule stateSignals = (MppcStateModule)Enum.Parse(typeof(MppcStateModule), sSMsg);
                        Messenger.Send(new StateSignalsMessage() { Collector = collector, StateSignals = stateSignals });
                    }
                    break;

                case EFeedbackMsgC13336.TemperatureMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double temperature = Double.TryParse(fullMessages.Last(), out var tempTemperature) ? tempTemperature : 0;
                        Messenger.Send(new TemperatureMessage() { Collector = collector, Temperature = temperature });
                    }
                    break;

                case EFeedbackMsgC13336.SensorTemperatureMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double sensorTemperature = Double.TryParse(fullMessages.Last(), out var tempSensorTemperature) ? tempSensorTemperature : 0;
                        Messenger.Send(new SensorTemperatureMessage() { Collector = collector, SensorTemperature = sensorTemperature });
                    }
                    break;

                case EFeedbackMsgC13336.PowerFctReadMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double powerFctRead = Double.TryParse(fullMessages.Last(), out var tempPowerFctRead) ? tempPowerFctRead : 0;
                        Messenger.Send(new PowerFctReadMessage() { Collector = collector, PowerFctRead = powerFctRead });
                    }
                    break;

                case EFeedbackMsgC13336.TempCorrectionFactorReadMsg:
                    if (!String.IsNullOrWhiteSpace(fullMessages.Last()))
                    {
                        double tempCorrectionFactorRead = Double.TryParse(fullMessages.Last(), out var tempTempCorrectionFactorRead) ? tempTempCorrectionFactorRead : 0;
                        Messenger.Send(new TempCorrectionFactorReadMessage() { Collector = collector, TempCorrectionFactorRead = tempCorrectionFactorRead });
                    }
                    break;

                case EFeedbackMsgC13336.FirmwareMsg:
                    sSMsg = (string)fullMessages.Last();
                    if (!String.IsNullOrWhiteSpace(sSMsg))
                    {
                        Messenger.Send(new FirmwareMessage() { Collector = collector, Firmware = fullMessages.Last() });
                    }
                    break;

                case EFeedbackMsgC13336.IdentifierMsg:
                    sSMsg = (string)fullMessages.Last();
                    if (!String.IsNullOrWhiteSpace(sSMsg))
                    {
                        Messenger.Send(new IdentifierMessage() { Collector = collector, Identifier = fullMessages.Last() });
                    }
                    break;

                default:
                    Logger.Warning("C13336Mppc - Unknown message  : " + msgName);
                    break;
            }
        }

        public override void TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }

        public override void ManageRelays(bool relayActivated)
        {
            throw new NotImplementedException();
        }

        public override void SetOutputVoltage(double voltage)
        {
            throw new NotImplementedException();
        }

        public override void TempCorrectionFactorSetting()
        {
            throw new NotImplementedException();
        }

        public override void SwitchTempCompensationMode()
        {
            throw new NotImplementedException();
        }

        public override void RefVoltageTempSetting()
        {
            throw new NotImplementedException();
        }

        public override void PowerFctSetting()
        {
            throw new NotImplementedException();
        }

        public override void OutputVoltageOn()
        {
            throw new NotImplementedException();
        }

        public override void OutputVoltageOff()
        {
            throw new NotImplementedException();
        }

        public override void PowerReset()
        {
            throw new NotImplementedException();
        }

        public override void CustomCommand(string custom)
        {
            throw new NotImplementedException();
        }
    }
}
