using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeMeasureVM : RecipeWizardStepBaseVM, IDisposable
    {
        public ANARecipeVM EditedRecipe { get; set; }

        public MeasureType MeasureType { get; set; }

        public new bool IsActive { get; set; }

        public RecipeMeasureVM(ANARecipeVM editedRecipe, MeasureType measureType, string measureName, bool isActive)
        {
            Name = measureName;
            IsEnabled = isActive;
            IsActive = isActive;
            IsMeasure = true;
            IsValidated = false;
            MeasureType = measureType;
            EditedRecipe = editedRecipe;
            MeasurePoints = new MeasurePointsVM(this);

            var measure = EditedRecipe.GetMeasureFromName(Name);
            if (!(measure is null) && measure.IsConfigured)
            {
                IsValidated = true;
            }
            IsActive = isActive;
        }

        #region Method Members

        private async Task LoadRecipeData()
        {
            var measure = EditedRecipe.GetMeasureFromName(Name);

            if (measure is null)
                return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MeasurePoints.Points.Clear();
            }));
            await LoadMeasureSettings(measure);

            var measurePointsIds = measure.IsMeasureWithSubMeasurePoints ? measure.SubMeasurePoints : measure.MeasurePoints;
            if (!(measurePointsIds is null))
            {
                foreach (var measurePointId in measurePointsIds)
                {
                    var recipeMeasurePoint = EditedRecipe.Points.Find(p => p.Id == measurePointId);
                    if (!(recipeMeasurePoint is null))
                    {
                        // Add the recipeMeasure point to the points manager
                        var newMeasurePointVM = MeasurePointVM.FromMeasurePoint(MeasurePoints, recipeMeasurePoint, MeasureSettings.ArePositionsOnDie);

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MeasurePoints.Points.Add(newMeasurePointVM);
                        }));
                    }
                }
            }

            if (measure.IsConfigured)
            {
                IsValidated = true;
            }
            MeasurePoints.IsModified = false;
            MeasureSettings.IsModified = false;
        }

        private async Task LoadMeasureSettings(MeasureSettingsBase measure)
        {
            if (MeasureSettings is null)
                MeasureSettings = CreateMeasureSettingsVM(measure.MeasureType);

            if (measure.IsConfigured)
                await MeasureSettings.LoadSettingsAsync(measure);
        }

        private void MeasureSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MeasureSettingsVM.IsModified))
            {
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(RequiresValidation));
            }
        }

        private void MeasurePoints_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MeasurePointsVM.IsModified))
            {
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(RequiresValidation));
            }
        }

        private void SaveRecipeData()
        {
            var index = EditedRecipe.Measures.FindIndex(m => m.Name == Name);
            if (index != -1) // Ensure the measure exists in the list
            {
                EditedRecipe.Measures.RemoveAt(index);
                var measureSettings = CreateMeasureSettings();
                if (!(measureSettings is null))
                {
                    EditedRecipe.Measures.Insert(index, measureSettings);
                }
            }  

            EditedRecipe.IsModified = true;

            IsValidated = true;
            MeasureSettings.IsModified = false;
            MeasurePoints.IsModified = false;
        }

        private MeasureSettingsBase CreateMeasureSettings(bool addPointsToTheRecipe = true)
        {
            MeasureSettingsBase newMeasureSettings = MeasureSettings.GetSettingsWithoutPoints();

            if (newMeasureSettings is null)
            {
                return null;
            }

            newMeasureSettings.Name = Name;
            newMeasureSettings.IsActive = true;
            newMeasureSettings.IsConfigured = true;
            newMeasureSettings.MeasurePoints = new List<int>();
            newMeasureSettings.SubMeasurePoints = new List<int>();
            if (addPointsToTheRecipe)
            {
                var realMeasurePoints = MeasureSettings.GetRealMeasurePoints(MeasurePoints.Points);
                if (realMeasurePoints != null)
                {
                    foreach (var measurePoint in realMeasurePoints)
                    {
                        int newPointID = (int)measurePoint.Id;
                        var newMeasurePoint = new MeasurePoint(newPointID, measurePoint.PointPosition, MeasureSettings.ArePositionsOnDie);
                        newMeasurePoint.IsSubMeasurePoint = measurePoint.IsSubMeasurePoint;
                        if (!EditedRecipe.Points.Exists(p => p.Id == measurePoint.Id))
                        {
                            newMeasurePoint.PatternRec = measurePoint.PointPatternRec;
                            EditedRecipe.Points.Add(newMeasurePoint);
                        }
                        else
                        {
                            if (measurePoint.IsModified)
                            {
                                var pointToModify = EditedRecipe.Points.FirstOrDefault(p => p.Id == measurePoint.Id);
                                if (pointToModify != null)
                                {
                                    pointToModify.Position = new PointPosition(measurePoint.PointPosition);
                                    pointToModify.PatternRec = measurePoint.PointPatternRec;
                                }

                                measurePoint.IsModified = false;
                            }
                        }
                        newPointID = (int)measurePoint.Id;
                        newMeasureSettings.MeasurePoints.Add(newPointID);
                        if (measurePoint.IsSubMeasurePoint)
                        {
                            newMeasureSettings.SubMeasurePoints.Add(newPointID);
                        }
                    }
                }
            }
            else
            {
                if (MeasurePoints.RecipeMeasure.IsMeasureWithSubMeasures())
                {
                    // this case can happen on a test in recipe for a measure that contains sub measure points
                    var realMeasurePoints = MeasureSettings.GetRealMeasurePoints(MeasurePoints.Points);
                    if (realMeasurePoints != null)
                    {
                        foreach (var measurePoint in realMeasurePoints.Where(p => p.IsSubMeasurePoint))
                        {
                            var newPointID = (int)measurePoint.Id;
                            var newMeasurePoint = new MeasurePoint(newPointID, measurePoint.PointPosition, MeasureSettings.ArePositionsOnDie);
                            newMeasurePoint.IsSubMeasurePoint = true;
                            newMeasureSettings.SubMeasurePoints.Add(newPointID);
                        }
                    }
                }
            }

            return newMeasureSettings;
        }

        public bool IsMeasureWithSubMeasures()
        {
            return MeasureSettings.IsMeasureWithSubMeasurePoints;
        }

        public void TestMeasureOnPoint(MeasurePointVM measurePoint)
        {
            IsMeasureTestInProgress = true;

            MeasurePoint newMeasurePoint = new MeasurePoint(0, measurePoint.PointPosition, MeasureSettings.ArePositionsOnDie)
            {
                PatternRec = measurePoint.PointPatternRec
            };

            try
            {
                DieIndex curDieIndex = null;
                if (!(EditedRecipe.WaferMap is null) && !MeasureSettings.IsMeasureWithSubMeasurePoints)
                {
                    curDieIndex = ServiceLocator.AxesSupervisor.AxesVM.CurrentDieIndex;
                }

                var subMeasurePointsVM = MeasurePoints.Points.Where(p => p.IsSubMeasurePoint).ToList();
                if (subMeasurePointsVM != null && subMeasurePointsVM.Count > 0)
                {
                    MeasurePoint GetSubMeasurePointFromMeasurePointVM(MeasurePointVM measurePointVM)
                    {
                        var subMeasurePoint = new MeasurePoint(measurePointVM.Id.Value, new PointPosition(measurePointVM.PointPosition), measurePointVM.PointPatternRec, measurePointVM.IsDiePosition);
                        subMeasurePoint.IsSubMeasurePoint = true;
                        return subMeasurePoint;
                    }
                    var subMeasurePoints = subMeasurePointsVM.Select(s => GetSubMeasurePointFromMeasurePointVM(s)).ToList();
                    ServiceLocator.MeasureSupervisor.StartMeasureWithSubMeasures(CreateMeasureSettings(false), newMeasurePoint, subMeasurePoints, curDieIndex);
                }
                else
                {
                    ServiceLocator.MeasureSupervisor.StartMeasure(CreateMeasureSettings(false), newMeasurePoint, curDieIndex);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on TestMeasureOnPoint");
                Messenger.Send(new Message(MessageLevel.Error, $"Start measure error {ex.Message}"));
            }
        }

        private void MeasureSupervisor_MeasureResultChangedEvent(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Console.WriteLine("MeasureSupervisor_MeasureResultChangedEvent : After begin invoke");
                //If the measurement is no longer in progress,
                //it has been cancelled, so we may not display the result
                if (!IsMeasureTestInProgress)
                    return;

                IsMeasureTestInProgress = false;
                // Note : Test even it has no value should be always displayed even if it has some not measured or error values 
                // we want to measure not to judge the measure
                // therefore  UI should handle some  Null, Nan or missings measures values in some case

                // UNcomment this only if 
                //if ((result.State == MeasureState.Success) || (result.State == MeasureState.Partial))
                //{
                DieIndex userDieIndex = null;
                if (dieIndex != null)
                    userDieIndex = ServiceLocator.AxesSupervisor.AxesVM.ConvertDieIndexToDieUserIndex(dieIndex);
                MeasureSettings.DisplayTestResult(result, resultFolderPath, userDieIndex);
                //}
                //else
                //    ServiceLocator.DialogService.ShowMessageBox($"The measure failed.\n{result.Message}", "Measure", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        private int GetNextPointId()
        {
            if (EditedRecipe.Points.Count == 0)
                return 1;

            EditedRecipe.Points.Sort((x, y) => x.Id.CompareTo(y.Id));

            return EditedRecipe.Points[EditedRecipe.Points.Count - 1].Id + 1;
        }

        private void UpdateValidationErrorMessage()
        {
            if (MeasureSettings is null)
                return;
            ValidationErrorMessage = MeasureSettings.ValidationErrorMessage;
            if (ValidationErrorMessage.IsNullOrEmpty() && (!IsModified))
            {
                ValidationErrorMessage = "Measure is not modified";
                return;
            }
        }

        #endregion Method Members

        #region Properties

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private MeasureSettingsVM _measureSettings;

        public MeasureSettingsVM MeasureSettings
        {
            get => _measureSettings;
            set
            {
                if (_measureSettings != value)
                {
                    if (!(_measureSettings is null))
                        _measureSettings.PropertyChanged -= MeasureSettings_PropertyChanged;
                    _measureSettings = value;
                    _measureSettings.PropertyChanged += MeasureSettings_PropertyChanged;
                    OnPropertyChanged();
                }
            }
        }

        private MeasurePointsVM _measurePoints;

        public MeasurePointsVM MeasurePoints
        {
            get => _measurePoints;
            set
            {
                if (_measurePoints != value)
                {
                    if (!(_measurePoints is null))
                        _measurePoints.PropertyChanged -= MeasurePoints_PropertyChanged;
                    _measurePoints = value;
                    _measurePoints.PropertyChanged += MeasurePoints_PropertyChanged;
                    OnPropertyChanged();
                }
            }
        }

        public DieDimensionalCharacteristic DieDimensions => EditedRecipe?.WaferMap?.DieAndStreetSizes?.DieDimensions;

        public WaferMapResult WaferMap => EditedRecipe?.WaferMap?.WaferMapData;

        private bool _isCenteredRoi = true;

        public bool IsCenteredRoi
        {
            get => _isCenteredRoi; set { if (_isCenteredRoi != value) { _isCenteredRoi = value; OnPropertyChanged(); } }
        }

        public bool CanMeasurePositionsBeOnDie { get; internal set; } = true;

        private bool _displayROI = false;

        public bool DisplayROI
        {
            get => _displayROI; set { if (_displayROI != value) { _displayROI = value; OnPropertyChanged(); } }
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

        private Rect _roiRect = Rect.Empty;

        public Rect RoiRect
        {
            get
            {
                if (_roiRect == Rect.Empty)
                    _roiRect = new Rect(0, 0, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                return _roiRect;
            }

            set { if (_roiRect != value) { _roiRect = value; OnPropertyChanged(); } }
        }

        private int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;

                    UpdateDisplayedTab();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateDisplayedTab()
        {
            if (SelectedIndex == 1)
            {
                MeasureSettings.HideSettingsTab();
                MeasurePoints.DisplayMeasurePointsTab();
            }
            else
            {
                MeasureSettings.DisplaySettingsTab();
                MeasureSettings.SetObjectiveUsedByMeasure();
            }
        }

        public void ResetRoiRect()
        {
            RoiRect = Rect.Empty;
        }

        private double _liseGain = 1.8;

        public double LiseGain
        {
            get => _liseGain; set { if (_liseGain != value) { _liseGain = value; MeasureSettings.IsModified = true; OnPropertyChanged(); } }
        }

        public bool IsModified
        {
            get => (MeasureSettings?.IsModified ?? false) || MeasurePoints.IsModified;
        }

        private bool _settingsAreValid => MeasureSettings?.AreSettingsValid(MeasurePoints.Points) ?? false;

        private bool IsReadyToValidate()
        {
            bool isReadyToValidate = _settingsAreValid;

            UpdateValidationErrorMessage();

            OnPropertyChanged(nameof(RequiresValidation));

            return isReadyToValidate;
        }

        public bool RequiresValidation => !IsValidated || IsModified || !_settingsAreValid;

        private bool _isMeasureTestInProgress = false;

        public bool IsMeasureTestInProgress
        {
            get => _isMeasureTestInProgress; set { if (_isMeasureTestInProgress != value) { _isMeasureTestInProgress = value; OnPropertyChanged(); } }
        }

        private string _validationErrorMessage = string.Empty;

        public string ValidationErrorMessage
        {
            get => _validationErrorMessage; set { if (_validationErrorMessage != value) { _validationErrorMessage = value; OnPropertyChanged(); } }
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

        // Factory for measure Setings VM
        private MeasureSettingsVM CreateMeasureSettingsVM(MeasureType measureType)
        {
            switch (measureType)
            {
                case MeasureType.Thickness:
                    return new ThicknessSettingsVM(this);

                case MeasureType.TSV:
                    return new TSVSettingsVM(this);

                case MeasureType.NanoTopo:
                    return new NanoTopoSettingsVM(this);

                case MeasureType.Topography:
                    return new TopographySettingsVM(this);

                case MeasureType.Bow:
                    return new BowSettingsVM(this);

                case MeasureType.Warp:
                    return new WarpSettingsVM(this);
                
                case MeasureType.EdgeTrim:
                    return new EdgeTrimSettingsVM(this);

                case MeasureType.Trench:
                    return new TrenchSettingsVM(this);
            }

            return null;
        }

        #region RelayCommands

        private AutoRelayCommand _validateMeasure;

        public AutoRelayCommand ValidateMeasure
        {
            get
            {
                return _validateMeasure ?? (_validateMeasure = new AutoRelayCommand(
                    () =>
                    {
                        if (IsModified)
                        {
                            MeasureSettings.EnsureMeasurePointsZPositions(MeasurePoints.Points);
                            SaveRecipeData();
                        }

                        ServiceLocator.NavigationManager.NavigateToNextPage();
                    },
                    () => { return IsReadyToValidate(); }
                ));
            }
        }

        private AutoRelayCommand _cancelMeasure;

        public AutoRelayCommand CancelMeasure
        {
            get
            {
                return _cancelMeasure ?? (_cancelMeasure = new AutoRelayCommand(
                     async () =>
                    {
                        var loadtask = LoadRecipeData();
                        MeasureSettings.IsModified = false;
                        MeasurePoints.IsModified = false;
                        await loadtask;
                    },
                    () => { return IsModified; }
                ));
            }
        }

        public bool CanCancelMeasure
        {
            get => (!(EditedRecipe.Measures is null)) && (!(EditedRecipe.GetMeasureFromName(Name) is null)) && EditedRecipe.GetMeasureFromName(Name).IsConfigured;
        }

        private AutoRelayCommand _startMeasureTest;

        public AutoRelayCommand StartMeasureTest
        {
            get
            {
                return _startMeasureTest ?? (_startMeasureTest = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            var measurePointForTest = MeasurePoints.CreateMeasurePointOnCurrentPosition();
                            TestMeasureOnPoint(measurePointForTest);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Error on StartMeasureTest");
                            Messenger.Send(new Message(MessageLevel.Error, $"Start measure error {ex.Message}"));
                        }
                    },
                    () => { return MeasureSettings?.AreSettingsValid(MeasurePoints.Points, true) ?? false; }
                ));
            }
        }

        private AutoRelayCommand _cancelMeasureTest;

        public AutoRelayCommand CancelMeasureTest
        {
            get
            {
                return _cancelMeasureTest ?? (_cancelMeasureTest = new AutoRelayCommand(
                    () =>
                    {
                        ServiceLocator.MeasureSupervisor.CancelMeasure();
                        IsMeasureTestInProgress = false;
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion RelayCommands

        #region INavigable

        public override async Task PrepareToDisplay()
        {
            IsBusy = true;

            var startstreamtask = Task.Run(() => ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming());
            ServiceLocator.MeasureSupervisor.MeasureResultChangedEvent += MeasureSupervisor_MeasureResultChangedEvent;
            await LoadRecipeData();
            await MeasureSettings.PrepareToDisplayAsync();
            await MeasurePoints.PrepareToDisplayAsync();

            // We select the settings tab
            SelectedIndex = 0;

            IsBusy = false;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (IsModified && IsReadyToValidate() && !forceClose)
            {
                var result = ServiceLocator.DialogService.ShowMessageBox($"The measure {Name} has not been validated. Do you really want to leave ?", $"Measure {Name}", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            MeasureSettings.IsModified = false;
            MeasurePoints.IsModified = false;
            MeasureSettings.Hide();
            SelectedIndex = -1;
            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();
            ServiceLocator.MeasureSupervisor.MeasureResultChangedEvent -= MeasureSupervisor_MeasureResultChangedEvent;

            return true;
        }

        #endregion INavigable

        #region IDisposable

        public void Dispose()
        {
            MeasurePoints.PropertyChanged -= MeasurePoints_PropertyChanged;
            MeasurePoints.Dispose();
            if (!(MeasureSettings is null))
            {
                MeasureSettings.PropertyChanged -= MeasureSettings_PropertyChanged;
                MeasureSettings.Dispose();
            }
        }



        #endregion IDisposable
    }
}
