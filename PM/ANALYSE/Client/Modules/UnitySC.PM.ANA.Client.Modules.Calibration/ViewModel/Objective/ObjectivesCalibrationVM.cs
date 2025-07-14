using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class ObjectivesCalibrationVM : CalibrationVMBase
    {
        #region Fields
        private CalibrationSupervisor _calibrationSupervisor;
        private CamerasSupervisor _camerasSupervisor;
        #endregion Fields

        #region Constructor
        public ObjectivesCalibrationVM() : base("Objectives")
        {
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            InitObjectiveCalibrations(null);
        }

        #endregion Constructor

        #region Properties
        private List<ObjectiveToCalibrateVM> _objectives;

        public List<ObjectiveToCalibrateVM> Objectives
        {
            get => _objectives; set { if (_objectives != value) { _objectives = value; OnPropertyChanged(); } }
        }

        private ObjectiveToCalibrateVM _selectedObjective;

        public ObjectiveToCalibrateVM SelectedObjective
        {
            get => _selectedObjective;
            set
            {
                if (_selectedObjective != value)
                {
                    _selectedObjective = value;

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateObjectiveAndCamera();
                    }));

                    OnPropertyChanged();
                }
            }
        }

        private bool _isProbeLiseAcquiring = false;

        public bool IsProbeLiseAcquiring
        {
            get => _isProbeLiseAcquiring; set { if (_isProbeLiseAcquiring != value) { _isProbeLiseAcquiring = value; OnPropertyChanged(); } }
        }


        private AutoFocusSettingsVM _autoFocusSettings;

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get
            {
                return _autoFocusSettings ?? (_autoFocusSettings = new AutoFocusSettingsVM()
                {
                    IsAutoFocusEnabled = true,
                    Type = AutoFocusType.Camera,
                    CameraScanRange = Service.Interface.Algo.ScanRangeType.Large
                });
            }
            set
            {
                if (_autoFocusSettings != value)
                {
                    _autoFocusSettings = value;
                }
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

        #endregion Properties

        #region Methods
        private void CamerasSupervisor_ObjectiveChangedEvent(string objectiveID)
        {
            if (objectiveID != null)
                SelectedObjective = Objectives.FirstOrDefault(x => x.Id == objectiveID);
        }

        public override void Dispose()
        {
            _camerasSupervisor.ObjectiveChangedEvent -= CamerasSupervisor_ObjectiveChangedEvent;
            AutoFocusSettings.Dispose();
            foreach (var objective in Objectives)
            {
                objective.ResultChangedEvent -= FocusPositionStep_ResultChangedEvent;
                objective.CentricitiesRefPosChangedEvent -= CentricitiesRefPos_ChangedEvent;
                objective.PropertyChanged -= Objective_PropertyChanged;
                objective.Dispose();
            }
        }

        private void Objective_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValidated")
            {
                Update();
            }
        }

        private void UpdateObjectiveAndCamera()
        {
            if (_selectedObjective == null)
                return;

            PrepareHardware();

            UpdateCentricityRefPos();
            UpdateAutofocusSettings();
            GoToCorrectedPosition();
        }

        private void PrepareHardware()
        {
            _camerasSupervisor.Camera = _camerasSupervisor.Cameras.FirstOrDefault(x => x.Configuration.ModulePosition == _selectedObjective.Position);
            _camerasSupervisor.Camera.StartStreaming();

            // Set Objective, will automatically apply Z, X and Y objective offset
            if (_camerasSupervisor.Camera != null)
                _camerasSupervisor.Objective = _camerasSupervisor.Objectives.FirstOrDefault(x => x.DeviceID == _selectedObjective.Id);

            var probeSupervisor = ServiceLocator.ProbesSupervisor;
            if (_selectedObjective.Position == ModulePositions.Up)
                probeSupervisor.SetCurrentProbe(probeSupervisor.ProbeLiseUp.DeviceID);
            else
                probeSupervisor.SetCurrentProbe(probeSupervisor.ProbeLiseDown.DeviceID);
        }

        private void UpdateAutofocusSettings()
        {
            AutoFocusSettings.CameraObjective = _camerasSupervisor.Objective;
        }

        private void UpdateCentricityRefPos()
        {
            var mainObjective = Objectives.Find(_ => _.IsMain);
            _selectedObjective.CentricityStep.RefPos = mainObjective.CentricityStep.RefPos;
        }

        private void GoToCorrectedPosition()
        {
            if (_selectedObjective?.CentricityStep?.RefPos != null)
            {
                var correctedPosition = (XYPosition)_selectedObjective.CentricityStep.RefPos.Clone();
                correctedPosition.X += _selectedObjective?.Image?.XOffset?.Value ?? 0;
                correctedPosition.Y += _selectedObjective?.Image?.YOffset?.Value ?? 0;

                Task.Run(() =>
                {
                    ClassLocator.Default.GetInstance<AxesSupervisor>().WaitMotionEnd(20000);
                    ClassLocator.Default.GetInstance<AxesSupervisor>().GotoPosition(correctedPosition, AxisSpeed.Normal);
                });
            }
        }

        private void InitObjectiveCalibrations(ObjectivesCalibrationData calibrations)
        {
            if (Objectives != null)
            {
                foreach (var objective in Objectives)
                {
                    objective.Dispose();
                }
            }
            var objectivesToCalibrate = _calibrationSupervisor.GetObjectivesToCalibrate()?.Result;
            if (objectivesToCalibrate != null)
            {
                var mainObjectiveId = objectivesToCalibrate.Find(_ => _.IsMain).DeviceId;
                if (mainObjectiveId != null)
                {
                    var mainObjectiveCalib = calibrations?.Calibrations.Find(_ => _.DeviceId == mainObjectiveId);

                    Objectives = new List<ObjectiveToCalibrateVM>();
                    foreach (var toCalibrate in objectivesToCalibrate.OrderBy(x => x.Position).ToList())
                    {
                        ObjectiveCalibration calibrationData = calibrations?.Calibrations.SingleOrDefault(x => x.DeviceId == toCalibrate.DeviceId);
                        if (calibrationData is null)
                            calibrationData = new ObjectiveCalibration() { ObjectiveName = toCalibrate.ObjectiveName, DeviceId = toCalibrate.DeviceId, Magnification = toCalibrate.Magnification };

                        var objectiveVM = new ObjectiveToCalibrateVM(calibrationData, toCalibrate.Position, toCalibrate.IsMain, toCalibrate.ObjType);
                        Objectives.Add(objectiveVM);

                        if (mainObjectiveCalib?.OpticalReferenceElevationFromStandardWafer != null)
                        {
                            objectiveVM.OpticalReferenceElevationFromStandardWafer = mainObjectiveCalib.OpticalReferenceElevationFromStandardWafer;
                        }

                        if (mainObjectiveCalib?.Image?.CentricitiesRefPosition != null)
                        {
                            objectiveVM.CentricityStep.RefPos = mainObjectiveCalib.Image.CentricitiesRefPosition;
                        }

                        objectiveVM.ResultChangedEvent += FocusPositionStep_ResultChangedEvent;
                        objectiveVM.CentricitiesRefPosChangedEvent += CentricitiesRefPos_ChangedEvent;
                        objectiveVM.PropertyChanged += Objective_PropertyChanged;
                    }
                }
            }

            IsValidated = Objectives?.All(x => x.IsValidated) ?? false;

            HasChanged = false;
        }

        private void FocusPositionStep_ResultChangedEvent(ObjectiveToCalibrateVM objective)
        {
            if (objective.FlowResult != null && objective.FlowResult.Status.State == FlowState.Success)
            {
                if (objective.IsMain)
                {
                    var opticalRef = objective.FlowResult.OpticalReferenceElevationFromStandardWafer;
                    foreach (var obj in Objectives)
                    {
                        if (obj.Position == objective.Position)
                        {
                            obj.OpticalReferenceElevationFromStandardWafer = opticalRef;
                        }
                    }
                }
                if (objective.PixelSizeStep.StepState != StepStates.Done)
                {
                    objective.PixelSizeStep.IsEditing = true;
                }
                Update();
            }
        }

        private void CentricitiesRefPos_ChangedEvent(ObjectiveToCalibrateVM objective)
        {
            if (objective.Image.CentricitiesRefPosition != objective.CentricityStep.RefPos)
            {
                bool isMainObjectiveRefPosDone = Objectives.Exists(obj => !obj.IsMain && obj.CentricityStep.StepState == StepStates.Done);
                if (isMainObjectiveRefPosDone)
                {
                    var dialogRes = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Reset initial position will erase all objectives centricities saved.\nDo you really want to reset it ?", "Objectives calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (dialogRes == MessageBoxResult.Yes)
                    {
                        foreach (var obj in Objectives)
                        {
                            if (obj.Image?.XOffset?.Length != null && obj.Image?.YOffset?.Length != null)
                            {
                                obj.Image.XOffset.Length = 0.Millimeters();
                                obj.Image.YOffset.Length = 0.Millimeters();
                                obj.CentricityStep.StepState = StepStates.NotDone;
                            }
                        }
                    }
                    else
                    {
                        objective.CentricityStep.RefPos = objective.Image.CentricitiesRefPosition;
                    }
                }
            }
        }

        private void Update()
        {
            UpdateZOffset(ModulePositions.Up);
            UpdateZOffset(ModulePositions.Down);
            IsValidated = Objectives.All(x => x.IsValidated);
            HasChanged = true;
        }

        private void UpdateZOffset(ModulePositions modulePosition)
        {
            var mainObjective = Objectives.FirstOrDefault(x => x.Position == modulePosition && x.IsMain && x.IsValidated);
            foreach (var objective in Objectives.Where(x => x.Position == modulePosition))
            {
                if (mainObjective?.Autofocus != null && objective?.Autofocus != null)
                {
                    objective.ZOffsetWithMainObjective = mainObjective.Autofocus.ZFocusPosition.Length - objective.Autofocus.ZFocusPosition.Length;
                }
                else
                {
                    objective.ZOffsetWithMainObjective = 0.Millimeters();
                }
            }
        }

        public override void UpdateCalibration(ICalibrationData calibrationData)
        {
            InitObjectiveCalibrations(calibrationData as ObjectivesCalibrationData);
        }

        public override void Save()
        {
            Objectives.ForEach(x => x.VMToCalibrationData());
            var calibration = new ObjectivesCalibrationData();
            calibration.User = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser?.Name;

            foreach (var calibVM in Objectives)
            {
                // Update model
                calibVM.VMToCalibrationData();
                calibration.Calibrations.Add(calibVM.CalibrationData);
            }

            _calibrationSupervisor.SaveCalibration(calibration);
            HasChanged = false;
        }

        public override void Init()
        {
            var objectivesCalibration = _calibrationSupervisor.GetCalibrations()?.Result.FirstOrDefault(c => c is ObjectivesCalibrationData);
            if (!(objectivesCalibration is null))
            {
                UpdateCalibration(objectivesCalibration);
            }
        }

        public override void CancelChanges()
        {
            Init();
        }

        public override bool CanCancelChanges()
        {
            return HasChanged;
        }

        public override bool CanSave()
        {
            return HasChanged && !SelectedObjective.IsEditing;
        }
        #endregion Methods

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
                    }
                    catch (Exception ex)
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during save");
                    }
                    finally
                    {
                        _calibrationSupervisor.UpdateCalibrationCache();
                    }
              },
              () => { return CanSave(); }));
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

                    var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Calibration has changed. Do you really want to undo change", "Undo calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                    if (res == MessageBoxResult.Yes)
                    {
                        CancelChanges();
                    }
                },
                () => { return (HasChanged) && (CanCancelChanges()); }));
            }
        }

        #endregion Commands

        #region INavigable implementation

        public override Task PrepareToDisplay()
        {
            // Disable referentials converter
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Motor);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(ReferentialTag.Motor, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(ReferentialTag.Wafer, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Wafer);

            _camerasSupervisor.ObjectiveChangedEvent += CamerasSupervisor_ObjectiveChangedEvent;

            IsProbeLiseAcquiring = true;
            return Task.CompletedTask;
        }
        
        private void LeaveDisplay()
        {
            // Re-enable referentials converter
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Motor);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Motor, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Wafer, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Wafer);
            _camerasSupervisor.ObjectiveChangedEvent -= CamerasSupervisor_ObjectiveChangedEvent;
            HasChanged = false;
            IsProbeLiseAcquiring = false;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (AutoFocusSettings.IsTestInProgress)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("You cannot leave this page while autofocus is in progress.", "AutoFocus", MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
            if (HasChanged && !forceClose)
            {
                var dialogRes = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The objectives calibration has changed. Do you really want to quit without saving ?", "Objectives calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
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
