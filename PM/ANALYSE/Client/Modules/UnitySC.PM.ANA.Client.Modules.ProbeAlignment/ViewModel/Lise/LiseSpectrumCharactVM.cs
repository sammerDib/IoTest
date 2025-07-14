using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Probe;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise
{
    public class LiseSpectrumCharactVM : ObservableObject, IDisposable
    {
        #region Fields

        private string _probeRef;
        private bool _isMeasurementRunning = false;
        private AsyncRelayCommand _startCommand;
        private AsyncRelayCommand _stopCommand;
        private double _wavelength = 0;
        private double _broadness = 0;
        private double _wavelengthSample = 0;
        private double _broadnessSample = 0;
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

        public double Wavelength
        {
            get => _wavelength;
            set => SetProperty(ref _wavelength, value);
        }

        public double Broadness
        {
            get => _broadness;
            set => SetProperty(ref _broadness, value);
        }

        public double WavelengthSample
        {
            get => _wavelengthSample;
            set => SetProperty(ref _wavelengthSample, value);
        }

        public double BroadnessSample
        {
            get => _broadnessSample;
            set => SetProperty(ref _broadnessSample, value);
        }

        #endregion

        #region Constructors

        public LiseSpectrumCharactVM(ProbeBaseVM probeLiseHFVM)
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
        }

        private async Task StopMeasurement()
        {
            IsMeasurementRunning = false;
        }

        #endregion
    }
}
