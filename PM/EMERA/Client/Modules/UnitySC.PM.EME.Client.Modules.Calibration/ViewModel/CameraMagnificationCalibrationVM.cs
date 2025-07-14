using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class CameraMagnificationCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly IAlgoSupervisor _algoSupervisor;
        private readonly IDialogOwnerService _dialogOwnerService;
        public CameraMagnificationCalibrationVM(IAlgoSupervisor algoSupervisor, CalibrationConfiguration calibrationConfiguration, ICalibrationService calibrationService, IDialogOwnerService dialogOwnerService) : base("Camera Magnification", calibrationService)
        {
            _algoSupervisor = algoSupervisor;
            _dialogOwnerService = dialogOwnerService;
            _algoSupervisor.PixelSizeComputationChangedEvent += UpdatePixelSizeResult;
            this.PropertyChanged += CameraMagnificationVM_PropertyChanged;
            BusyMessage = "Measuring pixel size...";
            TargetPixelSize = calibrationConfiguration.TargetPixelSize;           
        }

        private void CameraMagnificationVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasChanged))
            {
                UpdateAllCanExecutes();
            }
        }

        private void UpdatePixelSizeResult(PixelSizeComputationResult pixelSizeComputationResult)
        {
            Result = pixelSizeComputationResult;
            IsBusy = !pixelSizeComputationResult.Status.IsFinished;
            if (pixelSizeComputationResult.Status.IsFinished &&
                pixelSizeComputationResult.Status.State == FlowState.Success)
            {
                HasChanged = true;
            }
        }

        public override void Init()
        {
            HasChanged = false;
            var calib = LoadCalibrationData<CameraCalibrationData>();
            if (calib == null)
            {
                return;
            }
            Result = new PixelSizeComputationResult()
            {
                PixelSize = calib.PixelSize,
                Status = new FlowStatus()
                {
                    Message = "Loaded from previous calibration",
                    State = FlowState.Success,
                }
            };
        }

        public override void CancelChanges()
        {
            Init();
        }

        public override bool CanCancelChanges()
        {
            return true;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (HasChanged && !forceClose)
            {
                var dialogRes = _dialogOwnerService.ShowMessageBox("The camera magnification has changed. Do you really want to quit without saving ?", "Camera Magnification", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void Save()
        {
            var calibration = new CameraCalibrationData() { PixelSize = Result.PixelSize };
            _calibrationService.SaveCalibration(calibration);
            HasChanged = false;
            IsReadyToValidate = true;
            ValidateAndEnableNextPage();
        }

        public override bool CanSave()
        {
            return HasChanged && (Result?.Status.State == FlowState.Success);
        }
        private PixelSizeComputationResult _result;

        public PixelSizeComputationResult Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
                IsGreenMarkDisplayed = IsPixelSizeCloseToTarget();
                UpdateAllCanExecutes();
            }
        }
        public Length TargetPixelSize { get; set; }

        private RelayCommand _measurePixelSize;

        public ICommand MeasurePixelSize
        {
            get
            {
                if (_measurePixelSize == null)
                {
                    _measurePixelSize = new RelayCommand(PerformPixelSizeMeasurement);
                }

                return _measurePixelSize;
            }
        }

        private void PerformPixelSizeMeasurement()
        {
            IsBusy = true;
            _algoSupervisor.StartPixelSizeComputation(new PixelSizeComputationInput());
        }

        private RelayCommand _cancel;

        public ICommand Cancel
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

        private void PerformCancel()
        {
            IsBusy = false;
            _algoSupervisor.CancelPixelSizeComputation();
        }


        private bool _isGreenMarkDisplayed;
        public bool IsGreenMarkDisplayed
        {
            get => _isGreenMarkDisplayed;
            set => SetProperty(ref _isGreenMarkDisplayed, value);
        }
        private bool _isReadyToValidate;
        public bool IsReadyToValidate
        {
            get => _isReadyToValidate;
            set
            {
                SetProperty(ref _isReadyToValidate, value);
                UpdateAllCanExecutes();
            }
        }
        private string _validationErrorMessage = string.Empty;
        public string ValidationErrorMessage
        {
            get => _validationErrorMessage;
            set => SetProperty(ref _validationErrorMessage, value);
        }

        public bool IsPixelSizeCloseToTarget()
        {
            if (Result == null || Result.PixelSize == null)
            {
                return false;
            }
            var lengthTolerance = new LengthTolerance(1, LengthToleranceUnit.Percentage);
            return lengthTolerance.IsInTolerance(Result.PixelSize, TargetPixelSize);
        }

        private AutoRelayCommand _saveCalibration;
        public AutoRelayCommand SaveCalibration
        {
            get
            {

                if (_saveCalibration == null)
                    _saveCalibration = new AutoRelayCommand(Save, CanSave);

                return _saveCalibration;
            }
        }
        private AutoRelayCommand _skipCommand;
        public AutoRelayCommand SkipCommand
        {
            get
            {
                return _skipCommand ?? (_skipCommand = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }
        private AutoRelayCommand _validateChoice;
        public AutoRelayCommand ValidateChoice
        {
            get
            {
                return _validateChoice ?? (_validateChoice = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    },
                    () =>
                    {
                        UpdateValidationErrorMessage();
                        return IsReadyToValidate;
                    }
                ));
            }
        }
        private void UpdateValidationErrorMessage()
        {
            switch (Result?.Status.State)
            {
                case FlowState.Error:
                    ValidationErrorMessage = "Camera magnification calibration is not valid.";
                    break;
                default:
                    ValidationErrorMessage = string.Empty;
                    break;
            }
        }
        public override void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                _algoSupervisor.PixelSizeComputationChangedEvent -= UpdatePixelSizeResult;
                this.PropertyChanged -= CameraMagnificationVM_PropertyChanged;
            }
        }
    }
}
