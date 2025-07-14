using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class AutoExposureVM : AlgoBaseVM
    {
        private readonly IAlgoSupervisor _algoSupervisor;

        public AutoExposureVM(FilterWheelBench filterWheelBench) : base("Camera Exposure")
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            FilterWheelBench = filterWheelBench;
        }

        private void UpdateResult(AutoExposureResult result)
        {
            Result = result;
            if (result.Status.IsFinished)
            {
                _algoSupervisor.CameraExposureChangedEvent -= UpdateResult;
                IsBusy = false;
            }
        }

        private double _targetBrightness = 0.80;

        public double TargetBrightness
        {
            get => _targetBrightness;
            set => SetProperty(ref _targetBrightness, value);
        }

        private double _toleranceBrightness = 0.05;

        public double DecimalToleranceBrightness
        {
            get => _toleranceBrightness;
            set => SetProperty(ref _toleranceBrightness, value);
        }

        private AutoExposureResult _result;

        public AutoExposureResult Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }


        private RelayCommand _calibrateExposure;

        public IRelayCommand CalibrateExposure
        {
            get
            {
                if (_calibrateExposure == null)
                    _calibrateExposure = new RelayCommand(PerformCalibrateExposure);

                return _calibrateExposure;
            }
        }

        private void PerformCalibrateExposure()
        {
            IsBusy = true;
            _algoSupervisor.CameraExposureChangedEvent += UpdateResult;
            var input = new AutoExposureInput();
            _algoSupervisor.StartAutoExposure(input);
        }


        private RelayCommand _cancel;

        public IRelayCommand Cancel
        {
            get
            {
                if (_cancel == null)
                {
                    _cancel = new RelayCommand(PerformCancel);                    
                }

                return _cancel;
            }
        }

        private FilterWheelBench _filterWheelBench;

        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }

        private void PerformCancel()
        {
            _algoSupervisor.CancelAutoExposure();
            _algoSupervisor.CameraExposureChangedEvent -= UpdateResult;
            IsBusy = false;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.CameraExposureChangedEvent -= UpdateResult;
                Result = null;
            }
        }      
    }
}
