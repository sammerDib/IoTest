using System;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public sealed class AxisOrthogonalityCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;
        public AxisOrthogonalityCalibrationVM(ICalibrationService calibrationService, FilterWheelBench filterWheelBench) 
            : base("Axis orthogonality", calibrationService)
        {
            BusyMessage = "Checking XY axes...";
            FilterWheelBench = filterWheelBench;
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            _algoSupervisor.AxisOrthogonalityChangedEvent += AxisOrthogonalityVM_AxisOrthogonalityChangedEvent;            
        }

        private AxisOrthogonalityResult _result;

        public AxisOrthogonalityResult Result
        {
            get => _result;
            set
            {                
                SetProperty(ref _result, value);               
                UpdateAllCanExecutes();
            }
        }

        private FilterWheelBench _filterWheelBench;
        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }

        private bool IsReadyToValidate => Result?.Status.State == FlowState.Success;

        private string _validationErrorMessage = string.Empty;
        public string ValidationErrorMessage
        {
            get => _validationErrorMessage;
            set => SetProperty(ref _validationErrorMessage, value);
        }

        private AutoRelayCommand _startAxisOrthogonality;

        public AutoRelayCommand StartAxisOrthogonality
        {
            get
            {
                return _startAxisOrthogonality ?? (_startAxisOrthogonality = new AutoRelayCommand(
              () =>
              {
                Result = null;
                IsBusy = true;
                GetZFocusInput getZFocusInput = null;
                if (_filterWheelBench.CurrentFilter != null)
                {
                      var currentFilterFocusDistance = _filterWheelBench.CurrentFilter.DistanceOnFocus;
                      getZFocusInput = new GetZFocusInput() { TargetDistanceSensor = currentFilterFocusDistance };
                }
                var axisOrthoInput = new AxisOrthogonalityInput(getZFocusInput);
                _algoSupervisor.StartAxisOrthogonality(axisOrthoInput);
              }));
            }
        }
        private AutoRelayCommand _cancelAxisOrthogonality;

        public AutoRelayCommand CancelAxisOrthogonality
        {
            get
            {
                return _cancelAxisOrthogonality ?? (_cancelAxisOrthogonality = new AutoRelayCommand(
              () =>
              {
                  IsBusy = false;
                  _algoSupervisor.CancelAxisOrthogonality();
              }));
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

        private AutoRelayCommand _saveCalibration;
        public AutoRelayCommand SaveCalibration =>
           _saveCalibration ?? (_saveCalibration = new AutoRelayCommand(Save, CanSave));

        private void UpdateValidationErrorMessage()
        {
            switch (Result?.Status.State)
            {
                case FlowState.Error:                
                    ValidationErrorMessage = "Axis orthogonality is not valid";
                    break;
                default:
                    ValidationErrorMessage = string.Empty;                    
                    break;
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
            HasChanged = false;
            return true;
        }

        public override void Save()
        {
            _calibrationService.SaveCalibration(new AxisOrthogonalityCalibrationData
            {
                CreationDate = DateTime.Now,
                AngleX = Result.XAngle,
                AngleY = Result.YAngle
            });
            HasChanged = false;
            ValidateAndEnableNextPage();
        }

        public override bool CanSave()
        {
            return HasChanged && (Result?.Status.State == FlowState.Success);
        }

        public override void Init()
        {
            HasChanged = false;
            var calib = LoadCalibrationData<AxisOrthogonalityCalibrationData>();
            if (calib == null)
            {
                return;
            }
            Result = new AxisOrthogonalityResult()
            {
                XAngle = calib.AngleX,
                YAngle = calib.AngleY,
                Status = new FlowStatus()
                {
                    Message = "Loaded from previous calibration",
                    State = FlowState.Success,
                }
            };
        }

        private void AxisOrthogonalityVM_AxisOrthogonalityChangedEvent(AxisOrthogonalityResult orthoResult)
        {
            UpdateResult(orthoResult);
        }

        public void UpdateResult(AxisOrthogonalityResult orthoResult)
        {
            Result = orthoResult;
            HasChanged = true;
            if (orthoResult.Status.IsFinished)
            {
                IsBusy = false;
            }
        }

        public override void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                _algoSupervisor.AxisOrthogonalityChangedEvent -= AxisOrthogonalityVM_AxisOrthogonalityChangedEvent;
            }
        }
    }
}
