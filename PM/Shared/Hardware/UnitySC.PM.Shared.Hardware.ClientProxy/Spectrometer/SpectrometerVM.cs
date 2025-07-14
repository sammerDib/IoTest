using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using LightningChartLib.WPF.ChartingMVVM;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Spectrometer
{
    public class SpectrometerVM : ObservableObject
    {
        private SpectrometerSupervisor _supervisor;
        private AutoRelayCommand _doMeasure;
        private AutoRelayCommand _startContinuousAcquisition;
        private AutoRelayCommand _stopContinuousAcquisition;

        private SeriesPoint[] _rawAcquisitionPoints;
        private double _xAxis = 1000;
        private double _xMinAxis = 100;

        private double _yAxis = 1000;

        public int NbAverage { get; set; } = 8;
        public double IntegrationTimeMs { get; set; } = 10.0;

        public int PowerSetpoint { get; set; }
        public string CustomTxt { get; set; }
        public string Status { get; set; }

        public SpectrometerVM(SpectrometerSupervisor supervisor)
        {
            _supervisor = supervisor;
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

        public AutoRelayCommand DoMeasure
        {
            get
            {
                return _doMeasure ?? (_doMeasure = new AutoRelayCommand(
                () =>
                {
                    Task.Run(() => _supervisor.DoMeasure(new SpectrometerParamBase(IntegrationTimeMs, NbAverage)));
                }));
            }
        }

        public AutoRelayCommand StartContinuousAcquisition
        {
            get
            {
                return _startContinuousAcquisition ?? (_startContinuousAcquisition = new AutoRelayCommand(
                () =>
                {
                    Task.Run(() => _supervisor.StartContinuousAcquisition(new SpectrometerParamBase(IntegrationTimeMs, NbAverage)));
                }));
            }
        }

        public AutoRelayCommand StopContinuousAcquisition
        {
            get
            {
                return _stopContinuousAcquisition ?? (_stopContinuousAcquisition = new AutoRelayCommand(
                () =>
                {
                    Task.Run(() => _supervisor.StopContinuousAcquisition());
                }));
            }
        }

        private bool _isActivate;

        public bool IsActivate
        {
            get => _isActivate;
            set
            {
                if (_isActivate != value)
                {
                    _isActivate = value;
                    OnPropertyChanged();
                }
            }
        }

        public SeriesPoint[] RawAcquisitionPoints
        {
            get { return _rawAcquisitionPoints; }
            set
            {
                _rawAcquisitionPoints = value;
                OnPropertyChanged();
            }
        }

        private void UpdateRawSignal(SpectroSignal spectroSignal)
        {
            try
            {
                var rawAcquisitionPoints = new SeriesPoint[spectroSignal.Wave.Count];

                for (int i = 0; i < spectroSignal.Wave.Count; i++)
                {
                    rawAcquisitionPoints[i] = new SeriesPoint() { X = spectroSignal.Wave[i], Y = spectroSignal.RawValues[i] };
                }
                RawAcquisitionPoints = rawAcquisitionPoints;

                AutoScale();
            }
            catch (Exception)
            {
                // When the Lise Graph control is unloaded an exception can be generated due to the binding
            }
        }

        private void AutoScale()
        {
            var query = RawAcquisitionPoints.GroupBy(i => i.Tag).Select(grp => new
            {
                XMax = grp.Max(t => t.X),
                YMax = grp.Max(t => t.Y),
                XMin = grp.Min(t => t.X),
            });

            XAxis = query.Max(r => r.XMax);
            YAxis = query.Max(r => r.YMax);
            XMinAxis = query.Min(r => r.XMin);
        }

        public double XAxis
        {
            get { return _xAxis; }
            set
            {
                _xAxis = value;
                OnPropertyChanged();
            }
        }

        public double XMinAxis
        {
            get { return _xMinAxis; }
            set
            {
                _xMinAxis = value;
                OnPropertyChanged();
            }
        }

        public double YAxis
        {
            get { return _yAxis; }
            set
            {
                _yAxis = value;
                OnPropertyChanged();
            }
        }

        public void SetRawSignal(SpectroSignal spectroSignal)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => UpdateRawSignal(spectroSignal)));
        }
    }
}
