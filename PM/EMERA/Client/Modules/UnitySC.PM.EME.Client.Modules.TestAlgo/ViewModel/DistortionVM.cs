using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class DistortionVM : AlgoBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;
        public IEnumerable<ScanRangeType> ScanRanges { get; private set; }

        public bool IsStreaming { get; set; }


        public DistortionVM() : base("Distortion")
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();            
        }

        private double _gaussianSigma = 0.5;

        public double GaussianSigma
        {
            get => _gaussianSigma;
            set { if (_gaussianSigma != value) { _gaussianSigma = value; OnPropertyChanged(); } }
        }

        private DistortionResult _result;

        public DistortionResult Result
        {
            get => _result;
            set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _startDistortion;

        public AutoRelayCommand StartDistortion
        {
            get
            {
                return _startDistortion ?? (_startDistortion = new AutoRelayCommand(
                    () =>
                    {
                        Result = null;
                        IsBusy = true;
                        _algoSupervisor.DistortionChangedEvent += DistortionVM_DistortionChangedEvent;
                        var distortionInput = new DistortionInput(_gaussianSigma);
                        _algoSupervisor.StartDistortion(distortionInput);
                    },
                    () => { return true; }));
            }
        }


        private void DistortionVM_DistortionChangedEvent(DistortionResult distortionResult)
        {
            UpdateResult(distortionResult);
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  _algoSupervisor.CancelDistortion();
                  _algoSupervisor.DistortionChangedEvent -= DistortionVM_DistortionChangedEvent;
                  IsBusy = false;
              }
              ));
            }
        }

        public void UpdateResult(DistortionResult distortionResult)
        {
            Result = distortionResult;
            if (distortionResult.Status.IsFinished)
            {
                _algoSupervisor.DistortionChangedEvent -= DistortionVM_DistortionChangedEvent;
                IsBusy = false;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.DistortionChangedEvent -= DistortionVM_DistortionChangedEvent;
                Result = null;
            }
        }       
    }
}
