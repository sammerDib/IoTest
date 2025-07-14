using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class DistanceSensorCalibrationVM : AlgoBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;

        public DistanceSensorCalibrationVM() : base("Distance Sensor Calibration")
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
        }

        private void AlgosSupervisor_DistanceSensorCalibrationChangedEvent(DistanceSensorCalibrationResult distanceSensorCalibrationResult)
        {
            UpdateResult(distanceSensorCalibrationResult);
        }

        private DistanceSensorCalibrationResult _result;

        public DistanceSensorCalibrationResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startDistanceSensorCalibration;

        public AutoRelayCommand StartDistanceSensorCalibration
        {
            get
            {
                return _startDistanceSensorCalibration ?? (_startDistanceSensorCalibration = new AutoRelayCommand(
              () =>
              {
                  Result = null;
                  IsBusy = true;

                  var distanceSensorCalibrationInput = new DistanceSensorCalibrationInput();

                  _algoSupervisor.DistanceSensorCalibrationChangedEvent += AlgosSupervisor_DistanceSensorCalibrationChangedEvent;
                  _algoSupervisor.StartDistanceSensorCalibration(distanceSensorCalibrationInput);
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  _algoSupervisor.CancelDistanceSensorCalibration();
                  _algoSupervisor.DistanceSensorCalibrationChangedEvent -= AlgosSupervisor_DistanceSensorCalibrationChangedEvent;
                  IsBusy = false;
              },
              () => { return true; }));
            }
        }

        public void UpdateResult(DistanceSensorCalibrationResult distanceSensorCalibrationResult)
        {
            Result = distanceSensorCalibrationResult;
            if (Result.Status.IsFinished)
            {
                _algoSupervisor.DistanceSensorCalibrationChangedEvent -= AlgosSupervisor_DistanceSensorCalibrationChangedEvent;
                IsBusy = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.DistanceSensorCalibrationChangedEvent -= AlgosSupervisor_DistanceSensorCalibrationChangedEvent;
                Result = null;
            }
        }
    }
}
