using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.LiseHF
{
    public class LiseHFCalibrationVM : CalibrationVMBase
    {
        #region Fields

        private ILogger _logger;
        private CalibrationSupervisor _calibrationSupervisor;
        private CamerasSupervisor _camerasSupervisor;
        private ProbesSupervisor _probesSupervisor;
        private ProbeLiseHFVM _probeLiseHF;

        private LiseHFCalibrationData _liseHFCalibration;

        #endregion Fields

        #region Constructor

        public LiseHFCalibrationVM() : base("Lise HF")
        {
            _probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            _logger=ClassLocator.Default.GetInstance<ILogger>();
            _probeLiseHF = _probesSupervisor.Probes.FirstOrDefault(p => p is ProbeLiseHFVM) as ProbeLiseHFVM;

            if (_probeLiseHF == null)
                IsEnabled = false;

            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            _camerasSupervisor.ObjectiveChangedEvent += CamerasSupervisor_ObjectiveChangedEvent;
        }
        #endregion Constructor

        #region Methods


        private void CamerasSupervisor_ObjectiveChangedEvent(string objectiveID)
        {
        }

        public override void Dispose()
        {
            _camerasSupervisor.ObjectiveChangedEvent -= CamerasSupervisor_ObjectiveChangedEvent;

        }

        private void PrepareHardware()
        {
        }

        private void GoToCorrectedPosition()
        {
        }

        private void Update()
        {
            IsValidated = true;
            HasChanged = true;
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        public override void Save()
        {
            SpotCalibration.UpdateLiseHFCalibration();
            RefCalibration.UpdateLiseHFCalibration();
            _liseHFCalibration.User = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser?.Name;
            _calibrationSupervisor.SaveCalibration(_liseHFCalibration);
            HasChanged = false;
            SpotCalibration.IsModified = false;
            RefCalibration.IsModified = false;
        }

        public override void Init()
        {
            InitLiseHFCalibration();
            UpdateCalibrationState();
        }


        public override void UpdateCalibration(ICalibrationData calibrationData)
        {
            throw new NotImplementedException();
        }

        public override void CancelChanges()
        {
            InitLiseHFCalibration();
            HasChanged = false;
            SpotCalibration.IsModified = false;
            RefCalibration.IsModified = false;

        }

        public override bool CanCancelChanges()
        {
            return HasChanged && !RefCalibration.IsCalibrationInProgress && !SpotCalibration.IsCalibrationInProgress;
        }

        public override bool CanSave()
        {
            return HasChanged && !RefCalibration.IsCalibrationInProgress && !SpotCalibration.IsCalibrationInProgress;
        }

        #endregion Methods

        #region Properties

        private LiseHFSpotsCalibrationVM _spotCalibration;

        private LiseHFRefCalibrationVM _refCalibration;

        public LiseHFSpotsCalibrationVM SpotCalibration => _spotCalibration ?? (_spotCalibration = new LiseHFSpotsCalibrationVM(_probeLiseHF?.DeviceID));
        public LiseHFRefCalibrationVM RefCalibration => _refCalibration ?? (_refCalibration = new LiseHFRefCalibrationVM(_probeLiseHF?.DeviceID));

        private void SpotCalibration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsModified") && (SpotCalibration.IsModified))
            {
                HasChanged=true;
                SaveCommand.NotifyCanExecuteChanged();
                CancelCommand.NotifyCanExecuteChanged();
            }
        }


        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            { 
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

        private void RefCalibration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsModified") && (RefCalibration.IsModified))
            {
                HasChanged=true;
                SaveCommand.NotifyCanExecuteChanged();
                CancelCommand.NotifyCanExecuteChanged();
            }
        }

        #endregion Properties

        #region Commands

        private AutoRelayCommand _saveCommand;

        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            Save();
                            UpdateCalibrationState();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message);
                            var dialogRes = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Error during save", "Calibration save", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        finally
                        {
                            _calibrationSupervisor.UpdateCalibrationCache();
                        }
                    },() => { return CanSave(); }));
            }
        }

        private void UpdateCalibrationState()
        {
            IsValidated = SpotCalibration.IsCalibrationComplete() && RefCalibration.IsCalibrationComplete();
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Calibration has changed. Do you really want to undo change", "Undo calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                  if (res == MessageBoxResult.Yes)
                  {
                      CancelChanges();
                  }
              },
              () => { return CanCancelChanges(); }));
            }
        }

        #endregion Commands

        #region INavigable implementation

        public override Task PrepareToDisplay()
        {
            _camerasSupervisor.Camera.StartStreaming();

            // Disable the offset correction for the probe spot position
            ServiceLocator.ReferentialSupervisor.SetSettings(new StageReferentialSettings() { EnableProbeSpotOffset = false });

            InitLiseHFCalibration();
            UpdateCalibrationState();
            _spotCalibration.PropertyChanged += SpotCalibration_PropertyChanged;
            _refCalibration.PropertyChanged += RefCalibration_PropertyChanged;
            return Task.CompletedTask;
        }

        private void LeaveDisplay()
        {
            _camerasSupervisor.Camera.StopStreaming();
            _spotCalibration.PropertyChanged -= SpotCalibration_PropertyChanged;
            _refCalibration.PropertyChanged -= RefCalibration_PropertyChanged;

            // Re-enable the offset correction for the probe spot position
            ServiceLocator.ReferentialSupervisor.SetSettings(new StageReferentialSettings() { EnableProbeSpotOffset = false });

            HasChanged = false;
            //Calibration are considered as not modified
            SpotCalibration.IsModified = false;
            RefCalibration.IsModified = false;

        }

        private void InitLiseHFCalibration()
        {
            _liseHFCalibration = _calibrationSupervisor.GetCalibrations()?.Result.OfType<LiseHFCalibrationData>().FirstOrDefault();
            _liseHFCalibration = _liseHFCalibration ?? new LiseHFCalibrationData();
            SpotCalibration.LiseHFCalibration = _liseHFCalibration;
            RefCalibration.LiseHFCalibration = _liseHFCalibration;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (SpotCalibration.IsCalibrationInProgress || RefCalibration.IsCalibrationInProgress)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("You cannot leave this page while calibration is in progress.", "LIseHF calibration", MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
            if (HasChanged && !forceClose)
            {
                var dialogRes = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The LiseHF calibration has changed. Do you really want to quit without saving ?", "LIseHF calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    LeaveDisplay();
                    return true;
                }
                return false;
            }

            LeaveDisplay();
            return true;
        }

        #endregion INavigable implementation
    }
}
