using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Controls.Camera;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.WaferMap;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class CornerPoint : CameraDisplayPoint
    {
        public CornerPoint(string name, System.Windows.Point position, CornerPosition cornerPosition) : base(name, position)
        {
            CornerPosition = cornerPosition;
        }

        public CornerPosition CornerPosition { get; set; }
    }

    public class RecipeWaferMapVM : RecipeWizardStepBaseVM
    {
        private ANARecipeVM _editedRecipe;
        private ObjectiveConfig _objectiveToUse;

        public Dictionary<string, LengthUnit> UnitsForDieSize { get; set; }

        public RecipeWaferMapVM(ANARecipeVM editedRecipe)
        {
            Name = "Wafer Map";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;
            _editedRecipe = editedRecipe;
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            LoadRecipeData();

            UnitsForDieSize = new Dictionary<string, LengthUnit>
            {
                { Length.GetUnitSymbol(LengthUnit.Millimeter), LengthUnit.Millimeter },
                { Length.GetUnitSymbol(LengthUnit.Micrometer), LengthUnit.Micrometer }
            };
            SetPreviousParameters();
        }

        // TODO Validate the value to use
        private const double DieSizeTolerance = 0.1;  // in mm

        #region Properties
        private bool _isWaferMapInProgress = false;

        public bool IsWaferMapInProgress
        {
            get => _isWaferMapInProgress;
            set => SetProperty(ref _isWaferMapInProgress, value);
        }

        private bool _isDieSizeSet = false;
        private bool _previousIsDieSizeSet = false;

        public bool IsDieSizeSet
        {
            get => _isDieSizeSet;
            set { if (_isDieSizeSet != value) { _isDieSizeSet = value; OnPropertyChanged(); } }
        }

        private LengthVM _dieWidth = new LengthVM(10, LengthUnit.Millimeter);
        private Length _previousDieWidth;

        public LengthVM DieWidth
        {
            get => _dieWidth; set { if (_dieWidth != value) { _dieWidth = value; OnPropertyChanged(); } }
        }

        private LengthVM _dieHeight = new LengthVM(10, LengthUnit.Millimeter);
        private Length _previousDieHeight;

        public LengthVM DieHeight
        {
            get => _dieHeight; set { if (_dieHeight != value) { _dieHeight = value; OnPropertyChanged(); } }
        }

        private LengthVM _edgeExclusion = new LengthVM(0, LengthUnit.Millimeter);
        private Length _previousEdgeExclusion;

        public LengthVM EdgeExclusion
        {
            get => _edgeExclusion; set { if (_edgeExclusion != value) { _edgeExclusion = value; OnPropertyChanged(); } }
        }

        private CornerStepVM _topLeftConerStep = null;

        public CornerStepVM TopLeftCornerStep
        {
            get
            {
                if (_topLeftConerStep is null)
                {
                    _topLeftConerStep = new CornerStepVM(CornerPosition.TopLeft, this);
                }
                return _topLeftConerStep;
            }

            set { if (_topLeftConerStep != value) { _topLeftConerStep = value; OnPropertyChanged(); } }
        }

        private CornerStepVM _bottomRightConerStep = null;

        public CornerStepVM BottomRightCornerStep
        {
            get
            {
                if (_bottomRightConerStep is null)
                {
                    _bottomRightConerStep = new CornerStepVM(CornerPosition.BottomRight, this);
                }
                return _bottomRightConerStep;
            }

            set { if (_bottomRightConerStep != value) { _bottomRightConerStep = value; OnPropertyChanged(); } }
        }

        private StepBaseVM _dieAndStreetSizesStep = null;

        public StepBaseVM DieAndStreetSizesStep
        {
            get
            {
                if (_dieAndStreetSizesStep is null)
                    _dieAndStreetSizesStep = new StepBaseVM();
                return _dieAndStreetSizesStep;
            }

            set { if (_dieAndStreetSizesStep != value) { _dieAndStreetSizesStep = value; OnPropertyChanged(); } }
        }

        private DieAndStreetSizesResult _dieAndStreetSizes = null;

        public DieAndStreetSizesResult DieAndStreetSizes
        {
            get => _dieAndStreetSizes; set { if (_dieAndStreetSizes != value) { _dieAndStreetSizes = value; OnPropertyChanged(); } }
        }

        private WaferMapResult _waferMap = null;

        public WaferMapResult WaferMap
        {
            get => _waferMap;
            set
            {
                if (_waferMap != value)
                {
                    _waferMap = value;
                    NbDies = 0;
                    if (!(_waferMap is null))
                    {
                        // We count the number of dies
                        int nbDies = 0;

                        Parallel.For(0, _waferMap.NbColumns, column =>
                        {
                            for (int row = 0; row < _waferMap.NbRows; row++)
                                if (_waferMap.DiesPresence.GetValue(row, column))
                                    Interlocked.Increment(ref nbDies);
                        });

                        NbDies = nbDies;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private int _nbDies = 0;

        public int NbDies
        {
            get => _nbDies; set { if (_nbDies != value) { _nbDies = value; OnPropertyChanged(); } }
        }

        private StepBaseVM _waferMapStep = null;

        public StepBaseVM WaferMapStep
        {
            get
            {
                if (_waferMapStep is null)
                    _waferMapStep = new StepBaseVM();
                return _waferMapStep;
            }

            set { if (_waferMapStep != value) { _waferMapStep = value; OnPropertyChanged(); } }
        }

        private BitmapSource _diesImage = null;

        public BitmapSource DiesImage
        {
            get
            {
                return _diesImage;
            }

            set
            {
                if (_diesImage == value)
                {
                    return;
                }

                _diesImage = value;
                OnPropertyChanged();
            }
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

        private Rect _roiRectWaferMap = Rect.Empty;

        public Rect RoiRectWaferMap
        {
            get
            {
                if (_roiRectWaferMap == Rect.Empty)
                    _roiRectWaferMap = new Rect(0, 0, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                return _roiRectWaferMap;
            }

            set { if (_roiRectWaferMap != value) { _roiRectWaferMap = value; OnPropertyChanged(); } }
        }

        private bool _isCenteredROI = false;

        public bool IsCenteredROI
        {
            get { return _isCenteredROI; }
            set { if (_isCenteredROI != value) { _isCenteredROI = value; OnPropertyChanged(); } }
        }

        public StepStates GlobalWaferMapState
        {
            get
            {
                if ((TopLeftCornerStep.StepState == StepStates.Error) || (BottomRightCornerStep.StepState == StepStates.Error) || (DieAndStreetSizesStep.StepState == StepStates.Error) || (WaferMapStep.StepState == StepStates.Error))
                {
                    return StepStates.Error;
                }
                if ((TopLeftCornerStep.StepState == StepStates.InProgress) || (BottomRightCornerStep.StepState == StepStates.InProgress) || (DieAndStreetSizesStep.StepState == StepStates.InProgress) || (WaferMapStep.StepState == StepStates.InProgress))
                {
                    return StepStates.InProgress;
                }
                if ((TopLeftCornerStep.StepState == StepStates.Done) && (BottomRightCornerStep.StepState == StepStates.Done) && (DieAndStreetSizesStep.StepState == StepStates.Done) && (WaferMapStep.StepState == StepStates.Done))
                {
                    return StepStates.Done;
                }
                return StepStates.NotDone;
            }
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    if (!_isModified)
                    {
                        AutoFocusSettings.IsModified = false;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(RequiresValidation));
                }
            }
        }

        private bool IsReadyToValidate => (DieAndStreetSizesStep.StepState == StepStates.Done) && (WaferMapStep.StepState == StepStates.Done) && !AutoFocusSettings.IsModified;

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

        public void ResetWaferMapAndStreetSizeSteps()
        {
            WaferMapStep.StepState = StepStates.NotDone;
            DieAndStreetSizesStep.StepState = StepStates.NotDone;
        }

        private void AutoFocusSettings_Modified(object sender, EventArgs e)
        {
            IsModified = true;
            ResetWaferMapAndStreetSizeSteps();
        }

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
        }

        public bool RequiresValidation => !IsValidated || IsModified || !IsReadyToValidate;


        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
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

        private AutoRelayCommand _skipWaferMap;

        public AutoRelayCommand SkipWaferMap
        {
            get
            {
                return _skipWaferMap ?? (_skipWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        if (!(_editedRecipe.WaferMap is null))
                        {
                            if (ServiceLocator.DialogService.ShowMessageBox("All the measure positions will be removed. Are you sure you want to remove the existing wafer map ?", "Wafer Map", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                return;

                            RemoveAllTheMeasurePositions();
                        }
                        _editedRecipe.IsWaferMapSkipped = true;
                        _editedRecipe.WaferMap = null;
                        WaferMap = null;
                        ServiceLocator.AxesSupervisor.AxesVM.WaferMap = null;
                        IsValidated = true;
                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }

        // Removes all the measure positions except for the measures that use always wafer positions like the bow or the warp
        private void RemoveAllTheMeasurePositions()
        {
            var existingMeasurePages = ServiceLocator.NavigationManager.AllPages.Where(p => (p as IWizardNavigationItem).IsMeasure);
            foreach (var measurePage in existingMeasurePages.ToList())
            {
                if (measurePage is RecipeMeasureVM recipeMeasureVM)
                {
                    if (recipeMeasureVM.CanMeasurePositionsBeOnDie)
                    {
                        var editedMeasure = _editedRecipe.Measures.FirstOrDefault(m => m.Name == recipeMeasureVM.Name);
                        if (editedMeasure != null)
                        {
                            editedMeasure.MeasurePoints.Clear();
                            editedMeasure.IsConfigured = false;
                        }
                        recipeMeasureVM.IsValidated = false;
                    }
                }
            }
        }

        private AutoRelayCommand _gotoTopLeftCorner;

        public AutoRelayCommand GotoTopLeftCorner
        {
            get
            {
                return _gotoTopLeftCorner ?? (_gotoTopLeftCorner = new AutoRelayCommand(
                    () =>
                    {
                        StartDieAndStreetSize.NotifyCanExecuteChanged();
                        TopLeftCornerStep.GotoCurrentPosition();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _gotoBottomRightCorner;

        public AutoRelayCommand GotoBottomRightCorner
        {
            get
            {
                return _gotoBottomRightCorner ?? (_gotoBottomRightCorner = new AutoRelayCommand(
                    () =>
                    {
                        BottomRightCornerStep.GotoCurrentPosition();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _importWaferMap;

        public AutoRelayCommand ImportWaferMap
        {
            get
            {
                return _importWaferMap ?? (_importWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        // Code to execute
                    },
                    () => { return false; }
                ));
            }
        }

        private AutoRelayCommand _submitParameters;

        public AutoRelayCommand SubmitParameters
        {
            get
            {
                return _submitParameters ?? (_submitParameters = new AutoRelayCommand(
                    () =>
                    {
                        if (((_previousIsDieSizeSet != IsDieSizeSet) && IsDieSizeSet)
                        || (IsDieSizeSet && ((_previousDieHeight != DieHeight.Length) || (_previousDieWidth != DieWidth.Length)))
                        || (_previousEdgeExclusion != EdgeExclusion.Length))
                        {
                            WaferMapStep.StepState = StepStates.NotDone;
                            DieAndStreetSizesStep.StepState = StepStates.NotDone;
                        }

                        SetPreviousParameters();

                        if (!CheckCurrentCornerPositionsValidity(out string errorMessage))
                        {
                            // We remove the point on the camera image
                            RemoveCameraPoint(CornerPosition.BottomRight);
                            BottomRightCornerStep.Reset();
                        }

                        IsModified = true;
                    },
                    () => { return HaveParametersChanged(); }
                ));
            }
        }

        private AutoRelayCommand _startDieAndStreetSize;

        public AutoRelayCommand StartDieAndStreetSize
        {
            get
            {
                return _startDieAndStreetSize ?? (_startDieAndStreetSize = new AutoRelayCommand(
                    () =>
                    {
                        IsWaferMapInProgress = true;
                        WaferMapStep.StepState = StepStates.NotDone;
                        var cameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
                        var dieAndStreetSizesInput = new DieAndStreetSizesInput(TopLeftCornerStep.CurrentPatternRecImage, BottomRightCornerStep.CurrentPatternRecImage, _editedRecipe.WaferDimentionalCharacteristic, EdgeExclusion.Length, AutoFocusSettings.GetAutoFocusSettings());
                        dieAndStreetSizesInput.CameraID = cameraId;
                        ServiceLocator.AlgosSupervisor.DieAndStreetSizesChangedEvent += AlgosSupervisor_DieAndStreetSizesChangedEvent;
                        ServiceLocator.AlgosSupervisor.StartDieAndStreetSizes(dieAndStreetSizesInput);
                        DieAndStreetSizesStep.StepState = StepStates.InProgress;
                        DieAndStreetSizes = null;
                    },
                    () => { return canStartDieAndStreetSize(); }
                ));
            }
        }

        private bool canStartDieAndStreetSize()
        {
            var isOneEditing = AutoFocusSettings.IsEditing || BottomRightCornerStep.IsEditing || TopLeftCornerStep.IsEditing;
            var isTopLeftStepDone = TopLeftCornerStep.StepState == StepStates.Done;
            var isBottomRightStepDone = BottomRightCornerStep.StepState == StepStates.Done;
            var isDieAndStreetSizeRunning = DieAndStreetSizesStep.StepState == StepStates.InProgress;
            var isWaferMapRunning = WaferMapStep.StepState == StepStates.InProgress;
            return !isOneEditing && isTopLeftStepDone && isBottomRightStepDone && !isDieAndStreetSizeRunning && !isWaferMapRunning;
        }

        private void SetPreviousParameters()
        {
            _previousDieHeight = DieHeight.Length;
            _previousDieWidth = DieWidth.Length;
            _previousEdgeExclusion = EdgeExclusion.Length;
            _previousIsDieSizeSet = IsDieSizeSet;
        }

        private bool HaveParametersChanged()
        {
            return ((_previousDieHeight != DieHeight.Length) || (_previousDieWidth != DieWidth.Length) || (_previousEdgeExclusion != EdgeExclusion.Length) || (_previousIsDieSizeSet != IsDieSizeSet));
        }

        private AutoRelayCommand _stopWaferMap;

        public AutoRelayCommand StopWaferMap
        {
            get
            {
                return _stopWaferMap ?? (_stopWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        ServiceLocator.AlgosSupervisor.CancelDieAndStreetSizes();
                        ServiceLocator.AlgosSupervisor.CancelWaferMap();
                        IsWaferMapInProgress = false;
                    },
                    () => { return IsWaferMapInProgress; }
                ));
            }
        }



        private AutoRelayCommand _resetCorners;

        public AutoRelayCommand ResetCorners
        {
            get
            {
                return _resetCorners ?? (_resetCorners = new AutoRelayCommand(
                    () =>
                    {
                        RemoveCorners();
                        DieAndStreetSizesStep.StepState = StepStates.NotDone;
                        WaferMapStep.StepState = StepStates.NotDone;
                    },
                    () => { return (TopLeftCornerStep.StepState != StepStates.NotDone) || (BottomRightCornerStep.StepState != StepStates.NotDone); }
                ));
            }
        }

        private void RemoveCorners()
        {
            TopLeftCornerStep.Reset();
            RemoveCameraPoint(CornerPosition.TopLeft);
            BottomRightCornerStep.Reset();
            RemoveCameraPoint(CornerPosition.BottomRight);
        }

        private AutoRelayCommand _displayWaferMap;

        public AutoRelayCommand DisplayWaferMap
        {
            get
            {
                return _displayWaferMap ?? (_displayWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        var waferMapDisplayVM = new WaferMapDisplayVM() { WaferMap = this.WaferMap };
                        ServiceLocator.AxesSupervisor.AxesVM.WaferMap = WaferMap;

                        if (ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<WaferMapDisplay>(waferMapDisplayVM) == true)
                            IsModified = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _validateWaferMap;

        public AutoRelayCommand ValidateWaferMap
        {
            get
            {
                return _validateWaferMap ?? (_validateWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        if (IsModified)
                        {
                            if (_editedRecipe.Measures.Any(m => m.IsConfigured))
                            {
                                if (ServiceLocator.DialogService.ShowMessageBox("All the measure positions will be removed. Are you sure you want to validate this new wafer map ?", "Wafer Map", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                    return;
                            }

                            // We remove all the measure positions
                            RemoveAllTheMeasurePositions();

                            IsValidated = true;
                            SaveRecipeData();
                            ServiceLocator.AxesSupervisor.AxesVM.WaferMap = WaferMap;
                        }
                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () => { UpdateValidationErrorMessage(); return IsReadyToValidate; }
                ));
            }
        }

        private AutoRelayCommand _cancelWaferMap;

        public AutoRelayCommand CancelWaferMap
        {
            get
            {
                return _cancelWaferMap ?? (_cancelWaferMap = new AutoRelayCommand(
                    () =>
                    {
                        LoadRecipeData();
                    },
                    () => { return !(_editedRecipe.WaferMap is null) && IsModified; }
                ));
            }
        }

        public bool CanCancelWaferMap
        {
            get => (!(_editedRecipe.WaferMap is null));
        }

        #endregion RelayCommands

        #region Method Members

        private void LoadRecipeData()
        {
            if (_editedRecipe.WaferMap is null)
            {
                ServiceLocator.AxesSupervisor.AxesVM.WaferMap = WaferMap;
                ServiceLocator.ReferentialSupervisor.DeleteSettings(ReferentialTag.Die);
                if (_editedRecipe.IsWaferMapSkipped)
                    IsValidated = true;
                return;
            }

            CameraPoints.Clear();

            DieAndStreetSizes = _editedRecipe.WaferMap.DieAndStreetSizes;
            DieAndStreetSizesStep.StepState = StepStates.Done;
            WaferMap = _editedRecipe.WaferMap.WaferMapData;

            if (!(_editedRecipe.WaferMap?.AutoFocus is null))
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(_editedRecipe.WaferMap?.AutoFocus);
                AutoFocusSettings.EnableWithoutEditing();
                AutoFocusSettings.AreSettingsVisible = false;
            }

            WaferMapStep.StepState = StepStates.Done;
            if (!(_editedRecipe.WaferMap.EdgeExclusion is null))
                EdgeExclusion = new LengthVM(_editedRecipe.WaferMap.EdgeExclusion);
            IsDieSizeSet = _editedRecipe.WaferMap.IsDieSizeSet;
            if (!(_editedRecipe.WaferMap.DieWidth is null))
                DieWidth = new LengthVM(_editedRecipe.WaferMap.DieWidth);
            if (!(_editedRecipe.WaferMap.DieHeight is null))
                DieHeight = new LengthVM(_editedRecipe.WaferMap.DieHeight);
            IsValidated = true;
            IsModified = false;

            RemoveCorners();
            ServiceLocator.AxesSupervisor.AxesVM.WaferMap = WaferMap;
        }

        private void SaveRecipeData()
        {
            _editedRecipe.IsModified = true;
            var waferMapSettings = new WaferMapSettings();
            waferMapSettings.DieAndStreetSizes = DieAndStreetSizes;
            waferMapSettings.EdgeExclusion = EdgeExclusion.Length;
            waferMapSettings.IsDieSizeSet = IsDieSizeSet;
            waferMapSettings.DieWidth = DieWidth.Length;
            waferMapSettings.DieHeight = DieHeight.Length;
            if (AutoFocusSettings.IsAutoFocusEnabled)
            {
                waferMapSettings.AutoFocus = AutoFocusSettings.GetAutoFocusSettings();
            }
            waferMapSettings.WaferMapData = WaferMap;
            _editedRecipe.WaferMap = waferMapSettings;
            _editedRecipe.IsWaferMapSkipped = false;
            IsModified = false;

            AddDiesSelectionStep();
        }

        private void AddDiesSelectionStep()
        {
            var recipeRunStep = ServiceLocator.NavigationManager.AllPages.FirstOrDefault(p => (p as RecipeWizardStepBaseVM) is RecipeRunVM);
            if (recipeRunStep != null)
            {
                // We check if the Dies selection step already exists
                var diesSelectionStep = ServiceLocator.NavigationManager.AllPages.FirstOrDefault(p => (p as RecipeWizardStepBaseVM) is RecipeDiesSelectionVM);
                if (diesSelectionStep is null)
                {
                    diesSelectionStep = new RecipeDiesSelectionVM(_editedRecipe) { IsEnabled = true };
                    ServiceLocator.NavigationManager.AllPages.Insert(ServiceLocator.NavigationManager.AllPages.IndexOf(recipeRunStep), diesSelectionStep);
                }
            }
        }

        public void StartEditCorner(CornerPosition cornerPosition)
        {
            // We remove the point on the camera image
            RemoveCameraPoint(cornerPosition);
            DieAndStreetSizesStep.StepState = StepStates.NotDone;
            WaferMapStep.StepState = StepStates.NotDone;
            if ((cornerPosition == CornerPosition.BottomRight) && (TopLeftCornerStep.StepState == StepStates.Done) && IsDieSizeSet)
                ServiceLocator.AxesSupervisor.GotoPosition(new XYPosition(new WaferReferential(), TopLeftCornerStep.CurrentPositionX + DieWidth.Length.Millimeters, TopLeftCornerStep.CurrentPositionY - DieHeight.Length.Millimeters), AxisSpeed.Fast);
        }

        private bool CheckCurrentCornerPositionsValidity(out string errorMessage)
        {
            // We suppose the top left corner is the reference, we check if the bottom right corner is OK
            return CheckCornerPositionValidity(CornerPosition.BottomRight, BottomRightCornerStep.CurrentPatternRecImage, out errorMessage);
        }

        internal bool CheckCornerPositionValidity(CornerPosition cornerPosition, PositionWithPatternRec patternRecImageToCheck, out string errorMessage)
        {
            errorMessage = String.Empty;
            double calculatedWidth = 0;
            double calculatedHeight = 0;

            // If we have only one corner it is always valid
            switch (cornerPosition)
            {
                case CornerPosition.TopLeft:
                    if ((BottomRightCornerStep.StepState != StepStates.Done) || (BottomRightCornerStep.CurrentPatternRecImage is null) || (patternRecImageToCheck is null))
                        return true;

                    calculatedWidth = CalculateWidth(patternRecImageToCheck.Position, BottomRightCornerStep.CurrentPatternRecImage.Position);
                    calculatedHeight = CalculateHeight(patternRecImageToCheck.Position, BottomRightCornerStep.CurrentPatternRecImage.Position);
                    break;

                case CornerPosition.BottomRight:
                    if ((TopLeftCornerStep.StepState != StepStates.Done) || (TopLeftCornerStep.CurrentPatternRecImage is null) || (patternRecImageToCheck is null))
                        return true;

                    calculatedWidth = CalculateWidth(TopLeftCornerStep.CurrentPatternRecImage.Position, patternRecImageToCheck.Position);
                    calculatedHeight = CalculateHeight(TopLeftCornerStep.CurrentPatternRecImage.Position, patternRecImageToCheck.Position);
                    break;
            }

            // Is the size valid ?
            if (calculatedWidth <= 0)
            {
                if (cornerPosition == CornerPosition.TopLeft)
                    errorMessage = "The top left corner must be on the left of the bottom right corner";
                else
                    errorMessage = "The bottom right corner must be on the right of the top left corner";
                return false;
            }
            if (calculatedHeight <= 0)
            {
                if (cornerPosition == CornerPosition.TopLeft)
                    errorMessage = "The top left corner must be above the bottom right corner";
                else
                    errorMessage = "The bottom right corner must be under the top left corner";
                return false;
            }

            if (!_previousIsDieSizeSet)   // nothing to check
                return true;

            // We must check that the size corresponds to the size set by the user
            if (Math.Abs(_previousDieWidth.Millimeters - calculatedWidth) > DieSizeTolerance)
            {
                errorMessage = "The selected corners do not correspond to the die width";
                return false;
            }

            if (Math.Abs(_previousDieHeight.Millimeters - calculatedHeight) > DieSizeTolerance)
            {
                errorMessage = "The selected corners do not correspond to the die height";
                return false;
            }

            return true;
        }

        private double CalculateWidth(PositionBase positionTopLeft, PositionBase positionBottomRight)
        {
            double width;

            if (positionTopLeft.Referential != positionBottomRight.Referential)
                throw new ArgumentException("Can not calculate a size with two positions in different referentials");
            if ((positionTopLeft is XYZTopZBottomPosition xyZTopZBottomPositionTopLeft) && (positionBottomRight is XYZTopZBottomPosition xyZTopZBottomPositionBottomRight))
            {
                width = xyZTopZBottomPositionBottomRight.X - xyZTopZBottomPositionTopLeft.X;
                return width;
            }
            else
            {
                throw new ArgumentException("The two positions must be XYPosition");
            }
        }

        private double CalculateHeight(PositionBase positionTopLeft, PositionBase positionBottomRight)
        {
            double height;

            if (positionTopLeft.Referential != positionBottomRight.Referential)
                throw new ArgumentException("Can not calculate a size with two positions in different referentials");
            if ((positionTopLeft is XYZTopZBottomPosition xyZTopZBottomPositionTopLeft) && (positionBottomRight is XYZTopZBottomPosition xyZTopZBottomPositionBottomRight))
            {
                height = xyZTopZBottomPositionTopLeft.Y - xyZTopZBottomPositionBottomRight.Y;
                return height;
            }
            else
            {
                throw new ArgumentException("The two positions must be XYPosition");
            }
        }

        internal void CornerPositionValidated(CornerPosition cornerPosition)
        {
            // We remove the point on the camera image
            RemoveCameraPoint(cornerPosition);

            switch (cornerPosition)
            {
                case CornerPosition.TopLeft:

                    CameraPoints.Add(new CornerPoint("Top left corner", new System.Windows.Point(TopLeftCornerStep.CurrentPositionX, TopLeftCornerStep.CurrentPositionY), CornerPosition.TopLeft));
                    break;

                case CornerPosition.BottomRight:

                    CameraPoints.Add(new CornerPoint("Bottom right corner", new System.Windows.Point(BottomRightCornerStep.CurrentPositionX, BottomRightCornerStep.CurrentPositionY), CornerPosition.BottomRight));
                    break;

                default:
                    break;
            }
        }

        private void AlgosSupervisor_DieAndStreetSizesChangedEvent(DieAndStreetSizesResult dieAndStreetSizesResult)
        {
            switch (dieAndStreetSizesResult.Status.State)
            {
                case FlowState.Waiting:
                    return;

                case FlowState.InProgress:
                    DieAndStreetSizesStep.StepState = StepStates.InProgress;
                    return;

                case FlowState.Error:
                    DieAndStreetSizesStep.StepState = StepStates.Error;
                    DieAndStreetSizesStep.ErrorMessage = dieAndStreetSizesResult.Status.Message;
                    break;

                case FlowState.Canceled:
                    DieAndStreetSizesStep.StepState = StepStates.Error;
                    DieAndStreetSizesStep.ErrorMessage = "Canceled";
                    break;

                case FlowState.Success:
                    DieAndStreetSizesStep.StepState = StepStates.Done;
                    DieAndStreetSizes = dieAndStreetSizesResult;
                    StartWaferMap();
                    break;

                default:
                    break;
            }
            UpdateAllCanExecutes();
            ServiceLocator.AlgosSupervisor.DieAndStreetSizesChangedEvent -= AlgosSupervisor_DieAndStreetSizesChangedEvent;

        }

        private void StartWaferMap()
        {
            var waferMapInput = new WaferMapInput(
                topLeftCorner: TopLeftCornerStep.CurrentPatternRecImage,
                bottomRightCorner: BottomRightCornerStep.CurrentPatternRecImage,
                waferCharacteristics: _editedRecipe.WaferDimentionalCharacteristic,
                edgeExclusion: EdgeExclusion.Length,
                dieDimensions: DieAndStreetSizes.DieDimensions);

            ServiceLocator.AlgosSupervisor.WaferMapChangedEvent += AlgosSupervisor_WaferMapChangedEvent;
            ServiceLocator.AlgosSupervisor.StartWaferMap(waferMapInput);
            
            WaferMapStep.StepState = StepStates.InProgress;
            WaferMap = null;
        }

        private void AlgosSupervisor_WaferMapChangedEvent(WaferMapResult waferMapResult)
        {
            switch (waferMapResult.Status.State)
            {
                case FlowState.Waiting:
                    return;

                case FlowState.InProgress:
                    WaferMapStep.StepState = StepStates.InProgress;
                    return;

                case FlowState.Error:
                    WaferMapStep.StepState = StepStates.Error;
                    WaferMapStep.ErrorMessage = waferMapResult.Status.Message;
                    IsWaferMapInProgress = false;
                    break;

                case FlowState.Canceled:
                    WaferMapStep.StepState = StepStates.Error;
                    WaferMapStep.ErrorMessage = "Canceled";
                    IsWaferMapInProgress = false;
                    break;

                case FlowState.Success:
                    WaferMap = waferMapResult;
                    WaferMapStep.StepState = StepStates.Done;
                    WaferMap.DieReference = new DieIndex(0, WaferMap.NbRows - 1);
                    AutoFocusSettings.IsModified = false;
                    IsWaferMapInProgress = false;
                    break;

                default:
                    break;
            }
            UpdateAllCanExecutes();
            ServiceLocator.AlgosSupervisor.WaferMapChangedEvent -= AlgosSupervisor_WaferMapChangedEvent;
        }

        private void RemoveCameraPoint(CornerPosition cornerToRemove)
        {
            // We remove the point on the camera image
            switch (cornerToRemove)
            {
                case CornerPosition.TopLeft:
                    var topLeftCameraPoint = CameraPoints.FirstOrDefault(cp => (cp as CornerPoint).CornerPosition == CornerPosition.TopLeft);
                    if (!(topLeftCameraPoint is null)) CameraPoints.Remove(topLeftCameraPoint);
                    break;

                case CornerPosition.BottomRight:
                    var bottomRightCameraPoint = CameraPoints.FirstOrDefault(cp => (cp as CornerPoint).CornerPosition == CornerPosition.BottomRight);
                    if (!(bottomRightCameraPoint is null)) CameraPoints.Remove(bottomRightCameraPoint);
                    break;

                default:
                    break;
            }
        }

        internal bool CanEdit()
        {
            if (TopLeftCornerStep.IsEditing || BottomRightCornerStep.IsEditing)
                return false;

            return true;
        }

        private void UpdateValidationErrorMessage()
        {
            if (!(DieAndStreetSizesStep.StepState == StepStates.Done) || (!(WaferMapStep.StepState == StepStates.Done)))
                ValidationErrorMessage = "WaferMap settings are not valid";
            else if (!IsModified)
                ValidationErrorMessage = "WaferMap is not modified";
            else
                ValidationErrorMessage = string.Empty;
        }

        #endregion Method Members

        #region INavigable

        public override async Task PrepareToDisplay()
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                // We select the main objective
                _objectiveToUse = ClassLocator.Default.GetInstance<CamerasSupervisor>().MainObjective;
                ClassLocator.Default.GetInstance<CamerasSupervisor>().Objective = _objectiveToUse;
                ServiceLocator.AxesSupervisor.WaitMotionEnd(20_000);
                ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming();

                // We move to the half-radius top-left position in the wafer
                double waferRadius = 0.5 * _editedRecipe.WaferDimentionalCharacteristic.Diameter.Millimeters;
                double percentageRadius = 0.5;
                var initialPosition = new XYZTopZBottomPosition(new WaferReferential(),
                                                                -waferRadius * percentageRadius,
                                                                waferRadius * percentageRadius,
                                                                0,
                                                                double.NaN);
                ServiceLocator.AxesSupervisor.GotoPosition(initialPosition, AxisSpeed.Fast);

                // Enable Lights
                ServiceLocator.LightsSupervisor.LightsAreLocked = false;

                // Set lights intensity
                if (!(_editedRecipe.Alignment is null))
                {
                    foreach (var light in ServiceLocator.LightsSupervisor.Lights)
                    {
                        ServiceLocator.LightsSupervisor.SetLightIntensity(light.DeviceID, 0);
                    }

                    ServiceLocator.LightsSupervisor.SetLightIntensity(Proxy.ServiceLocator.LightsSupervisor.GetMainLight().DeviceID, _editedRecipe.Alignment.AutoLight.LightIntensity);
                }

                TopLeftCornerStep.PropertyChanged += Step_PropertyChanged;
                BottomRightCornerStep.PropertyChanged += Step_PropertyChanged;
                DieAndStreetSizesStep.PropertyChanged += Step_PropertyChanged;
                WaferMapStep.PropertyChanged += Step_PropertyChanged;

                base.OnPropertyChanged(nameof(RoiRectWaferMap));
                IsBusy = false;
            });
        }

        private void Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlignmentStepBaseVM.StepState))
            {
                OnPropertyChanged(nameof(GlobalWaferMapState));
            }
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (IsReadyToValidate && IsModified && !forceClose)
            {
                var result = ServiceLocator.DialogService.ShowMessageBox("The wafer map has not been validated. Do you really want to leave ?", "Wafer Map", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }

            // We restore the last validated state
            try
            {
                LoadRecipeData();
            }
            catch (Exception)
            {
                Logger.Error("Impossible to restore Wafer Map state");
            }

            if (TopLeftCornerStep.StepState == StepStates.InProgress)
                TopLeftCornerStep.StepState = StepStates.NotDone;

            if (BottomRightCornerStep.StepState == StepStates.InProgress)
                BottomRightCornerStep.StepState = StepStates.NotDone;

            ServiceLocator.AlgosSupervisor.CancelDieAndStreetSizes();
            ServiceLocator.AlgosSupervisor.CancelWaferMap();

            TopLeftCornerStep.PropertyChanged -= Step_PropertyChanged;
            BottomRightCornerStep.PropertyChanged -= Step_PropertyChanged;
            DieAndStreetSizesStep.PropertyChanged -= Step_PropertyChanged;
            WaferMapStep.PropertyChanged -= Step_PropertyChanged;

            ServiceLocator.AlgosSupervisor.CancelDieAndStreetSizes();

            TopLeftCornerStep.PropertyChanged -= Step_PropertyChanged;
            BottomRightCornerStep.PropertyChanged -= Step_PropertyChanged;
            DieAndStreetSizesStep.PropertyChanged -= Step_PropertyChanged;
            WaferMapStep.PropertyChanged -= Step_PropertyChanged;

            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();

            IsModified = false;
            return true;
        }

        #endregion INavigable
    }
}
