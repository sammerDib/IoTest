using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class TopographySettingsVM : MeasureSettingsVM
    {
        public static IMapper TopographySettingsMapper;
        private bool _isDisplayed = false;
        private ResultCorrectionType _correctionType;
        static TopographySettingsVM()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TopographySettings, TopographySettingsVM>().ReverseMap();
                cfg.CreateMap<AutoFocusSettings, AutoFocusSettingsVM>().ReverseMap();
                cfg.CreateMap<PostProcessingSettings, PostProcessingSettingsVM>().ReverseMap();
                cfg.CreateMap<PostProcessingOutput, PostProcessingOutputVM>().ReverseMap();
                cfg.CreateMap<ResultCorrectionAnyUnitSettings, ResultCorrectionAnyUnitSettingsVM>().ReverseMap();
            });
            TopographySettingsMapper = configuration.CreateMapper();
        }

        public TopographySettingsVM(RecipeMeasureVM recipeMeasure)
        {
            var topographyMeasureConfig = (MeasureTopoConfiguration)(ServiceLocator.MeasureSupervisor.GetMeasureConfiguration(MeasureType.Topography)?.Result);
            if (topographyMeasureConfig != null)
            {
                _correctionType = topographyMeasureConfig.CorrectionType;
            }
            RecipeMeasure = recipeMeasure;
            AutoFocusSettings = new AutoFocusSettingsVM();
            PostProcessingSettings = new PostProcessingSettingsVM(_correctionType);
            PostProcessingSettings.IsEnabled = true;
            AutoFocusSettings.Type = AutoFocusType.Camera;
        }

        public override async Task LoadSettingsAsync(MeasureSettingsBase measureSettings)
        {
            // If a loading was already in progress, we wait
            if (!SpinWait.SpinUntil(() => !IsLoading, 20000))
            {
                ClassLocator.Default.GetInstance<ILogger>().Error("Topography Loading failed");
                return;
            }

            if (!(measureSettings is TopographySettings topographySettings))
                return;

            IsLoading = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TopographySettingsVM.TopographySettingsMapper.Map<TopographySettings, TopographySettingsVM>(topographySettings, this);
            }));

            foreach (var output in this.PostProcessingSettings.Outputs)
            {
                output.Correction.CorrectionType = _correctionType;
            }

            HeightVariation = topographySettings.HeightVariation;
            ScanMargin = topographySettings.ScanMargin ?? 0.Micrometers();
            SurfaceInFocus = topographySettings.SurfacesInFocus;

            if (topographySettings.ROI is null)
            {
                UseROI = false;
            }
            else
            {
                UseROI = true;
                RoiSize = RoiHelpers.GetSizeInPixels(topographySettings.ROI, topographySettings.MeasureContext.TopObjectiveContext.ObjectiveId);
            }

            if (topographySettings.AutoFocusSettings is null)
            {
                AutoFocusSettings = new AutoFocusSettingsVM();
                AutoFocusSettings.Type = AutoFocusType.Camera;
            }
            else
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(topographySettings.AutoFocusSettings);
                AutoFocusSettings.EnableWithoutEditing();
            }
            AutoFocusSettings.AreSettingsVisible = false;

            if (topographySettings.PostProcessingSettings is null)
            {
                PostProcessingSettings = new PostProcessingSettingsVM(_correctionType);
            }
            else
            {
                // TODO : restore post processing from topographySettings.PostProcessingSettings
            }

            LightsIntensities.Clear();
            if (!(topographySettings.MeasureContext is null))
            {
                foreach (var lightIntensity in topographySettings.MeasureContext.Lights.Lights)
                {
                    LightsIntensities.Add(lightIntensity.DeviceID, lightIntensity.Intensity);
                }
            }

            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
            await ApplyLightSettingsAsync();
            ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;
 
            CharacteristicsChanged = false;
            IsModified = false;
            IsLoading = false;
        }

        private async Task ApplyLightSettingsAsync()
        {
            foreach (var light in LightsIntensities.ToList())
            {
                await ServiceLocator.LightsSupervisor.SetLightIntensityAsync(light.Key, light.Value);
            }
        }

        #region Properties

        private Length _heightVariation = 10.Micrometers();

        public Length HeightVariation
        {
            get => _heightVariation; set { if (_heightVariation != value) { _heightVariation = value; IsModified = true; OnPropertyChanged(); } }
        }

        private Length _scanMargin = 0.Micrometers();

        public Length ScanMargin
        {
            get => _scanMargin; set { if (_scanMargin != value) { _scanMargin = value; IsModified = true; OnPropertyChanged(); } }
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
                }
            }
        }

        private bool _isShapeRemoval = false;

        public bool IsShapeRemoval
        {
            get => _isShapeRemoval; set { if (_isShapeRemoval != value) { _isShapeRemoval = value; IsModified = true; OnPropertyChanged(); } }
        }

        private RecipeMeasureVM _recipeMeasure;

        public RecipeMeasureVM RecipeMeasure
        {
            get { return _recipeMeasure; }
            set
            {
                if (_recipeMeasure != value)
                {
                    _recipeMeasure = value;

                    OnPropertyChanged();
                }
            }
        }

        private bool _characteristicsChanged = true;

        public bool CharacteristicsChanged
        {
            get => _characteristicsChanged; set { if (_characteristicsChanged != value) { _characteristicsChanged = value; OnPropertyChanged(); } }
        }

        private PostProcessingSettingsVM _postProcessingSettings;

        public PostProcessingSettingsVM PostProcessingSettings
        {
            get => _postProcessingSettings;
            set
            {
                if (_postProcessingSettings != value)
                {
                    if (!(_postProcessingSettings is null))
                    {
                        _postProcessingSettings.PostProcessingSettingsModified -= PostProcessingSettings_Modified;
                    }
                    _postProcessingSettings = value;
                    if (!(_postProcessingSettings is null))
                    {
                        _postProcessingSettings.PostProcessingSettingsModified += PostProcessingSettings_Modified;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private AutoFocusSettingsVM _autoFocusSettings;

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get => _autoFocusSettings;
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

        private SurfacesInFocus _surfaceInFocus = SurfacesInFocus.Unknown;

        public SurfacesInFocus SurfaceInFocus
        {
            get
            {
                return _surfaceInFocus;
            }

            set
            {
                if (_surfaceInFocus == value)
                {
                    return;
                }

                _surfaceInFocus = value;
                IsModified = true;
                OnPropertyChanged();
            }
        }

        public TopImageAcquisitionContext MeasureContext { get; set; }

        private TopographyMeasureTools _tools;

        public IEnumerable<NanoTopoAcquisitionResolution> Resolutions { get; private set; }
        public IEnumerable<string> Algos { get; private set; }

        public override bool ArePositionsOnDie
        {
            get
            {
                return !(RecipeMeasure.WaferMap is null);
            }
        }

        private bool _useROI = false;

        public bool UseROI
        {
            get => _useROI;
            set
            {
                RecipeMeasure.DisplayROI = value;
                if (_useROI != value)
                {
                    _useROI = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        private Size _roiSize = Size.Empty;

        public Size RoiSize
        {
            get
            {
                return _roiSize;
            }
            set
            {
                if (_roiSize != value)
                {
                    _roiSize = value;
                    RecipeMeasure.RoiSize = RoiSize;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<string, double> _lightsIntensities;

        public Dictionary<string, double> LightsIntensities
        {
            get
            {
                if (_lightsIntensities is null)
                    _lightsIntensities = new Dictionary<string, double>();
                return _lightsIntensities;
            }
            set { if (_lightsIntensities != value) { _lightsIntensities = value; OnPropertyChanged(); } }
        }

        private void LightsSupervisor_LightsChangedEvent(string lightID, double intensity)
        {
            // if not editing AutoFocus
            if (!AutoFocusSettings.IsEditing)
            {
                UpdateLightIntensities();
                IsModified = true;
            }
        }

        private void UpdateLightIntensities()
        {
            foreach (var light in ServiceLocator.LightsSupervisor.LightsUp)
            {
                if (LightsIntensities.ContainsKey(light.DeviceID))
                    LightsIntensities[light.DeviceID] = light.Intensity;
                else
                    LightsIntensities.Add(light.DeviceID, light.Intensity);
            }
        }

        #endregion Properties

        private ObjectiveConfig GetObjectiveFromId(string deviceId)
        {
            return ServiceLocator.CamerasSupervisor.Objectives.FirstOrDefault(o => o.DeviceID == deviceId);
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            // We update the measure context
            MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            var newTopographySettings = TopographySettingsVM.TopographySettingsMapper.Map<TopographySettings>(this);

            newTopographySettings.NbOfRepeat = 1;

            newTopographySettings.MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            newTopographySettings.CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;

            newTopographySettings.ScanMargin = ScanMargin;

            newTopographySettings.HeightVariation = HeightVariation;

            newTopographySettings.ObjectiveId = SelectedObjective.DeviceID;

            newTopographySettings.SurfacesInFocus = SurfaceInFocus;

            if (UseROI)
                newTopographySettings.ROI = RoiHelpers.GetCenteredRegionOfInterest(RoiSize);
            if ((!(AutoFocusSettings is null)) && AutoFocusSettings.IsAutoFocusEnabled)
            {
                newTopographySettings.AutoFocusSettings = AutoFocusSettings.GetAutoFocusSettings();
            }
            else
            {
                newTopographySettings.AutoFocusSettings = null;
            }

            if ((PostProcessingSettings.IsEnabled))
            {
                newTopographySettings.PostProcessingSettings = new PostProcessingSettings();
                TopographySettingsVM.TopographySettingsMapper.Map<PostProcessingSettingsVM, PostProcessingSettings>(PostProcessingSettings, newTopographySettings.PostProcessingSettings);
            }

            return newTopographySettings;
        }

        public override void Dispose()
        {
            if (!(AutoFocusSettings is null))
            {
                _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                AutoFocusSettings.Dispose();
            }

            if (!(PostProcessingSettings is null))
            {
                _postProcessingSettings.PostProcessingSettingsModified -= PostProcessingSettings_Modified;
                _postProcessingSettings.Dispose();
            }
        }

        private void PostProcessingSettings_Modified(object sender, EventArgs e)
        {
            IsModified = true;
        }

        private bool _postProcessingIsAvailable;

        public bool PostProcessingIsAvailable
        {
            get => _postProcessingIsAvailable; set { if (_postProcessingIsAvailable != value) { _postProcessingIsAvailable = value; OnPropertyChanged(); } }
        }

        public override async Task PrepareToDisplayAsync()
        {
            // Enable Lights
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;

            _tools = ((TopographyMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(new TopographySettings())?.Result);
            if (Objectives is null && _tools != null)
            {
                PostProcessingIsAvailable = _tools.PostProcessingIsAvailable;
                PostProcessingSettings.MountainsConfig = ClassLocator.Default.GetInstance<MountainsConfiguration>();
                if (Objectives is null)
                {
                    Objectives = new ObservableCollection<ObjectiveConfig>();
                    foreach (var objectiveID in _tools.CompatibleObjectives)
                    {
                        var existingObjective = ServiceLocator.CamerasSupervisor.Objectives.FirstOrDefault(o => o.DeviceID == objectiveID);
                        if (!(existingObjective is null))
                            Objectives.Add(existingObjective);
                    }
                }
                IsModified = false;
            }

            if (MeasureContext?.TopObjectiveContext?.ObjectiveId != null)
            {
                SelectedObjective = GetObjectiveFromId(MeasureContext.TopObjectiveContext.ObjectiveId);
            }
            else
            {
                SelectedObjective = Objectives.FirstOrDefault();
            }

            ServiceLocator.ContextSupervisor.Apply(MeasureContext);

            ServiceLocator.ProbesSupervisor.CurrentProbe = ServiceLocator.ProbesSupervisor.ProbeLiseUp;

            // to avoid warning CS1998
            await Task.Delay(1);

            _isDisplayed = true;
            IsModified = false;
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is TopographyPointResult topoPointResult))
            {
                return;
            }

            if (topoPointResult.Datas.LastOrDefault() is TopographyPointData topoPointData && topoPointData.ResultImageFileName != null)
            {
                PostProcessingSettings.Latest3DAPath = Path.Combine(resultFolderPath, topoPointData.ResultImageFileName);
            }

            var resultDisplayVM = new TopographyResultDisplayVM(topoPointResult, (TopographySettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);
            ServiceLocator.DialogService.ShowDialog<TopographyResultDisplay>(resultDisplayVM);
        }

        public override void Hide()
        {
            AutoFocusSettings.IsEditing = false;
            SelectedObjective = null;
            _isDisplayed = false;
        }

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {
            if (_autoFocusSettings.IsEditing)
            {
                ValidationErrorMessage = "Autofocus must be validated";
                return false;
            }

            if ((!forTestOnPoint) && (measurePoints == null || measurePoints.Count == 0))
            {
                ValidationErrorMessage = "Measure points are not defined";
                return false;
            }

            var measureSupervisor = ServiceLocator.MeasureSupervisor;
            var topoMeasureConfig = (MeasureTopoConfiguration)measureSupervisor.GetMeasureConfiguration(MeasureType.Topography)?.Result;
            if (topoMeasureConfig?.VSIMarginConstant == null)
            {
                ValidationErrorMessage = "VSI Margin constant cannot be null";
                return false;
            }

            var AxesSupervisor = ServiceLocator.AxesSupervisor;
            var piezoAxisConfig = AxesSupervisor.GetAxesConfiguration()?.Result.AxisConfigs.Find(_ => _.AxisID == SelectedObjective?.PiezoAxisID);
            if (piezoAxisConfig == null)
            {
                ValidationErrorMessage = "Piezo axis config cannot be null";
                return false;
            }

            // Check if the height variation can be measured
            var heightFactor = 1;
            if (!_autoFocusSettings.IsAutoFocusEnabled || SurfaceInFocus == SurfacesInFocus.Unknown)
                heightFactor = 2;

            var marginConstant = topoMeasureConfig.VSIMarginConstant?.Micrometers * 2;
            var piezoMaxAmplitude = Math.Abs(piezoAxisConfig.PositionMax.Micrometers - piezoAxisConfig.PositionMin.Micrometers);
            if (HeightVariation.Micrometers * heightFactor + 2.0 * ScanMargin.Micrometers + marginConstant > piezoMaxAmplitude)
            {
                ValidationErrorMessage = "The height amplitude is too high";
                return false;
            }

            if (!PostProcessingIsAvailable)
            {
                ValidationErrorMessage = "Post Processing is not available, please check the configuration file";
                return false;
            }

            if (!PostProcessingSettings.IsEnabled)
            {
                ValidationErrorMessage = "Post Processing needs to be enabled";
                return false;
            }
            else
            {
                if (PostProcessingSettings.Template == null)
                {
                    ValidationErrorMessage = "A template must be chosen";
                    return false;
                }
                else
                {
                    if (!PostProcessingSettings.Outputs.Any(output => output.IsUsed))
                    {
                        ValidationErrorMessage = "At least 1 result output needs to be checked";
                        return false;
                    }
                }
            }

            bool postProcOutputNamesAreDistinct = PostProcessingSettings.Outputs.Select(output => output.Name).Distinct().Count() == PostProcessingSettings.Outputs.Count();

            if (!postProcOutputNamesAreDistinct)
            {
                ValidationErrorMessage = "All post processing output names must be distinct";
                return false;
            }

            ValidationErrorMessage = string.Empty;
            return true;
        }

        public override void DisplaySettingsTab()
        {
            RecipeMeasure.IsCenteredRoi = true;
            RecipeMeasure.DisplayROI = UseROI;
            if (UseROI)
            {
                RecipeMeasure.RoiSize = RoiSize;
            }

            // Used to retrieve the modifications of the ROI
            RecipeMeasure.PropertyChanged += RecipeMeasure_PropertyChanged;
        }

        private void RecipeMeasure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RecipeMeasure.RoiSize))
            {
                RoiSize = RecipeMeasure.RoiSize;
            }
        }

        public override void HideSettingsTab()
        {
            RecipeMeasure.PropertyChanged -= RecipeMeasure_PropertyChanged;
        }

        #region PatternRec objectives

        public override ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            return SelectedObjective;
        }

        #endregion
    }
}
