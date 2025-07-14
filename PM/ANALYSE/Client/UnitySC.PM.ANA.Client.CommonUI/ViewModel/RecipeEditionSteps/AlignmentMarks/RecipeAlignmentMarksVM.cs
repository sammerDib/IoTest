using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Controls.Camera;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeAlignmentMarksVM : RecipeWizardStepBaseVM
    {
        private ANARecipeVM _editedRecipe;
        private ObjectiveConfig _objectiveToUse;

        public RecipeAlignmentMarksVM(ANARecipeVM editedRecipe)
        {
            Name = "Wafer Alignment";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;
            _editedRecipe = editedRecipe;
            LoadRecipeData();
        }

        #region Method Members

        private void LoadRecipeData()
        {
            AlignmentMarkSite1 = new AlignmentMarkSiteStepVM(this);
            AlignmentMarkSite2 = new AlignmentMarkSiteStepVM(this);

            if (!(_editedRecipe.AlignmentMarks is null))
            {
                LoadAlignmentMarksSite(_editedRecipe.AlignmentMarks.AlignmentMarksSite1, AlignmentMarkSite1);
                LoadAlignmentMarksSite(_editedRecipe.AlignmentMarks.AlignmentMarksSite2, AlignmentMarkSite2);

                if (!(_editedRecipe.AlignmentMarks?.AutoFocus is null))
                {
                    AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(_editedRecipe.AlignmentMarks?.AutoFocus);
                    AutoFocusSettings.EnableWithoutEditing();
                }
                AutoFocusSettings.AreSettingsVisible = false;

                if (_editedRecipe.AlignmentMarks?.ObjectiveContext?.ObjectiveId != null)
                {
                    _selectedObjective = CamerasSupervisor.GetObjectiveFromId(_editedRecipe.AlignmentMarks?.ObjectiveContext?.ObjectiveId);
                    ServiceLocator.CamerasSupervisor.Objective = _selectedObjective;
                    OnPropertyChanged(nameof(SelectedObjective));
                }

                IsValidated = true;
                IsModified = false;
            }
            else
            {
                if (_editedRecipe.IsAlignmentMarksSkipped)
                    IsValidated = true;
                AutoFocusSettings = null;
                IsModified = false;
            }
        }

        private void LoadAlignmentMarksSite(List<PositionWithPatternRec> siteAlignmentMarks, AlignmentMarkSiteStepVM alignementMarkSite)
        {
            if (siteAlignmentMarks is null)
            {
                return;
            }
            bool isMain = true; // The first alignmentMark is the main one
            // We load the information from the recipe
            alignementMarkSite.AlignmentMarks.Clear();
            foreach (var alignmentMark in siteAlignmentMarks)
            {
                var alignmentMarkVM = new AlignmentMarkStepVM(alignementMarkSite, alignmentMark, isMain);
                alignmentMarkVM.StepState = StepStates.Done;
                alignementMarkSite.AddAlignmentMark(alignmentMarkVM);
                isMain = false;
            }
            alignementMarkSite.StepState = StepStates.Done;
        }

        private void SaveRecipeData()
        {
            var alignmentMarksSettings = new AlignmentMarksSettings() { ObjectiveContext = new ObjectiveContext(_objectiveToUse.DeviceID) };
            alignmentMarksSettings.AlignmentMarksSite1 = new List<PositionWithPatternRec>();
            foreach (var alignmentMark in AlignmentMarkSite1.AlignmentMarks.ToList().OrderByDescending(am => am.IsMain))
            {
                alignmentMarksSettings.AlignmentMarksSite1.Add(alignmentMark.CurrentPatternRecImage);
            }
            alignmentMarksSettings.AlignmentMarksSite2 = new List<PositionWithPatternRec>();
            foreach (var alignmentMark in AlignmentMarkSite2.AlignmentMarks.ToList().OrderByDescending(am => am.IsMain))
            {
                alignmentMarksSettings.AlignmentMarksSite2.Add(alignmentMark.CurrentPatternRecImage);
            }
            if (AutoFocusSettings.IsAutoFocusEnabled)
            {
                alignmentMarksSettings.AutoFocus = AutoFocusSettings.GetAutoFocusSettings();
            }

            // Set up the camera objective.
            alignmentMarksSettings.ObjectiveContext = new TopObjectiveContext(SelectedObjective.DeviceID);

            _editedRecipe.AlignmentMarks = alignmentMarksSettings;
            _editedRecipe.IsModified = true;
            _editedRecipe.IsAlignmentMarksSkipped = false;
            IsValidated = true;
            IsModified = false;
        }

        private void AlignmentMarks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCameraPoints();
        }

        public void UpdateCameraPoints()
        {
            CameraPoints.Clear();
            foreach (var alignmentMark in AlignmentMarkSite1.AlignmentMarks)
            {
                AddCameraPoint(alignmentMark, "Alignment mark site 1");
            }

            foreach (var alignmentMark in AlignmentMarkSite2.AlignmentMarks)
            {
                AddCameraPoint(alignmentMark, "Alignment mark site 2");
            }
        }

        private void AddCameraPoint(AlignmentMarkStepVM alignmentMark, string pointName)
        {
            if (!double.IsNaN(alignmentMark.CurrentPositionX) && !double.IsNaN(alignmentMark.CurrentPositionY))
                CameraPoints.Add(new CameraDisplayPoint(pointName, new Point(alignmentMark.CurrentPositionX, alignmentMark.CurrentPositionY)));
        }

        private double CalculateMinDistanceBetweenTheSites()
        {
            var minDistance = double.MaxValue;
            foreach (var alignmentMarkSite1 in AlignmentMarkSite1.AlignmentMarks)
            {
                foreach (var alignmentMarkSite2 in AlignmentMarkSite2.AlignmentMarks)
                {
                    var distance = Math.Sqrt(Math.Pow(alignmentMarkSite2.CurrentPositionX - alignmentMarkSite1.CurrentPositionX, 2) + Math.Pow(alignmentMarkSite2.CurrentPositionY - alignmentMarkSite1.CurrentPositionY, 2));
                    minDistance = Math.Min(minDistance, distance);
                }
            }

            return minDistance;
        }

        private bool IsDistanceBeteweenTheSitesValid()
        {
            // We calculate the min distance between the two sites
            var minDistance = CalculateMinDistanceBetweenTheSites();

            if (minDistance < _editedRecipe.WaferDimentionalCharacteristic.Diameter.Millimeters / 2)
            {
                return false;
            }

            return true;
        }

        private bool IsAlignmentMarksValid()
        {
            if (AlignmentMarkSite1.AlignmentMarks.Any(am => am.StepState != UnitySC.Shared.UI.Controls.StepStates.Done) || AlignmentMarkSite2.AlignmentMarks.Any(am => am.StepState != UnitySC.Shared.UI.Controls.StepStates.Done))
                return false;

            return true;
        }

        internal bool CanEdit()
        {
            if (AlignmentMarkSite1.AlignmentMarks.Any(am => am.IsEditing) || AlignmentMarkSite2.AlignmentMarks.Any(am => am.IsEditing) || AutoFocusSettings.IsEditing)
                return false;

            return true;
        }

        private void AlgosSupervisor_AlignmentMarksChangedEvent(AlignmentMarksResult alignmentMarksResult)
        {
            if ((!alignmentMarksResult.Status.IsFinished) || (alignmentMarksResult.Status.State == FlowState.Canceled))
            {
                return;
            }

            if (alignmentMarksResult.Status.State == FlowState.Canceled)
            {
                IsTestInProgress = false;
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DisplayTestResult(alignmentMarksResult);
            }));
        }

        private void DisplayTestResult(AlignmentMarksResult alignmentMarksResult)
        {
            var alignmentMarksResultDisplayVM = new AlignmentMarksResultDisplayVM(alignmentMarksResult);

            ServiceLocator.DialogService.ShowDialog<AlignmentMarksResultDisplay>(alignmentMarksResultDisplayVM);

            IsTestInProgress = false;
        }

        private void UpdateValidationErrorMessage()
        {
            if ((!IsAlignmentMarksValid()) || (!(AlignmentMarkSite1.StepState == StepStates.Done)) || (!(AlignmentMarkSite2.StepState == StepStates.Done)))
                ValidationErrorMessage = "Alignment marks settings are not valid";
            else if (!IsDistanceBeteweenTheSitesValid())
            {
                ValidationErrorMessage = "The minimal distance between the two sites is not long enough";
            }
            else if (!IsModified)
                ValidationErrorMessage = "Wafer alignment is not modified";
            else
                ValidationErrorMessage = string.Empty;
        }

        #endregion Method Members

        #region Properties
        private AlignmentMarkSiteStepVM _alignmentMarkSite1 = null;

        public AlignmentMarkSiteStepVM AlignmentMarkSite1
        {
            get => _alignmentMarkSite1; set { if (_alignmentMarkSite1 != value) { _alignmentMarkSite1 = value; OnPropertyChanged(); } }
        }

        private AlignmentMarkSiteStepVM _alignmentMarkSite2 = null;

        public AlignmentMarkSiteStepVM AlignmentMarkSite2
        {
            get => _alignmentMarkSite2; set { if (_alignmentMarkSite2 != value) { _alignmentMarkSite2 = value; OnPropertyChanged(); } }
        }


        private Size _roiSize = Size.Empty;

        public Size RoiSize
        {
            get
            {
                if (_roiSize == Size.Empty)
                    _roiSize = new Size(ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                return _roiSize;
            }
            set { if (_roiSize != value) { _roiSize = value; OnPropertyChanged(); } }
        }

        private Rect _roiRectAlignmentMarks = Rect.Empty;

        public Rect RoiRectAlignmentMarks
        {
            get
            {
                if (_roiRectAlignmentMarks == Rect.Empty)
                    _roiRectAlignmentMarks = new Rect(0, 0, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                return _roiRectAlignmentMarks;
            }

            set { if (_roiRectAlignmentMarks != value) { _roiRectAlignmentMarks = value; OnPropertyChanged(); } }
        }

        private bool _isCenteredROI = true;

        public bool IsCenteredROI
        {
            get { return _isCenteredROI; }
            set { if (_isCenteredROI != value) { _isCenteredROI = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ICameraDisplayPoint> _cameraPoints = null;

        public ObservableCollection<ICameraDisplayPoint> CameraPoints
        {
            get
            {
                if (_cameraPoints is null)
                    _cameraPoints = new ObservableCollection<ICameraDisplayPoint>();
                return _cameraPoints;
            }

            set { if (_cameraPoints != value) { _cameraPoints = value; OnPropertyChanged(); } }
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); OnPropertyChanged(nameof(RequiresValidation)); } }
        }

        public bool RequiresValidation => !IsValidated || IsModified || !IsReadyToValidate;



        private bool _isTestInProgress = false;

        public bool IsTestInProgress
        {
            get => _isTestInProgress; set { if (_isTestInProgress != value) { _isTestInProgress = value; OnPropertyChanged(); } }
        }

        private bool IsReadyToValidate => IsAlignmentMarksValid() && AlignmentMarkSite1.StepState == StepStates.Done && AlignmentMarkSite2.StepState == StepStates.Done && (!IsTestInProgress) && IsDistanceBeteweenTheSitesValid() && !AutoFocusSettings.IsEditing;

        private AutoFocusSettingsVM _autoFocusSettings;

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get
            {
                if (_autoFocusSettings is null)
                {
                    _autoFocusSettings = new AutoFocusSettingsVM();
                    _autoFocusSettings.AutoFocusSettingsModified += AutoFocusSettings_Modified;
                }
                return _autoFocusSettings;
            }
            set
            {
                if (_autoFocusSettings != value)
                {
                    if (!(_autoFocusSettings is null))
                    {
                        _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                    }
                    _autoFocusSettings = value;
                    if (!(_autoFocusSettings is null))
                    {
                        _autoFocusSettings.AutoFocusSettingsModified += AutoFocusSettings_Modified;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private void AutoFocusSettings_Modified(object sender, EventArgs e)
        {
            IsModified = true;
        }

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private ObservableCollection<ObjectiveConfig> _objectives;

        public ObservableCollection<ObjectiveConfig> Objectives
        {
            get => _objectives; set { if (_objectives != value) { _objectives = value; OnPropertyChanged(); } }
        }

        private ObjectiveConfig _selectedObjective;

        public ObjectiveConfig SelectedObjective
        {
            get => _selectedObjective;

            set
            {
                if (_selectedObjective != value)
                {
                    _selectedObjective = value;
                    IsModified = true;
                    OnPropertyChanged();
                    if (_selectedObjective != null)
                        ServiceLocator.CamerasSupervisor.Objective = _selectedObjective;
                    AlignmentMarkSite1.RemoveAllAlignmentMarks();
                    AlignmentMarkSite2.RemoveAllAlignmentMarks();
                }
            }
        }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions> { SpecificPositions.PositionWaferCenter };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionWaferCenter;
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _skipAlignmentMarks;

        public AutoRelayCommand SkipAlignmentMarks
        {
            get
            {
                return _skipAlignmentMarks ?? (_skipAlignmentMarks = new AutoRelayCommand(
                    () =>
                    {
                        _editedRecipe.IsAlignmentMarksSkipped = true;
                        _editedRecipe.AlignmentMarks = null;
                        IsValidated = true;
                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _validateAlignmentMarks;

        public AutoRelayCommand ValidateAlignmentMarks
        {
            get
            {
                return _validateAlignmentMarks ?? (_validateAlignmentMarks = new AutoRelayCommand(
                    () =>
                    {
                        if (IsModified)
                        {
                            // We calculate the min distance between the two sites
                            var minDistance = CalculateMinDistanceBetweenTheSites();

                            if (minDistance < _editedRecipe.WaferDimentionalCharacteristic.Diameter.Millimeters / 2)
                            {
                                var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                                dialogService.ShowMessageBox("The distance between the two sites is not long enough", "Error", MessageBoxButton.OK);
                                return;
                            }

                            SaveRecipeData();
                        }
                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () =>
                    {
                        UpdateValidationErrorMessage();
                        return IsReadyToValidate;
                    }
                ));
            }
        }

        private AutoRelayCommand _cancelAlignmentMarks;

        public AutoRelayCommand CancelAlignmentMarks
        {
            get
            {
                return _cancelAlignmentMarks ?? (_cancelAlignmentMarks = new AutoRelayCommand(
                    () =>
                    {
                        LoadRecipeData();
                    },
                    () => { return !(_editedRecipe.AlignmentMarks is null) && IsModified; }
                ));
            }
        }

        public bool CanCancelAlignmentMarks
        {
            get => (!(_editedRecipe.AlignmentMarks is null));
        }

        private AutoRelayCommand _startAlignmentMarksTest;

        public AutoRelayCommand StartAlignmentMarksTest
        {
            get
            {
                return _startAlignmentMarksTest ?? (_startAlignmentMarksTest = new AutoRelayCommand(
                    () =>
                    {
                        var site1PatternRecs = AlignmentMarkSite1.AlignmentMarks.Select(s => s.CurrentPatternRecImage).ToList();
                        var site2PatternRecs = AlignmentMarkSite2.AlignmentMarks.Select(s => s.CurrentPatternRecImage).ToList();

                        AlignmentMarksInput alignmentMarksInput = new AlignmentMarksInput(site1PatternRecs, site2PatternRecs, AutoFocusSettings.GetAutoFocusSettings());
                        ServiceLocator.AlgosSupervisor.StartAlignmentMarks(alignmentMarksInput);
                        IsTestInProgress = true;
                    },
                    () => { return IsAlignmentMarksValid() && AlignmentMarkSite1.StepState == StepStates.Done && AlignmentMarkSite2.StepState == StepStates.Done && !AutoFocusSettings.IsEditing; }
                ));
            }
        }

        private AutoRelayCommand _cancelAlignmentMarksTest;

        public AutoRelayCommand CancelAlignmentMarksTest
        {
            get
            {
                return _cancelAlignmentMarksTest ?? (_cancelAlignmentMarksTest = new AutoRelayCommand(
                    () =>
                    {
                        ServiceLocator.AlgosSupervisor.CancelAlignmentMarks();
                        IsTestInProgress = false;
                    },
                    () => { return IsTestInProgress; }
                ));
            }
        }

        #endregion RelayCommands

        #region INavigable

        public override async Task PrepareToDisplay()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                // We select the main objective
                _objectiveToUse = ClassLocator.Default.GetInstance<CamerasSupervisor>().MainObjective;
                ClassLocator.Default.GetInstance<CamerasSupervisor>().Objective = _objectiveToUse;
                ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming();

                // Enable Lights
                ServiceLocator.LightsSupervisor.LightsAreLocked = false;

                // Set lights intensity
                if (!(_editedRecipe.Alignment is null))
                {
                    foreach (var light in ServiceLocator.LightsSupervisor.Lights)
                    {
                        ServiceLocator.LightsSupervisor.SetLightIntensity(light.DeviceID, 0);
                    }

                    ServiceLocator.LightsSupervisor.SetLightIntensity(ServiceLocator.LightsSupervisor.GetMainLight().DeviceID, _editedRecipe.Alignment.AutoLight.LightIntensity);
                }

                Objectives = new ObservableCollection<ObjectiveConfig>();
                foreach (var objective in ServiceLocator.CamerasSupervisor.Objectives)
                {
                    if ((objective.ObjType == ObjectiveConfig.ObjectiveType.NIR) || (objective.ObjType == ObjectiveConfig.ObjectiveType.VIS))
                        Objectives.Add(objective);
                }

                if (_editedRecipe.AlignmentMarks?.ObjectiveContext?.ObjectiveId != null)
                {
                    _selectedObjective = CamerasSupervisor.GetObjectiveFromId(_editedRecipe.AlignmentMarks?.ObjectiveContext?.ObjectiveId);
                }
                else
                {
                    _selectedObjective = Objectives.FirstOrDefault();
                }
                ServiceLocator.CamerasSupervisor.Objective = _selectedObjective;
                OnPropertyChanged(nameof(SelectedObjective));

                AlignmentMarkSite1.AlignmentMarks.CollectionChanged += AlignmentMarks_CollectionChanged;
                AlignmentMarkSite2.AlignmentMarks.CollectionChanged += AlignmentMarks_CollectionChanged;

                ServiceLocator.AlgosSupervisor.AlignmentMarksChangedEvent += AlgosSupervisor_AlignmentMarksChangedEvent;

                OnPropertyChanged(nameof(RoiRectAlignmentMarks));
                IsModified = false;
                IsBusy = false;
            });
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (IsReadyToValidate && IsModified && !forceClose)
            {
                var result = ServiceLocator.DialogService.ShowMessageBox("The wafer alignment has not been validated. Do you really want to leave ?", "AlignmentMarks", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }

            // We restore the last validated state
            LoadRecipeData();

            AlignmentMarkSite1.AlignmentMarks.CollectionChanged -= AlignmentMarks_CollectionChanged;
            AlignmentMarkSite2.AlignmentMarks.CollectionChanged -= AlignmentMarks_CollectionChanged;
            ServiceLocator.AlgosSupervisor.AlignmentMarksChangedEvent -= AlgosSupervisor_AlignmentMarksChangedEvent;

            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();

            IsModified = false;
            return true;
        }

        #endregion INavigable
    }
}
