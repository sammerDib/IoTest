using System;
using System.ComponentModel;
using System.Windows;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class DistortionCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;
        private readonly IDialogOwnerService _dialogOwnerService;
        public DistortionCalibrationVM(ICalibrationService calibrationService, IDialogOwnerService dialogOwnerService) : base("Distortion Calibration", calibrationService)
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            _dialogOwnerService = dialogOwnerService;
            _algoSupervisor.DistortionChangedEvent += DistortionVM_DistortionChangedEvent;
            this.PropertyChanged += DistortionCalibrationVM_PropertyChanged;           
        }

        private void DistortionCalibrationVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasChanged))
            {
                UpdateAllCanExecutes();
            }
        }

        private void DistortionVM_DistortionChangedEvent(DistortionResult distortionResult)
        {
            UpdateResult(distortionResult);
        }

        private void UpdateResult(DistortionResult distortionResult)
        {
            Result = distortionResult;
            IsBusy = !distortionResult.Status.IsFinished;
            if (distortionResult.Status.IsFinished &&
                distortionResult.Status.State == PM.Shared.Flow.Interface.FlowState.Success)
            {
                HasChanged = true;
            }
        }

        private double _gaussianSigma = 0.5;

        private DistortionResult _result;
        public DistortionResult Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
                UpdateAllCanExecutes();
            }
        }
        private AutoRelayCommand _startDistortionCalibration;
        public AutoRelayCommand StartDistortionCalibration
        {
            get
            {
                return _startDistortionCalibration ?? (_startDistortionCalibration = new AutoRelayCommand(
              () =>
              {
                  Result = null;
                  IsBusy = true;

                  var distortionInput = new DistortionInput(_gaussianSigma);
                  _algoSupervisor.StartDistortion(distortionInput);
              }));
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
                  IsBusy = false;
                  _algoSupervisor.CancelDistortion();
              }));
            }
        }

        private AutoRelayCommand _saveCalibration;

        public AutoRelayCommand SaveCalibration
        {
            get
            {
                return _saveCalibration ?? (_saveCalibration = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      Save();
                  }
                  catch (Exception ex)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during distortion calibration saving");
                  }
              },
              () => { return CanSave(); }));
            }
        }

        public override bool CanCancelChanges()
        {
            return true;
        }

        public override void CancelChanges()
        {
            Init();
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (HasChanged && !forceClose)
            {
                var dialogRes = _dialogOwnerService.ShowMessageBox("The Distortion Calibration has changed. Do you really want to quit without saving ?", "Distortion Calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
                    return true;
                }
                return false;
            }
            return true;
        }

        public override bool CanSave()
        {
            return HasChanged && (Result?.Status.State == FlowState.Success);
        }

        public override void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                _algoSupervisor.DistortionChangedEvent -= DistortionVM_DistortionChangedEvent;
                this.PropertyChanged -= DistortionCalibrationVM_PropertyChanged;
            }
        }

        public override void Init()
        {
            HasChanged = false;
            var calib = LoadCalibrationData<DistortionCalibrationData>();
            if (calib == null)
            {
                return;
            }
            Result = new DistortionResult()
            {
                DistortionData = calib.DistortionData,
                Status = new FlowStatus()
                {
                    Message = "Loaded from previous calibration",
                    State = FlowState.Success
                }
            };
        }

        public override void Save()
        {
            var distortionCalibrationData = new DistortionCalibrationData();
            distortionCalibrationData.DistortionData = Result.DistortionData;
            _calibrationService.SaveCalibration(distortionCalibrationData);
            HasChanged = false;
            IsReadyToValidate = true;
            ValidateAndEnableNextPage();
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
                    ValidationErrorMessage = "Distortion Calibration calibration is not valid.";
                    break;
                default:
                    ValidationErrorMessage = string.Empty;
                    break;
            }
        }
    }
}
