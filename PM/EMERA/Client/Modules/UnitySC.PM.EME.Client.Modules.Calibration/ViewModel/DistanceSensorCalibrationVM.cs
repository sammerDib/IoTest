using System;
using System.ComponentModel;
using System.Windows;

using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class DistanceSensorCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly IAlgoSupervisor _algoSupervisor;
        private readonly IDialogOwnerService _dialogOwnerService;

        private DistanceSensorCalibrationResult _result;

        private AutoRelayCommand _saveCalibration;

        private AutoRelayCommand _startDistanceSensorCalibration;

        public DistanceSensorCalibrationVM(ICalibrationService calibrationService,
            IAlgoSupervisor algoSupervisor, IDialogOwnerService dialogOwnerService) : base("Distance sensor", calibrationService)
        {
            _algoSupervisor = algoSupervisor;
            _dialogOwnerService = dialogOwnerService;   
            _algoSupervisor.DistanceSensorCalibrationChangedEvent += UpdateDistanceSensorResult;
            this.PropertyChanged += DistanceSensorCalibrationVM_PropertyChanged;
            IsValidated = false;                        
        }

        private void DistanceSensorCalibrationVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasChanged))
            {
                UpdateAllCanExecutes();
            }
        }

        public AutoRelayCommand StartDistanceSensorCalibration =>
            _startDistanceSensorCalibration ?? (_startDistanceSensorCalibration = new AutoRelayCommand(
                () =>
                {
                    IsBusy = true;
                    _algoSupervisor.StartDistanceSensorCalibration(new DistanceSensorCalibrationInput());
                }));

        public DistanceSensorCalibrationResult Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
                UpdateAllCanExecutes();
            }
        }

        public AutoRelayCommand SaveCalibration =>
            _saveCalibration ?? (_saveCalibration = new AutoRelayCommand(Save, CanSave));

        public override void Init()
        {
            HasChanged = false;
            var calibrationData = LoadCalibrationData<DistanceSensorCalibrationData>();
            if (calibrationData == null)
            {
                return;
            }

            Result = new DistanceSensorCalibrationResult
            {
                OffsetX = calibrationData.OffsetX,
                OffsetY = calibrationData.OffsetX,
                Status = new FlowStatus { Message = "Loaded from previous calibration", State = FlowState.Success }
            };
            IsValidated = true;
        }

        public override void CancelChanges()
        {
        }

        public override bool CanCancelChanges()
        {
            return false;
        }

        public override void Save()
        {
            _calibrationService.SaveCalibration(new DistanceSensorCalibrationData
            {
                CreationDate = DateTime.Now, OffsetX = Result.OffsetX, OffsetY = Result.OffsetY
            });
            HasChanged = false;
            ValidateAndEnableNextPage();
        }

        public override bool CanSave()
        {
            return HasChanged && (Result?.Status.State == FlowState.Success);
        }
      

        private void UpdateDistanceSensorResult(DistanceSensorCalibrationResult result)
        {
            if (result.Status.State == FlowState.InProgress) return;
            Result = result;
            IsBusy = false;
            if (result.Status.IsFinished &&
               result.Status.State == FlowState.Success)
            {
                HasChanged = true;
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
                    }
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
                    () => Result?.Status.State == FlowState.Success
                ));
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
                        _algoSupervisor.CancelDistanceSensorCalibration();
                    }));
            }
        }
        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (HasChanged && !forceClose)
            {
                var dialogRes = _dialogOwnerService.ShowMessageBox("The distance sensor has changed. Do you really want to quit without saving ?", "Distance sensor", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
                    return true;
                }
                return false;
            }
            return true;
        }
        public override void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                _algoSupervisor.DistanceSensorCalibrationChangedEvent -= UpdateDistanceSensorResult;
                this.PropertyChanged -= DistanceSensorCalibrationVM_PropertyChanged;
            }
        }
    }
}
