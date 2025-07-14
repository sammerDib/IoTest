using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class AutoFocusVM : AlgoBaseVM
    {
        private readonly CameraBench _cameraBench;
        private readonly IAlgoSupervisor _algoSupervisor;

        public AutoFocusVM() : base("Autofocus")
        {
            _cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
            ScanRanges = Enum.GetValues(typeof(ScanRangeType)).Cast<ScanRangeType>().Where(e => !e.Equals(ScanRangeType.Configured));
            SelectedRange = ScanRangeType.Small;
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();            
        }

        private AutoFocusCameraResult _cameraAutoFocusResult;

        public AutoFocusCameraResult CameraAutoFocusResult
        {
            get => _cameraAutoFocusResult;
            set => SetProperty(ref _cameraAutoFocusResult, value);
        }

        public IEnumerable<ScanRangeType> ScanRanges { get; private set; }

        private ScanRangeType _selectedRange;

        public ScanRangeType SelectedRange
        {
            get => _selectedRange;
            set => SetProperty(ref _selectedRange, value);
        }

        private AutoRelayCommand _startCameraAutoFocus;

        public AutoRelayCommand StartCameraAutoFocus =>
            _startCameraAutoFocus ?? (_startCameraAutoFocus = new AutoRelayCommand(PerformStartCameraAutoFocus));

        private void PerformStartCameraAutoFocus()
        {
            if (!_cameraBench.IsStreaming)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Camera must be started",
                    "Camera", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CameraAutoFocusResult = null;
            IsBusy = true;
            _algoSupervisor.AutoFocusCameraChangedEvent += AutoFocusCameraVM_AutoFocusCameraChangedEvent;
            var autoFocusCameraInput = new AutoFocusCameraInput(SelectedRange);
            _algoSupervisor.StartAutoFocusCamera(autoFocusCameraInput);
        }

        private void AutoFocusCameraVM_AutoFocusCameraChangedEvent(AutoFocusCameraResult autoFocusCameraResult)
        {
            UpdateResult(autoFocusCameraResult);
        }

        private void UpdateResult(AutoFocusCameraResult autoFocusResult)
        {
            CameraAutoFocusResult = autoFocusResult;
            if (autoFocusResult.Status.IsFinished)
            {
                _algoSupervisor.AutoFocusCameraChangedEvent -= AutoFocusCameraVM_AutoFocusCameraChangedEvent;
                IsBusy = false;
            }
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
                () =>
                {
                    _algoSupervisor.CancelAutoFocusCamera();
                    _algoSupervisor.AutoFocusCameraChangedEvent -= AutoFocusCameraVM_AutoFocusCameraChangedEvent;
                    _algoSupervisor.CancelGetZFocus();
                    _algoSupervisor.GetZFocusChangedEvent -= OnGetZFocusChangedEvent;
                    IsBusy = false;
                }
            ));

        private double _targetDistance;

        public double TargetDistance
        {
            get => _targetDistance;
            set => SetProperty(ref _targetDistance, value);
        }

        private AutoRelayCommand _startDistanceSensorAutoFocus;

        public AutoRelayCommand StartDistanceSensorAutoFocus =>
            _startDistanceSensorAutoFocus ?? (_startDistanceSensorAutoFocus =
                new AutoRelayCommand(PerformStartDistanceSensorAutoFocus));

        private void PerformStartDistanceSensorAutoFocus()
        {
            IsBusy = true;
            DistanceSensorAutoFocusResult = null;
            _algoSupervisor.GetZFocusChangedEvent += OnGetZFocusChangedEvent;
            var input = new GetZFocusInput { TargetDistanceSensor = TargetDistance };
            _algoSupervisor.StartGetZFocus(input);
        }

        private void OnGetZFocusChangedEvent(GetZFocusResult result)
        {
            DistanceSensorAutoFocusResult = result;
            if (result.Status.IsFinished)
            {
                DistanceSensorAutoFocusResult = result;
                _algoSupervisor.GetZFocusChangedEvent -= OnGetZFocusChangedEvent;
                IsBusy = false;
            }
        }

        private GetZFocusResult _distanceSensorAutoFocusResult;

        public GetZFocusResult DistanceSensorAutoFocusResult
        {
            get => _distanceSensorAutoFocusResult;
            set => SetProperty(ref _distanceSensorAutoFocusResult, value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.AutoFocusCameraChangedEvent -= AutoFocusCameraVM_AutoFocusCameraChangedEvent;
                _algoSupervisor.GetZFocusChangedEvent -= OnGetZFocusChangedEvent;
                CameraAutoFocusResult = null;
            }
        }
    }
}
