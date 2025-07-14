using System.Globalization;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance
{
    public class LiseHFVM : ViewModelBase
    {
        private ILiseHFService _supervisor;

        public int PowerSetpoint { get; set; }
        public ServoPosition AttenuationAbsPositionSetpoint { get; set; }

        public double FastAttenuationAbsPositionSetpoint { get; set; }

        private CultureInfo _culture = CultureInfo.InvariantCulture;

        public LiseHFVM(ILiseHFService supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<Proxy.LiseHF.LaserPowerStatusChangedMessage>(this, (m) => { UpdateLaserPowerStatus(m.LaserPowerOn); });
            Messenger.Register<Proxy.LiseHF.InterlockStatusChangedMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            Messenger.Register<Proxy.LiseHF.LaserTemperatureChangedMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            Messenger.Register<Proxy.LiseHF.CrystalTemperatureChangedMessage>(this, (m) => { UpdateCrystalTemperature(m.CrystalTemperature); });
            Messenger.Register<Proxy.LiseHF.AttenuationPositionChangedMessage>(this, (m) => { UpdateAttenuationPosition(m.AttenuationPosition); });
            Messenger.Register<Proxy.LiseHF.FastAttenuationPositionChangedMessage>(this, (m) => { UpdateFastAttenuationPosition(m.FastAttenuationPosition); });
            Messenger.Register<Proxy.LiseHF.ShutterIrisPositionChangedMessages>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });

            Task.Run(() => _supervisor.InitializeUpdate());

            CultureInfo.CurrentCulture = _culture;
        }

        private GalaSoft.MvvmLight.Messaging.IMessenger _messenger;

        public GalaSoft.MvvmLight.Messaging.IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<GalaSoft.MvvmLight.Messaging.IMessenger>();
                return _messenger;
            }
        }

        private void UpdateLaserPowerStatus(bool value)
        {
            LaserPowerStatus = value;
        }

        private void UpdateInterlockStatus(string interlockStatus)
        {
            InterlockStatus = interlockStatus;
        }

        private void UpdateLaserTemperature(double laserTemperature)
        {
            LaserTemperature = laserTemperature;
        }

        private void UpdateCrystalTemperature(double crystalTemperature)
        {
            CrystalTemperature = crystalTemperature;
        }

        private void UpdateAttenuationPosition(double attenuationPosition)
        {
            BeamShaperPosition = attenuationPosition;
        }

        private void UpdateFastAttenuationPosition(double fastAttenuationPosition)
        {
            FastAttenuationPosition = fastAttenuationPosition;
        }
        
        private void UpdateShutterIrisPosition(string shutterIrisPosition)
        {
            ShutterIrisPosition = shutterIrisPosition;
        }

        private bool _laserPowerOn;

        public bool LaserPowerStatus
        {
            get => _laserPowerOn;
            set
            {
                if (_laserPowerOn != value)
                {
                    _laserPowerOn = value;
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }

        private double _beamShaperPosition;

        public double BeamShaperPosition
        {
            get => _beamShaperPosition;
            set
            {
                if (_beamShaperPosition != value)
                {
                    _beamShaperPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _fastAttenuationPosition;

        public double FastAttenuationPosition
        {
            get => _fastAttenuationPosition;
            set
            {
                if (_fastAttenuationPosition != value)
                {
                    _fastAttenuationPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _shutterIrisPosition;

        public string ShutterIrisPosition
        {
            get => _shutterIrisPosition;
            set
            {
                if (_shutterIrisPosition != value)
                {
                    _shutterIrisPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region RelayCommands

        private RelayCommand _powerOnCommand;

        public RelayCommand PowerOnCommand
        {
            get
            {
                return _powerOnCommand ?? (_powerOnCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOn());
                    }));
            }
        }

        private RelayCommand _powerOffCommand;

        public RelayCommand PowerOffCommand
        {
            get
            {
                return _powerOffCommand ?? (_powerOffCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOff());
                    }));
            }
        }

        private RelayCommand _applyPowerCommand;

        public RelayCommand ApplyPowerCommand
        {
            get
            {
                return _applyPowerCommand ?? (_applyPowerCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetPower(PowerSetpoint));
                    }));
            }
        }

        private RelayCommand _attenuationhomePositionCommand;

        public RelayCommand AttenuationHomePositionCommand
        {
            get
            {
                return _attenuationhomePositionCommand ?? (_attenuationhomePositionCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.AttenuationHomePosition());
                    }));
            }
        }

        private ServoPosition _attenuationPosition = ServoPosition.Pos0;

        public ServoPosition AttenuationPosition
        {
            get => _attenuationPosition;
            set
            {
                if (_attenuationPosition != value)
                {
                    _attenuationPosition = value;
                    Task.Run(() => _supervisor.AttenuationMoveAbsPosition(value));
                    RaisePropertyChanged();
                }
            }
        }

        /*private RelayCommand _attenuationMoveAbsPosition0Command;

        public RelayCommand AttenuationMoveAbsPosition0Command
        {
            get
            {
                return _attenuationMoveAbsPosition0Command ?? (_attenuationMoveAbsPosition0Command = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.AttenuationMoveAbsPosition(ServoPosition.Pos0));
                    }));
            }
        }

        private RelayCommand _attenuationMoveAbsPosition1Command;

        public RelayCommand AttenuationMoveAbsPosition1Command
        {
            get
            {
                return _attenuationMoveAbsPosition1Command ?? (_attenuationMoveAbsPosition1Command = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.AttenuationMoveAbsPosition(ServoPosition.Pos1));
                    }));
            }
        }*/

        private RelayCommand _fastAttenuationMoveAbsPositionCommand;

        public RelayCommand FastAttenuationMoveAbsPositionCommand
        {
            get
            {
                return _fastAttenuationMoveAbsPositionCommand ?? (_fastAttenuationMoveAbsPositionCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.FastAttenuationMoveAbsPosition(FastAttenuationAbsPositionSetpoint));
                    }));
            }
        }

        private RelayCommand _openShutterCommand;

        public RelayCommand OpenShutterCommand
        {
            get
            {
                return _openShutterCommand ?? (_openShutterCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.OpenShutterCommand());
                    }));
            }
        }

        private RelayCommand _closeShutterCommand;

        public RelayCommand CloseShutterCommand
        {
            get
            {
                return _closeShutterCommand ?? (_closeShutterCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CloseShutterCommand());
                    }));
            }
        }

        #endregion RelayCommands
    }
}
