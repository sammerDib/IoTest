using System;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class PixelSizeComputationVM : AlgoBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;

        public PixelSizeComputationVM() : base("Pixel Size Computation")
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
        }

        private void AlgosSupervisor_PixelSizeComputationChangedEvent(PixelSizeComputationResult pixelSizeComputationResult)
        {
            UpdateResult(pixelSizeComputationResult);
        }

        private PixelSizeComputationResult _result;

        public PixelSizeComputationResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startPixelSizeComputation;

        public AutoRelayCommand StartPixelSizeComputation
        {
            get
            {
                return _startPixelSizeComputation ?? (_startPixelSizeComputation = new AutoRelayCommand(
              () =>
              {
                  Result = null;
                  IsBusy = true;

                  var pixelSizeComputationInput = new PixelSizeComputationInput();

                  _algoSupervisor.PixelSizeComputationChangedEvent += AlgosSupervisor_PixelSizeComputationChangedEvent;
                  _algoSupervisor.StartPixelSizeComputation(pixelSizeComputationInput);
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
                  _algoSupervisor.CancelPixelSizeComputation();
                  _algoSupervisor.PixelSizeComputationChangedEvent -= AlgosSupervisor_PixelSizeComputationChangedEvent;
                  IsBusy = false;
              },
              () => { return true; }));
            }
        }

        public void UpdateResult(PixelSizeComputationResult pixelSizeComputationResult)
        {
            Result = pixelSizeComputationResult;
            if (Result.Status.IsFinished)
            {
                _algoSupervisor.PixelSizeComputationChangedEvent -= AlgosSupervisor_PixelSizeComputationChangedEvent;
                IsBusy = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.PixelSizeComputationChangedEvent -= AlgosSupervisor_PixelSizeComputationChangedEvent;
                Result = null;
            }            
        }
    }
}
