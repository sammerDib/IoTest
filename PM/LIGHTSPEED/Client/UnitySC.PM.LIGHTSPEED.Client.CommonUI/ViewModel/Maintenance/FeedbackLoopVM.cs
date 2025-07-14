using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance
{
    public class FeedbackLoopVM : ViewModelBase
    {
        private IFeedbackLoopService _supervisor;
        private System.Timers.Timer _aTimer;
        private DateTime _startDate;

        private const double HS_POSITION = 188.1;
        private const double HT_POSITION = 97.1;

        public int PowerSetpoint { get; set; }
        public int CurrentSetpoint { get; set; }

        public string SaveFile { get; set; } = "C:\\Unity\\MonitoringBeamFlow\\Montoring.csv";

        public FeedbackLoopVM(IFeedbackLoopService supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<Proxy.FeedbackLoop.PowerChangedMessage>(this, (m) => { UpdatePower(m.Flow, m.Power); });
            Messenger.Register<Proxy.FeedbackLoop.WavelengthChangedMessage>(this, (m) => { UpdateWavelength(m.Flow, m.Wavelength); });
            Messenger.Register<Proxy.FeedbackLoop.BeamDiameterChangedMessage>(this, (m) => { UpdateBeamDiameter(m.Flow, m.BeamDiameter); });
            Messenger.Register<Proxy.FeedbackLoop.PowerLaserChangedMessage>(this, (m) => { UpdatePowerLaser(m.Power); });
            Messenger.Register<Proxy.FeedbackLoop.InterlockStatusChangedMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            Messenger.Register<Proxy.FeedbackLoop.LaserTemperatureChangedMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            Messenger.Register<Proxy.FeedbackLoop.PsuTemperatureChangedMessage>(this, (m) => { UpdatePsuTemperature(m.PsuTemperature); });
            Messenger.Register<Proxy.FeedbackLoop.ShutterIrisPositionChangedMessages>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
            Messenger.Register<Proxy.FeedbackLoop.AttenuationPositionChangedMessages>(this, (m) => { UpdateAttenuationPosition(m.AttenuationPosition); });

            Task.Run(() => _supervisor.InitializeUpdate());

            _aTimer = new System.Timers.Timer();
            _aTimer.Interval = 1000;

            // Hook up the Elapsed event for the timer.
            _aTimer.Elapsed += OnTimedEvent;
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

        private void UpdatePower(PowerIlluminationFlow flow, double value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowerIlluminationHs = value;
            }
            else
            {
                PowerIlluminationHt = value;
            }
        }

        private void UpdateWavelength(PowerIlluminationFlow flow, uint value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                GetWavelengthHs = value;
            }
            else
            {
                GetWavelengthHt = value;
            }
        }

        private void UpdateBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                GetBeamDiameterHs = value;
            }
            else
            {
                GetBeamDiameterHt = value;
            }
        }

        private void UpdatePowerLaser(double value)
        {
            PowerLaser = value;
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

        private void UpdateShutterIrisPosition(string shutterIrisPosition)
        {
            ShutterIrisPosition = shutterIrisPosition;
        }

        private void UpdateAttenuationPosition(double attenuationPosition)
        {
            AttenuationPosition = attenuationPosition;
        }

        private double _powerIlluminationHs;

        public double PowerIlluminationHs
        {
            get => _powerIlluminationHs;
            set
            {
                if (_powerIlluminationHs != value)
                {
                    _powerIlluminationHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _wavelengthHs;

        public uint GetWavelengthHs
        {
            get => _wavelengthHs;
            set
            {
                if (_wavelengthHs != value)
                {
                    _wavelengthHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _beamDiameterHs;

        public uint GetBeamDiameterHs
        {
            get => _beamDiameterHs;
            set
            {
                if (_beamDiameterHs != value)
                {
                    _beamDiameterHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powerIlluminationHt;

        public double PowerIlluminationHt
        {
            get => _powerIlluminationHt;
            set
            {
                if (_powerIlluminationHt != value)
                {
                    _powerIlluminationHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _wavelengthHt;

        public uint GetWavelengthHt
        {
            get => _wavelengthHt;
            set
            {
                if (_wavelengthHt != value)
                {
                    _wavelengthHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _beamDiameterHt;

        public uint GetBeamDiameterHt
        {
            get => _beamDiameterHt;
            set
            {
                if (_beamDiameterHt != value)
                {
                    _beamDiameterHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powerLaser;

        public double PowerLaser
        {
            get => _powerLaser;
            set
            {
                if (_powerLaser != value)
                {
                    _powerLaser = value;
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

        private double _psuTemperature;

        public double PsuTemperature
        {
            get => _psuTemperature;
            set
            {
                if (_psuTemperature != value)
                {
                    _psuTemperature = value;
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

        private double _attenuationPosition;

        public double AttenuationPosition
        {
            get => _attenuationPosition;
            set
            {
                if (_attenuationPosition != value)
                {
                    _attenuationPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _recordingTime = "0 day(s) 0:0:0";

        public string RecordingTime
        {
            get => _recordingTime;
            set
            {
                if (_recordingTime != value)
                {
                    _recordingTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isMonitoringRunning;

        public bool IsMonitoringRunning
        {
            get => _isMonitoringRunning;
            set
            {
                if (_isMonitoringRunning != value)
                {
                    _isMonitoringRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _numberPoints = 1;

        public uint NumberPoints
        {
            get => _numberPoints;
            set
            {
                if (_numberPoints != value)
                {
                    _numberPoints = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        private RelayCommand _applyCurrentCommand;

        public RelayCommand ApplyCurrentCommand
        {
            get
            {
                return _applyCurrentCommand ?? (_applyCurrentCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetCurrent(CurrentSetpoint));
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

        private RelayCommand _attenuationHomePosition;

        public RelayCommand AttenuationHomePosition
        {
            get
            {
                return _attenuationHomePosition ?? (_attenuationHomePosition = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.AttenuationHomePosition());
                    }));
            }
        }

        private PowerIlluminationFlow _powerIlluminationFlow = PowerIlluminationFlow.Unknown;

        public PowerIlluminationFlow PowerIlluminationFlow
        {
            get => _powerIlluminationFlow;
            set
            {
                if (_powerIlluminationFlow != value)
                {
                    _powerIlluminationFlow = value;

                    if (_powerIlluminationFlow == PowerIlluminationFlow.HS)
                    {
                        Task.Run(() => _supervisor.MoveAbsPosition(HS_POSITION));
                    }
                    else if (_powerIlluminationFlow == PowerIlluminationFlow.HT)
                    {
                        Task.Run(() => _supervisor.MoveAbsPosition(HT_POSITION));
                    }

                    RaisePropertyChanged();
                }
            }
        }

        private RelayCommand _startMonitoringCommand;

        public RelayCommand StartMonitoringCommand
        {
            get
            {
                return _startMonitoringCommand ?? (_startMonitoringCommand = new RelayCommand(
                    () =>
                    {
                        IsMonitoringRunning = true;

                        _startDate = DateTime.Now;

                        // Start the timer
                        _aTimer.Enabled = true;

                        Task.Run(() => StartMonitoring());
                    }));
            }
        }

        private RelayCommand _stopMonitoringCommand;

        public RelayCommand StopMonitoringCommand
        {
            get
            {
                return _stopMonitoringCommand ?? (_stopMonitoringCommand = new RelayCommand(
                    () =>
                    {
                        IsMonitoringRunning = false;
                        _aTimer.Enabled = false;
                        RecordingTime = "0 day(s) 0:0:0";
                    }));
            }
        }

        private void StartMonitoring()
        {
            try
            {
                string delimiter = ";";
                using (StreamWriter sw = File.CreateText(SaveFile))
                {
                    while (IsMonitoringRunning)
                    {
                        var powerIllumination = (PowerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? PowerIlluminationHs : PowerIlluminationHt;
                        sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", DateTime.Now.ToString("dddd HH:mm:ss:fff"), delimiter, powerIllumination, delimiter, PowerLaser, delimiter, LaserTemperature, delimiter, PsuTemperature));
                        Thread.Sleep(1000 / (int)NumberPoints);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Debug.WriteLine(msg);
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan interval = DateTime.Now - _startDate;
            RecordingTime = string.Format("{0} day(s) {1}:{2}:{3}", interval.Days, interval.Hours, interval.Minutes, interval.Seconds);
        }
    }
}
