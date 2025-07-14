using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Dispatcher;
using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Client.Proxy.Chiller
{
    public class ChillerViewModel : ObservableRecipient
    {
        private readonly IChillerSupervisor _chillerSupervisor;
        private readonly IDispatcher _dispatcher;

        public ChillerViewModel(IChillerSupervisor chillerSupervisor, IDispatcher dispatcher, IMessenger messenger)
        {
            _chillerSupervisor = chillerSupervisor;
            _dispatcher = dispatcher;
            messenger.Register<FanSpeedChangedMessage>(this, (r, m) => { FanSpeedPercent = m.FanSpeed; });
            messenger.Register<CompressionChangedMessage>(this, (r, m) => { Compression = m.Compression; });
            messenger.Register<TemperatureChangedMessage>(this, (r, m) => { Temperature = m.Temperature; });
            messenger.Register<FanSpeedModeChangedMessage>(this, (r, m) => { ConstFanSpeedMode = m.ConstFanSpeedMode; });
            messenger.Register<LeakDetectionChangedMessage>(this, (r, m) => { Leak = m.Leak; });
            messenger.Register<AlarmChangedMessage>(this, (r, m) => { Alarms = m.Alarm; });
            messenger.Register<ChillerModeChangedMessage>(this, (r, m) => { Mode = m.Mode; });
        }

        private double _fanSpeedPercentSliderValue;
        public double FanSpeedPercentSliderValue
        {
            get => _fanSpeedPercentSliderValue;
            set
            {
                if (_fanSpeedPercentSliderValue == value)
                {
                    return;
                }
                _fanSpeedPercentSliderValue = value;
                OnPropertyChanged();
            }
        }

        private double _compressionPercentSliderValue;
        public double CompressionPercentSliderValue
        {
            get => _compressionPercentSliderValue;
            set
            {
                if (_compressionPercentSliderValue == value)
                {
                    return;
                }
                _compressionPercentSliderValue = value;
                OnPropertyChanged();
            }
        }

        private double _temperatureSliderValue;
        public double TemperatureSliderValue
        {
            get => _temperatureSliderValue;
            set
            {
                if (_temperatureSliderValue == value)
                {
                    return;
                }
                _temperatureSliderValue = value;
                OnPropertyChanged();
            }
        }

        private double _fanSpeedPercent;
        public double FanSpeedPercent
        {
            get => _fanSpeedPercent;
            set
            {
                _fanSpeedPercent = value;
                OnPropertyChanged();
            }
        }

        private double _compression;
        public double Compression
        {
            get => _compression;
            set
            {
                _compression = value;
                OnPropertyChanged();
            }
        }
        
        private double _temperature;
        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        private ConstFanSpeedMode _constFanSpeedMode;
        public ConstFanSpeedMode ConstFanSpeedMode 
        { 
            get => _constFanSpeedMode;
            set
            {
                _constFanSpeedMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConstantFanSpeedSelected));
            }
        }

        private ChillerMode _mode;

        public ChillerMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StandaloneModeSelected));
            }
        }

        public LeakDetection Leak { get; set; }
        public AlarmDetection Alarms { get; set; }
        
        public bool ConstantFanSpeedSelected
        {
            get => ConstFanSpeedMode == ConstFanSpeedMode.Enabled;
        }

        public bool StandaloneModeSelected
        {
            get => Mode == ChillerMode.Standalone;
        }
        
        private RelayCommand _enableFanSpeedModeCommand;

        public RelayCommand EnableFanSpeedModeCommand
        {
            get => _enableFanSpeedModeCommand ?? (_enableFanSpeedModeCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() =>
                {
                    _chillerSupervisor.SetConstFanSpeedMode(ConstFanSpeedMode.Enabled);
                });
            }));
        }
        
        private RelayCommand _disableFanSpeedModeCommand;

        public RelayCommand DisableFanSpeedModeCommand
        {
            get => _disableFanSpeedModeCommand ?? (_disableFanSpeedModeCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() =>
                {
                    _chillerSupervisor.SetConstFanSpeedMode(ConstFanSpeedMode.Disabled);
                });
            }));
        }

        private RelayCommand _changeSpeedCommand;
        public RelayCommand ChangeSpeed
        {
            get => _changeSpeedCommand ?? (_changeSpeedCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.SetFanSpeed(FanSpeedPercentSliderValue); });
            }));
        }
        
        private RelayCommand _changeCompressionCommand;
        public RelayCommand ChangeCompression
        {
            get => _changeCompressionCommand ?? (_changeCompressionCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.SetMaxCompressionSpeed(CompressionPercentSliderValue); });
            }));
        }    
        
        private RelayCommand _changeTemperatureCommand;
        public RelayCommand ChangeTemperature
        {
            get => _changeTemperatureCommand ?? (_changeTemperatureCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.SetTemperature(TemperatureSliderValue); });
            }));
        }

        private RelayCommand _resetCommand;
        public RelayCommand ResetCommand
        {
            get => _resetCommand ?? (_resetCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.Reset(); });
            }));
        }

        private RelayCommand _setStandAloneModeCommand;
        public RelayCommand SetStandAloneModeCommand { 
            get => _setStandAloneModeCommand ?? (_setStandAloneModeCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.SetChillerMode(ChillerMode.Standalone); });
            }));
        }
        
        private RelayCommand _setRemoteModeCommand;
        public RelayCommand SetRemoteModeCommand { 
            get => _setRemoteModeCommand ?? (_setRemoteModeCommand = new RelayCommand(() =>
            {
                _dispatcher.Invoke(() => { _chillerSupervisor.SetChillerMode(ChillerMode.Remote); });
            }));
        }
    }
}
