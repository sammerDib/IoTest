using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Mppc
{
    public class MppcVM : ObservableObject
    {
        private MppcSupervisor _supervisor;
        public MppcCollector Collector { get; set; }

        public double VoltageSetpoint { get; set; }

        public string CustomTxt { get; set; }

        public MppcVM(MppcSupervisor supervisor, string collecor)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.Collector, m.State); });
            Messenger.Register<MonitorInfoStatusChangedMessage>(this, (r, m) => { UpdateMonitorInfoStatus(m.Collector, m.MonitorInfoStatus); });
            Messenger.Register<OutputCurrentChangedMessage>(this, (r, m) => { UpdateOutputCurrent(m.Collector, m.OutputCurrent); });
            Messenger.Register<OutputVoltageChangedMessage>(this, (r, m) => { UpdateOutputVoltage(m.Collector, m.OutputVoltage); });
            Messenger.Register<OutputVoltageSettingChangedMessage>(this, (r, m) => { UpdateOutputVoltageSetting(m.Collector, m.OutputVoltageSetting); });
            Messenger.Register<HighVoltageStatusChangedMessage>(this, (r, m) => { UpdateHighVoltageStatus(m.Collector, m.HighVoltageStatus); });
            Messenger.Register<StateSignalsChangedMessage>(this, (r, m) => { UpdateStateSignals(m.Collector, m.StateSignals); });
            Messenger.Register<TemperatureChangedMessage>(this, (r, m) => { UpdateTemperature(m.Collector, m.Temperature); });
            Messenger.Register<SensorTemperatureChangedMessage>(this, (r, m) => { UpdateSensorTemperature(m.Collector, m.SensorTemperature); });
            Messenger.Register<PowerFctReadChangedMessage>(this, (r, m) => { UpdatePowerFctRead(m.Collector, m.PowerFctRead); });
            Messenger.Register<TempCorrectionFactorReadChangedMessage>(this, (r, m) => { UpdateTempCorrectionFactorRead(m.Collector, m.TempCorrectionFactorRead); });
            Messenger.Register<FirmwareChangedMessage>(this, (r, m) => { UpdateFirmware(m.Collector, m.Firmware); });
            Messenger.Register<IdentifierChangedMessage>(this, (r, m) => { UpdateIdentifier(m.Collector, m.Identifier); });

            Collector = (collecor == MppcCollector.WIDE.ToString()) ? MppcCollector.WIDE : MppcCollector.NARROW;

            Task.Run(() => _supervisor.TriggerUpdateEvent(Collector));
        }

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        private void UpdateState(MppcCollector collector, string value)
        {
            if (collector == Collector)
            {
                State = value;
            }
        }

        public void UpdateMonitorInfoStatus(MppcCollector collector, string value)
        {
            if (collector == Collector)
            {
                GetMonitorInfoStatus = value;
            }
        }

        public void UpdateOutputCurrent(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetOutputCurrent = value;
            }
        }

        public void UpdateOutputVoltage(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetOutputVoltage = value;
            }
        }

        public void UpdateOutputVoltageSetting(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetOutputVoltageSetting = value;
            }
        }        

        public void UpdateHighVoltageStatus(MppcCollector collector, string value)
        {
            if (collector == Collector)
            {
                GetHighVoltageStatus = value;
            }
        }

        public void UpdateStateSignals(MppcCollector collector, MppcStateModule value)
        {
            if (collector == Collector)
            {
                GetStateSignals = value;
            }
        }

        public void UpdateTemperature(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetTemperature = value;
            }
        }

        public void UpdateSensorTemperature(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetSensorTemperature = value;
            }
        }        

        public void UpdatePowerFctRead(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetPowerFctRead = value;
            }
        }

        public void UpdateTempCorrectionFactorRead(MppcCollector collector, double value)
        {
            if (collector == Collector)
            {
                GetTempCorrectionFactorRead = value;
            }
        }

        public void UpdateFirmware(MppcCollector collector, string value)
        {
            if (collector == Collector)
            {
                GetFirmware = value;
            }
        }

        private void UpdateIdentifier(MppcCollector collector, string value)
        {
            if (collector == Collector)
            {
                GetId = value;
            }
        }

        private string _state;

        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _monitorInfoStatus;

        public string GetMonitorInfoStatus
        {
            get => _monitorInfoStatus;
            set
            {
                if (_monitorInfoStatus != value)
                {
                    _monitorInfoStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _outputCurrent;

        public double GetOutputCurrent
        {
            get => _outputCurrent;
            set
            {
                if (_outputCurrent != value)
                {
                    _outputCurrent = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _outputVoltage;

        public double GetOutputVoltage
        {
            get => _outputVoltage;
            set
            {
                if (_outputVoltage != value)
                {
                    _outputVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _outputVoltageSetting;

        public double GetOutputVoltageSetting
        {
            get => _outputVoltageSetting;
            set
            {
                if (_outputVoltageSetting != value)
                {
                    _outputVoltageSetting = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _highVoltagestatus;

        public string GetHighVoltageStatus
        {
            get => _highVoltagestatus;
            set
            {
                if (_highVoltagestatus != value)
                {
                    _highVoltagestatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private MppcStateModule _stateSignals;

        public MppcStateModule GetStateSignals
        {
            get => _stateSignals;
            set
            {
                if (_stateSignals != value)
                {
                    _stateSignals = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _temperature;

        public double GetTemperature
        {
            get => _temperature;
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _sensorTemperature;

        public double GetSensorTemperature
        {
            get => _sensorTemperature;
            set
            {
                if (_sensorTemperature != value)
                {
                    _sensorTemperature = value;
                    OnPropertyChanged();
                }
            }
        }        

        private double _powerFctRead;

        public double GetPowerFctRead
        {
            get => _powerFctRead;
            set
            {
                if (_powerFctRead != value)
                {
                    _powerFctRead = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _tempCorrectionFactorRead;

        public double GetTempCorrectionFactorRead
        {
            get => _tempCorrectionFactorRead;
            set
            {
                if (_tempCorrectionFactorRead != value)
                {
                    _tempCorrectionFactorRead = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _firmware;

        public string GetFirmware
        {
            get => _firmware;
            set
            {
                if (_firmware != value)
                {
                    _firmware = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getId;

        public string GetId
        {
            get => _getId;
            set
            {
                if (_getId != value)
                {
                    _getId = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand _tempCorrectionFactorSetting;

        public AutoRelayCommand TempCorrectionFactorSetting
        {
            get
            {
                return _tempCorrectionFactorSetting ?? (_tempCorrectionFactorSetting = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.TempCorrectionFactorSetting(Collector));
                    }));
            }
        }

        private AutoRelayCommand _switchTempCompensationMode;

        public AutoRelayCommand SwitchTempCompensationMode
        {
            get
            {
                return _switchTempCompensationMode ?? (_switchTempCompensationMode = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SwitchTempCompensationMode(Collector));
                    }));
            }
        }

        private AutoRelayCommand _setOutputVoltage;

        public AutoRelayCommand SetOutputVoltage
        {
            get
            {
                return _setOutputVoltage ?? (_setOutputVoltage = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetOutputVoltage(Collector, VoltageSetpoint));
                    }));
            }
        }

        private AutoRelayCommand _refVoltageTempSetting;

        public AutoRelayCommand RefVoltageTempSetting
        {
            get
            {
                return _refVoltageTempSetting ?? (_refVoltageTempSetting = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.RefVoltageTempSetting(Collector));
                    }));
            }
        }

        private AutoRelayCommand _powerFctSetting;

        public AutoRelayCommand PowerFctSetting
        {
            get
            {
                return _powerFctSetting ?? (_powerFctSetting = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerFctSetting(Collector));
                    }));
            }
        }

        private AutoRelayCommand _outputVoltageOn;

        public AutoRelayCommand OutputVoltageOn
        {
            get
            {
                return _outputVoltageOn ?? (_outputVoltageOn = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.OutputVoltageOn(Collector));
                    }));
            }
        }

        private AutoRelayCommand _outputVoltageOff;

        public AutoRelayCommand OutputVoltageOff
        {
            get
            {
                return _outputVoltageOff ?? (_outputVoltageOff = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.OutputVoltageOff(Collector));
                    }));
            }
        }

        private AutoRelayCommand _powerReset;

        public AutoRelayCommand PowerReset
        {
            get
            {
                return _powerReset ?? (_powerReset = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerReset(Collector));
                    }));
            }
        }
    }
}
