using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class ChuckManagerCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;
        private readonly ReferentialSupervisor _referentialSupervisor;
        private readonly ChuckVM _chuckVM;
        private readonly IDialogOwnerService _dialogOwnerService;
        private WaferReferentialCalibrationData _calibration;
        public ChuckManagerCalibrationVM(ICalibrationService calibrationService, FilterWheelBench filterWheelBench, IDialogOwnerService dialogOwnerService) 
            : base("Chuck Manager", calibrationService)
        {            
            FilterWheelBench = filterWheelBench;
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
            _chuckVM = ClassLocator.Default.GetInstance<ChuckVM>();
            _dialogOwnerService = dialogOwnerService;
            _algoSupervisor.MultiSizeChuckChangedEvent += MultiSizeChuckVM_MultiSizeChuckChangedEvent;
            WaferReferentialCalibrationData = _calibrationService.GetCalibrations()?.Result.OfType<WaferReferentialCalibrationData>().FirstOrDefault();           
            this.PropertyChanged += MultiSizeChuckManagerVM_PropertyChanged;
            _chuckVM.PropertyChanged += ChuckVM_PropertyChanged;                     
        }

        private void ChuckVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedWaferCategory" && _calibration != null)
            {
                UpdateResultAccordingToSelectedWaferCategory();
            }
        }

        private void MultiSizeChuckManagerVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasChanged))
            {
                UpdateAllCanExecutes();
            }
        }
        private void MultiSizeChuckVM_MultiSizeChuckChangedEvent(MultiSizeChuckResult multiSizeChuckResult)
        {
            UpdateResult(multiSizeChuckResult);
        }

        private void UpdateResultAccordingToSelectedWaferCategory()
        {
            var waferDiameter = _chuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter;
            var waferConfiguration = _calibration.WaferConfigurations?.Find(config => config?.WaferDiameter == waferDiameter)?.WaferReferentialSettings;
            if (waferConfiguration != null)
            {
                Result = new MultiSizeChuckResult()
                {
                    ShiftX = waferConfiguration.ShiftX,
                    ShiftY = waferConfiguration.ShiftY,
                    WaferAngle = waferConfiguration.WaferAngle,
                    Status = new FlowStatus()
                    {
                        Message = "Loaded from previous calibration",
                        State = FlowState.Success
                    }
                };
            }
            else
            {
                Result = null;
            }
        }

        private void UpdateResult(MultiSizeChuckResult multiSizeChuckResult)
        {
            Result = multiSizeChuckResult;
            IsBusy = !multiSizeChuckResult.Status.IsFinished;
            if (multiSizeChuckResult.Status.IsFinished &&
                multiSizeChuckResult.Status.State == PM.Shared.Flow.Interface.FlowState.Success)
            {
                HasChanged = true;
                var waferReferentialSettings = new WaferReferentialSettings
                {
                    ShiftX = multiSizeChuckResult.ShiftX,
                    ShiftY = multiSizeChuckResult.ShiftY
                };
                _referentialSupervisor.SetSettings(waferReferentialSettings);
            }
        }

        private FilterWheelBench _filterWheelBench;
        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }


        private MultiSizeChuckResult _result;
        public MultiSizeChuckResult Result
        {
            get => _result; 
            set 
            {
                SetProperty(ref _result, value);
                UpdateAllCanExecutes();
            }
        }
        private AutoRelayCommand _startAdjustPositionForWaferSize;
        public AutoRelayCommand StartAdjustingPositionForWaferSize
        {
            get
            {
                return _startAdjustPositionForWaferSize ?? (_startAdjustPositionForWaferSize = new AutoRelayCommand(
              () =>
              {
                  Result = null;
                  IsBusy = true;

                  var waferDiameter = _chuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter;
                  GetZFocusInput getZFocusInput = null;
                  if (_filterWheelBench.CurrentFilter != null)
                  {
                      var currentFilterFocusDistance = _filterWheelBench.CurrentFilter.DistanceOnFocus;
                      getZFocusInput = new GetZFocusInput() { TargetDistanceSensor = currentFilterFocusDistance };
                  }
                  var multiSizeChuckInput = new MultiSizeChuckInput(waferDiameter, getZFocusInput);
                  _algoSupervisor.StartMultiSizeChuck(multiSizeChuckInput);
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
                  _algoSupervisor.CancelMultiSizeChuck();
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
                      ApplyWaferReferentialSettings(Result);
                      Save();
                  }
                  catch (Exception ex)
                  {
                      _dialogOwnerService.ShowException(ex, "Error during registration");
                  }
                  finally
                  {
                      _chuckVM.SetReferentialForSelectedWafer();
                  }
              },
              () => { return CanSave(); }));
            }
        }

        private void ApplyWaferReferentialSettings(MultiSizeChuckResult result)
        {
            var waferReferentialSettings = new WaferReferentialSettings
            {
                ShiftX = result?.ShiftX ?? default,
                ShiftY = result?.ShiftY ?? default
            };

            var waferConfiguration = WaferReferentialCalibrationData.WaferConfigurations.Find(c => c.WaferDiameter == _chuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter);
            if (waferConfiguration == null)
            {
                WaferReferentialCalibrationData.WaferConfigurations.Add(new WaferConfiguration
                {
                    WaferDiameter = _chuckVM.SelectedWaferCategory.DimentionalCharacteristic.Diameter,
                    WaferReferentialSettings = waferReferentialSettings,
                });
            }
            else
            {
                waferConfiguration.WaferReferentialSettings = waferReferentialSettings;
            }
        }

        private AutoRelayCommand _undoCalibration;

        public AutoRelayCommand UndoCalibration
        {
            get
            {
                return _undoCalibration ?? (_undoCalibration = new AutoRelayCommand(
              () =>
              {
                  var res = _dialogOwnerService.ShowMessageBox("Calibration has changed. Do you really want to undo change", "Undo calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                  if (res == MessageBoxResult.Yes)
                  {
                      CancelChanges();
                  }
              },
              () => { return HasChanged && CanCancelChanges(); }));
            }
        }
        private WaferReferentialCalibrationData _waferReferentialCalibrationData;
        public WaferReferentialCalibrationData WaferReferentialCalibrationData
        {
            get
            {
                if (_waferReferentialCalibrationData == null)
                {
                    _waferReferentialCalibrationData = new WaferReferentialCalibrationData();
                    _waferReferentialCalibrationData.WaferConfigurations = new List<WaferConfiguration>();
                }
                return _waferReferentialCalibrationData;
            }
            set
            {
                if (_waferReferentialCalibrationData != value)
                {
                    _waferReferentialCalibrationData = value;
                    OnPropertyChanged();
                }
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
                var dialogRes = _dialogOwnerService.ShowMessageBox("The chuck Manager has changed. Do you really want to quit without saving ?", "Camera Magnification", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
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

        public override void Init()
        {
            HasChanged = false;
            _calibration = LoadCalibrationData<WaferReferentialCalibrationData>();
            if (_calibration == null)
            {
                return;
            }
            UpdateResultAccordingToSelectedWaferCategory();
        }

        public override void Save()
        {            
            _calibrationService.SaveCalibration(WaferReferentialCalibrationData);
            _calibration = LoadCalibrationData<WaferReferentialCalibrationData>();
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
                    ValidationErrorMessage = "Chuck Manager calibration is not valid.";
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
                _algoSupervisor.MultiSizeChuckChangedEvent -= MultiSizeChuckVM_MultiSizeChuckChangedEvent;
                this.PropertyChanged -= MultiSizeChuckManagerVM_PropertyChanged;
                _chuckVM.PropertyChanged -= ChuckVM_PropertyChanged;
            }
        }
    }
}
