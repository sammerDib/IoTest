using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Probe;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise
{
    public class LiseBeamProfilerVM : ObservableObject, IDisposable
    {
        #region Fields

        private string _probeRef;
        private bool _isMeasurementRunning = false;
        private AsyncRelayCommand _startCommand;
        private AsyncRelayCommand _stopCommand;
        private bool _isSuccess = false;
        private double _amplitude = -1;
        private double _gaussX = -1;
        private double _gaussY = -1;
        private double _radius = -1;
        private double _background = -1;
        private double _norme = -1;
        private double _weightedNorme = -1;
        private double _ellipseAxisRatio = -1;
        private double _ellipseAngle = -1;
        private ProbeBaseVM _probeLiseHFVM;

        #endregion

        #region Properties

        public string ProbeRef
        {
            get => _probeRef;
            set => SetProperty(ref _probeRef, value);
        }
        
        public bool IsMeasurementRunning
        {
            get => _isMeasurementRunning;
            set => SetProperty(ref _isMeasurementRunning, value);
        }

        public AsyncRelayCommand StartCommand
        {
            get => _startCommand ?? (_startCommand = new AsyncRelayCommand(StartMeasurement));
        }
        
        public AsyncRelayCommand StopCommand
        {
            get => _stopCommand ?? (_stopCommand = new AsyncRelayCommand(StopMeasurement));
        }
        
        public bool IsSuccess
        {
            get => _isSuccess;
            set => SetProperty(ref _isSuccess, value);
        }
        
        public double Amplitude
        {
            get => _amplitude;
            set => SetProperty(ref _amplitude, value);
        }
        
        public double GaussX
        {
            get => _gaussX;
            set => SetProperty(ref _gaussX, value);
        }

        public double GaussY
        {
            get => _gaussY;
            set => SetProperty(ref _gaussY, value);
        }

        public double Radius
        {
            get => _radius;
            set => SetProperty(ref _radius, value);
        }

        public double Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }

        public double Norme
        {
            get => _norme;
            set => SetProperty(ref _norme, value);
        }

        public double WeightedNorme
        {
            get => _weightedNorme;
            set => SetProperty(ref _weightedNorme, value);
        }


        public double EllipseAxisRatio
        {
            get => _ellipseAxisRatio;
            set => SetProperty(ref _ellipseAxisRatio, value);
        }

        public double EllipseAngle
        {
            get => _ellipseAngle;
            set => SetProperty(ref _ellipseAngle, value);
        }

        #endregion

        #region Constructors

        public LiseBeamProfilerVM(ProbeBaseVM probeLiseHFVM)
        {
            _probeLiseHFVM = probeLiseHFVM;
            ProbeRef = probeLiseHFVM.Name;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            StopMeasurement().Wait();
        }

        private async Task StartMeasurement()
        {
            IsMeasurementRunning = true;
            SimulateValues();
        }

        private async Task StopMeasurement()
        {
            IsMeasurementRunning = false;
        }

        // TODO ONLY FOR DEV
        private async Task SimulateValues()
        {
            var rand = new Random(123);
            while (IsMeasurementRunning)
            {
                IsSuccess = rand.NextDouble() >= 0.5;
                await Task.Delay(500);
            }
        }

        #endregion
    }
}
