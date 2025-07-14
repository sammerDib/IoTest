using System.ServiceModel;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.Mppc;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Mppc;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MppcService : DuplexServiceBase<IMppcServiceCallback>, IMppcService
    {
        private HardwareManager _hardwareManager;

        public MppcService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<StateMessage>(this, (r, m) => { UpdateState(m.Collector, m.State); });
            messenger.Register<MonitorInfoStatusMessage>(this, (r, m) => { UpdateMonitorInfoStatus(m.Collector, m.MonitorInfoStatus); });
            messenger.Register<OutputCurrentMessage>(this, (r, m) => { UpdateOutputCurrent(m.Collector, m.OutputCurrent); });
            messenger.Register<OutputVoltageMessage>(this, (r, m) => { UpdateOutputVoltage(m.Collector, m.OutputVoltage); });
            messenger.Register<HighVoltageStatusMessage>(this, (r, m) => { UpdateHighVoltageStatus(m.Collector, m.HighVoltageStatus); });
            messenger.Register<StateSignalsMessage>(this, (r, m) => { UpdateStateSignals(m.Collector, m.StateSignals); });
            messenger.Register<TemperatureMessage>(this, (r, m) => { UpdateTemperature(m.Collector, m.Temperature); });
            messenger.Register<PowerFctReadMessage>(this, (r, m) => { UpdatePowerFctRead(m.Collector, m.PowerFctRead); });
            messenger.Register<TempCorrectionFactorReadMessage>(this, (r, m) => { UpdateTempCorrectionFactorRead(m.Collector, m.TempCorrectionFactorRead); });
            messenger.Register<FirmwareMessage>(this, (r, m) => { UpdateFirmware(m.Collector, m.Firmware); });
            messenger.Register<IdentifierMessage>(this, (r, m) => { UpdateIdentifier(m.Collector, m.Identifier); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to Mppc change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to Mppc change"));
            });
        }

        public Response<VoidResult> Connect(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Mppcs[collector.ToString()] != null)
                {
                    _logger.Information("Mppc communication Connect");
                    _hardwareManager.Mppcs[collector.ToString()].Connect();
                    messageContainer.Add(new Message(MessageLevel.Information, "Hardware status " + _hardwareManager.Mppcs[collector.ToString()].State.Status.ToString()));
                }
            });
        }

        public Response<VoidResult> TriggerUpdateEvent(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Mppc initialize update");
                _hardwareManager.Mppcs[collector.ToString()].TriggerUpdateEvent();
                messageContainer.Add(new Message(MessageLevel.Information, "Mppc initialize update"));
            });
        }

        public Response<VoidResult> SetOutputVoltage(MppcCollector collector, double voltage)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("SetOutputVoltage");
                _hardwareManager.Mppcs[collector.ToString()].SetOutputVoltage(voltage);
                messageContainer.Add(new Message(MessageLevel.Information, "Set output voltage"));
            });
        }

        public Response<VoidResult> TempCorrectionFactorSetting(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("TempCorrectionFactorSetting");
                _hardwareManager.Mppcs[collector.ToString()].TempCorrectionFactorSetting();
                messageContainer.Add(new Message(MessageLevel.Information, "TempCorrectionFactorSetting"));
            });
        }

        public Response<VoidResult> SwitchTempCompensationMode(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("SwitchTempCompensationMode");
                _hardwareManager.Mppcs[collector.ToString()].SwitchTempCompensationMode();
                messageContainer.Add(new Message(MessageLevel.Information, "SwitchTempCompensationMode"));
            });
        }

        public Response<VoidResult> RefVoltageTempSetting(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("RefVoltageTempSetting");
                _hardwareManager.Mppcs[collector.ToString()].RefVoltageTempSetting();
                messageContainer.Add(new Message(MessageLevel.Information, "RefVoltageTempSetting"));
            });
        }

        public Response<VoidResult> PowerFctSetting(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("PowerFctSetting");
                _hardwareManager.Mppcs[collector.ToString()].PowerFctSetting();
                messageContainer.Add(new Message(MessageLevel.Information, "PowerFctSetting"));
            });
        }

        public Response<VoidResult> OutputVoltageOn(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("OutputVoltageOn");
                _hardwareManager.Mppcs[collector.ToString()].OutputVoltageOn();
                messageContainer.Add(new Message(MessageLevel.Information, "OutputVoltageOn"));
            });
        }

        public Response<VoidResult> OutputVoltageOff(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("OutputVoltageOff");
                _hardwareManager.Mppcs[collector.ToString()].OutputVoltageOff();
                messageContainer.Add(new Message(MessageLevel.Information, "OutputVoltageOff"));
            });
        }

        public Response<VoidResult> PowerReset(MppcCollector collector)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("PowerReset");
                _hardwareManager.Mppcs[collector.ToString()].PowerReset();
                messageContainer.Add(new Message(MessageLevel.Information, "PowerReset"));
            });
        }

        public Response<VoidResult> CustomCommand(MppcCollector collector, string custom)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("CustomCommand " + custom);
                _hardwareManager.Mppcs[collector.ToString()].CustomCommand(custom);
                messageContainer.Add(new Message(MessageLevel.Information, "CustomCommand " + custom));
            });
        }

        public Response<VoidResult> ManageRelays(MppcCollector collector, bool relayActivated)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("ManageRelays - activated : " + relayActivated);
                _hardwareManager.Mppcs[collector.ToString()].ManageRelays(relayActivated);
                messageContainer.Add(new Message(MessageLevel.Information, "ManageRelays - activated : " + relayActivated));
            });
        }

        public void UpdateState(MppcCollector collector, string state)
        {
            InvokeCallback(i => i.StateChangedCallback(collector, state));
        }

        public void UpdateMonitorInfoStatus(MppcCollector collector, string value)
        {
            InvokeCallback(i => i.MonitorInfoStatusChangedCallback(collector, value));
        }

        public void UpdateOutputCurrent(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.OutputCurrentChangedCallback(collector, value));
        }

        public void UpdateOutputVoltage(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.OutputVoltageChangedCallback(collector, value));
        }

        public void UpdateOutputVoltageSetting(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.OutputVoltageSettingChangedCallback(collector, value));
        }

        public void UpdateHighVoltageStatus(MppcCollector collector, string value)
        {
            InvokeCallback(i => i.HighVoltageStatusChangedCallback(collector, value));
        }

        public void UpdateStateSignals(MppcCollector collector, MppcStateModule value)
        {
            InvokeCallback(i => i.StateSignalsChangedCallback(collector, value));
        }

        public void UpdateTemperature(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.TemperatureChangedCallback(collector, value));
        }

        public void UpdateSensorTemperature(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.SensorTemperatureChangedCallback(collector, value));
        }

        public void UpdatePowerFctRead(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.PowerFctReadChangedCallback(collector, value));
        }

        public void UpdateTempCorrectionFactorRead(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.TempCorrectionFactorReadChangedCallback(collector, value));
        }

        public void UpdateFirmware(MppcCollector collector, string value)
        {
            InvokeCallback(i => i.FirmwareChangedCallback(collector, value));
        }

        public void UpdateIdentifier(MppcCollector collector, string value)
        {
            InvokeCallback(i => i.IdentifierChangedCallback(collector, value));
        }
    }
}
