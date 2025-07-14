using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Laser
{
    public class LaserVM : ObservableObject
    {
        private LaserSupervisor _supervisor;

        public int PowerSetpoint { get; set; }
        public string CustomTxt { get; set; }
        public string Status { get; set; }

        public LaserVM(LaserSupervisor supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<PowerChangedMessage>(this, (r, m) => { UpdatePower(m.Power); });
            Messenger.Register<InterlockStatusChangedMessage>(this, (r, m) => { UpdateInterlockStatus(m.InterlockStatus); });
            Messenger.Register<LaserTemperatureChangedMessage>(this, (r, m) => { UpdateLaserTemperature(m.LaserTemperature); });
            Messenger.Register<LaserPowerStatusChangedMessage>(this, (r, m) => { UpdateLaserPowerStatus(m.LaserPowerStatus); });
            Messenger.Register<CrystalTemperatureChangedMessage>(this, (r, m) => { UpdateCrystalTemperature(m.CrystalTemperature); });
            Messenger.Register<IdChangedMessage>(this, (r, m) => { UpdateId(m.Id); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Custom); });

            Task.Run(() => _supervisor.TriggerUpdateEvent());
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

        private void UpdateState(DeviceState value)
        {
            State = value.Status.ToString();
        }

        private void UpdatePower(double value)
        {
            Power = value;
        }

        private void UpdateInterlockStatus(string interlockStatus)
        {
            InterlockStatus = interlockStatus;
        }

        private void UpdateLaserTemperature(double laserTemperature)
        {
            LaserTemperature = laserTemperature;
        }

        private void UpdatePsuTemperature(double psuTemperature)
        {
            PsuTemperature = psuTemperature;
        }

        private void UpdateLaserPowerStatus(bool value)
        {
            LaserPowerStatus = value;
        }

        private void UpdateCrystalTemperature(double crystalTemperature)
        {
            CrystalTemperature = crystalTemperature;
        }

        private void UpdateId(string value)
        {
            GetId = value;
        }

        private void UpdateCustom(string value)
        {
            GetCustom = value;
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

        private double _power;

        public double Power
        {
            get => _power;
            set
            {
                if (_power != value)
                {
                    _power = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _interlockStatus;

        public string InterlockStatus
        {
            get => _interlockStatus;
            set
            {
                if (_interlockStatus != value)
                {
                    _interlockStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _laserTemperature;

        public double LaserTemperature
        {
            get => _laserTemperature;
            set
            {
                if (_laserTemperature != value)
                {
                    _laserTemperature = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _psuTemperature;

        public double PsuTemperature
        {
            get => _psuTemperature;
            set
            {
                if (_psuTemperature != value)
                {
                    _psuTemperature = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _laserPowerStatus;

        public bool LaserPowerStatus
        {
            get => _laserPowerStatus;
            set
            {
                if (_laserPowerStatus != value)
                {
                    _laserPowerStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _crystalTemperature;

        public double CrystalTemperature
        {
            get => _crystalTemperature;
            set
            {
                if (_crystalTemperature != value)
                {
                    _crystalTemperature = value;
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

        private AutoRelayCommand _applyPowerCommand;

        public AutoRelayCommand ApplyPowerCommand
        {
            get
            {
                return _applyPowerCommand ?? (_applyPowerCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetPower(PowerSetpoint));
                    }));
            }
        }

        private AutoRelayCommand _powerOnCommand;

        public AutoRelayCommand PowerOnCommand
        {
            get
            {
                return _powerOnCommand ?? (_powerOnCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOn());
                    }));
            }
        }

        private AutoRelayCommand _powerOffCommand;

        public AutoRelayCommand PowerOffCommand
        {
            get
            {
                return _powerOffCommand ?? (_powerOffCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOff());
                    }));
            }
        }

        private AutoRelayCommand _customCommand;

        public AutoRelayCommand CustomCommand
        {
            get
            {
                return _customCommand ?? (_customCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CustomCommand(CustomTxt));
                    }));
            }
        }
    }
}
