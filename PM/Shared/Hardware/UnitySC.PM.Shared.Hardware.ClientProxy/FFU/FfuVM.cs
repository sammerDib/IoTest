using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Ffu
{
    public class FfuVM : ObservableRecipient
    {
        private readonly ILogger _logger;
        private FfuSupervisor _supervisor;
        private Dictionary<string, ushort> _ffuConfigValues;

        public string CustomTxt { get; set; }

        public FfuVM(FfuSupervisor supervisor)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _supervisor = supervisor;
            _ffuConfigValues = _supervisor?.GetDefaultFfuValues().Result;            

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<StatusChangedMessage>(this, (r, m) => { UpdateStatus(m.Status); });
            Messenger.Register<CurrentSpeedChangedMessage>(this, (r, m) => { UpdateCurrentSpeed(m.CurrentSpeed); });
            Messenger.Register<TemperatureChangedMessage>(this, (r, m) => { UpdateTemperature(m.Temperature); });
            Messenger.Register<WarningChangedMessage>(this, (r, m) => { UpdateWarning(m.Warning); });
            Messenger.Register<AlarmChangedMessage>(this, (r, m) => { UpdateAlarm(m.Alarm); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Custom); });

            Task.Run(() => _supervisor.TriggerUpdateEvent());            

            Init();
        }

        private void Init()
        {
            var defaultSpeed = GetDefaultFFuValue("NormalRunningSpeed");
            if (FanSpeedPercentSliderValue != defaultSpeed)
            {
                FanSpeedPercentSliderValue = defaultSpeed; 
            }
        }

        private void UpdateState(DeviceState value)
        {
            State = value.Status.ToString();
        }

        private void UpdateStatus(string value)
        {
            Status = value;
        }

        private void UpdateCurrentSpeed(ushort value)
        {
            SpeedPercent = value;
        }

        private void UpdateTemperature(double value)
        {
            Temperature = value;
        }

        private void UpdateWarning(bool value)
        {
            Warning = value; 
        }

        private void UpdateAlarm(bool value)
        {
            Alarm = value; 
        }

        private void UpdateCustom(string value)
        {
            GetCustom = value;
        }

        private ushort GetDefaultFFuValue(string value)
        {
            return _ffuConfigValues.TryGetValue(value, out ushort result) ? result : (ushort)0;
        }

        private ushort _fanSpeedPercentSliderValue;
        public ushort FanSpeedPercentSliderValue
        {
            get => _fanSpeedPercentSliderValue;
            set
            {
                _supervisor.SetSpeed(value);
                SetProperty(ref _fanSpeedPercentSliderValue, value);
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

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _temperature;
        public double Temperature
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

        private bool _warning;
        public bool Warning
        {
            get => _warning;
            set
            {
                if (_warning != value)
                {
                    _warning = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _alarm;
        public bool Alarm
        {
            get => _alarm;
            set
            {
                if (_alarm != value)
                {
                    _alarm = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getCustom;
        public string GetCustom
        {
            get => _getCustom;
            set
            {
                if (_getCustom != value)
                {
                    _getCustom = value;

                    OnPropertyChanged();
                }
            }
        }

        private ushort _speedPercent;
        public ushort SpeedPercent
        {
            get => _speedPercent;
            set => SetProperty(ref _speedPercent, value);
        }

        private RelayCommand _changeSpeedCommand;
        public RelayCommand ChangeSpeed
        {
            get
            {
                return _changeSpeedCommand ?? (_changeSpeedCommand = new RelayCommand(
                    () =>
                    {                        
                        Application.Current.Dispatcher.Invoke(() => { _supervisor.SetSpeed(FanSpeedPercentSliderValue); });
                    }));
            }
        }

        private RelayCommand _customCommand;
        public RelayCommand CustomCommand
        {
            get
            {
                return _customCommand ?? (_customCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CustomCommand(CustomTxt));
                    }));
            }
        }        
    }
}
