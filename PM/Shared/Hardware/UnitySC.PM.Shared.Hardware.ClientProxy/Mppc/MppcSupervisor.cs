using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.Service.Interface.Mppc;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Mppc
{
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MppcSupervisor : IMppcServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IMppcService> _mppcService;
        private IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public MppcSupervisor(ILogger<MppcSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _mppcService = new DuplexServiceInvoker<IMppcService>(_instanceContext, "MppcService", ClassLocator.Default.GetInstance<SerilogLogger<IMppcService>>(), messenger, s => s.SubscribeToChanges());
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        public Response<VoidResult> Connect(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.Connect(collector));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent(collector));
        }

        public Response<VoidResult> SetOutputVoltage(MppcCollector collector, double voltage)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.SetOutputVoltage(collector, voltage));
        }

        public Response<VoidResult> TempCorrectionFactorSetting(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.TempCorrectionFactorSetting(collector));
        }

        public Response<VoidResult> SwitchTempCompensationMode(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.SwitchTempCompensationMode(collector));
        }

        public Response<VoidResult> RefVoltageTempSetting(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.RefVoltageTempSetting(collector));
        }

        public Response<VoidResult> PowerFctSetting(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.PowerFctSetting(collector));
        }

        public Response<VoidResult> OutputVoltageOn(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.OutputVoltageOn(collector));
        }

        public Response<VoidResult> OutputVoltageOff(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.OutputVoltageOff(collector));
        }

        public Response<VoidResult> PowerReset(MppcCollector collector)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.PowerReset(collector));
        }

        public Response<VoidResult> CustomCommand(MppcCollector collector, string customCmd)
        {
            return _mppcService.TryInvokeAndGetMessages(s => s.CustomCommand(collector, customCmd));
        }

        void IMppcServiceCallback.StateChangedCallback(MppcCollector collector, string state)
        {
            throw new NotImplementedException();
        }

        void IMppcServiceCallback.MonitorInfoStatusChangedCallback(MppcCollector collector, string value)
        {
            _messenger.Send(new MonitorInfoStatusChangedMessage() { Collector = collector, MonitorInfoStatus = value });
        }

        void IMppcServiceCallback.OutputCurrentChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new OutputCurrentChangedMessage() { Collector = collector, OutputCurrent = value });
        }

        void IMppcServiceCallback.OutputVoltageChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new OutputVoltageChangedMessage() { Collector = collector, OutputVoltage = value });
        }

        void IMppcServiceCallback.OutputVoltageSettingChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new OutputVoltageSettingChangedMessage() { Collector = collector, OutputVoltageSetting = value });
        }

        void IMppcServiceCallback.HighVoltageStatusChangedCallback(MppcCollector collector, string value)
        {
            _messenger.Send(new HighVoltageStatusChangedMessage() { Collector = collector, HighVoltageStatus = value });
        }

        void IMppcServiceCallback.StateSignalsChangedCallback(MppcCollector collector, MppcStateModule value)
        {
            _messenger.Send(new StateSignalsChangedMessage() { Collector = collector, StateSignals = value });
        }

        void IMppcServiceCallback.TemperatureChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new TemperatureChangedMessage() { Collector = collector, Temperature = value });
        }

        void IMppcServiceCallback.SensorTemperatureChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new SensorTemperatureChangedMessage() { Collector = collector, SensorTemperature = value });
        }

        void IMppcServiceCallback.PowerFctReadChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new PowerFctReadChangedMessage() { Collector = collector, PowerFctRead = value });
        }

        void IMppcServiceCallback.TempCorrectionFactorReadChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new TempCorrectionFactorReadChangedMessage() { Collector = collector, TempCorrectionFactorRead = value });
        }

        void IMppcServiceCallback.FirmwareChangedCallback(MppcCollector collector, string value)
        {
            _messenger.Send(new FirmwareChangedMessage() { Collector = collector, Firmware = value });
        }

        void IMppcServiceCallback.IdentifierChangedCallback(MppcCollector collector, string value)
        {
            _messenger.Send(new IdentifierChangedMessage() { Collector = collector, Identifier = value });
        }
    }
}
