using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class NanoTopoSettingsVM : MeasureSettingsVM
    {
        public static IMapper NanoTopoSettingsMapper;
        private bool _isDisplayed = false;

        static NanoTopoSettingsVM()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NanoTopoSettings, NanoTopoSettingsVM>().ReverseMap();
                cfg.CreateMap<AutoFocusSettings, AutoFocusSettingsVM>().ReverseMap();
                cfg.CreateMap<PostProcessingSettings, PostProcessingSettingsVM>().ReverseMap();
                cfg.CreateMap<PostProcessingOutput, PostProcessingOutputVM>().ReverseMap();
                cfg.CreateMap<ResultCorrectionAnyUnitSettings, ResultCorrectionAnyUnitSettingsVM>().ReverseMap();
            });
            NanoTopoSettingsMapper = configuration.CreateMapper();
        }

        public NanoTopoSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            RecipeMeasure = recipeMeasure;
            AutoFocusSettings = new AutoFocusSettingsVM();
            PostProcessingSettings = new PostProcessingSettingsVM();
            AutoFocusSettings.Type = AutoFocusType.Camera;
        }

        public override async Task LoadSettingsAsync(MeasureSettingsBase measureSettings)
        {
            // If a loading was already in progress, we wait
            if (!SpinWait.SpinUntil(() => !IsLoading, 20000))
            {
                ClassLocator.Default.GetInstance<ILogger>().Error("NanoTopo Loading failed");
                return;
            }


            if (!(measureSettings is NanoTopoSettings nanoTopoSettings))
                return;

            IsLoading = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                NanoTopoSettingsVM.NanoTopoSettingsMapper.Map<NanoTopoSettings, NanoTopoSettingsVM>(nanoTopoSettings, this);
            }));


            if (RoughnessTarget is null)
            {
                IsCharacteristicRoughness = false;
                RoughnessTarget = 10.Nanometers();
                RoughnessTolerance = new LengthTolerance(1, LengthToleranceUnit.Nanometer);
            }
            else
            {
                //IsCharacteristicRoughness = true;
                IsCharacteristicRoughness = false; // while view is collapsed do not activate rougthness measure -- comment above to restore when view will be restore again
            }

            if (StepHeightTarget is null)
            {
                IsCharacteristicStepHeight = false;
                StepHeightTarget = 10.Nanometers();
                StepHeightTolerance = new LengthTolerance(1, LengthToleranceUnit.Nanometer);
            }
            else
            {
                //IsCharacteristicStepHeight = true;
                IsCharacteristicStepHeight = false;// while view is collapsed do not activate rougthness measure -- comment above to restore when view will be restore again
            }

            if (nanoTopoSettings.ROI is null)
                UseROI = false;
            else
            {
                UseROI = true;
                RoiSize = RoiHelpers.GetSizeInPixels(nanoTopoSettings.ROI, nanoTopoSettings.MeasureContext.TopObjectiveContext.ObjectiveId);
            }

            if (nanoTopoSettings.AutoFocusSettings is null)
                AutoFocusSettings = new AutoFocusSettingsVM();
            else
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(nanoTopoSettings.AutoFocusSettings);
                AutoFocusSettings.EnableWithoutEditing();
            }

            AutoFocusSettings.AreSettingsVisible = false;

            if (nanoTopoSettings.PostProcessingSettings is null)
                PostProcessingSettings = new PostProcessingSettingsVM();

            if (!(MeasureContext is null))
            {
                var redLightContext = MeasureContext.Lights.Lights.FirstOrDefault(l => l.DeviceID == _redLightID);
                RedLightIntensity = redLightContext?.Intensity ?? 0;
            }

            if (_isDisplayed)
            {
                ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
                await ApplyLightSettingsAsync();
                ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;
            }
            CharacteristicsChanged = false;
            IsModified = false;
            IsLoading = false;
        }

        #region Properties

        private NanoTopoAcquisitionResolution _resolution = NanoTopoAcquisitionResolution.Medium;

        public NanoTopoAcquisitionResolution Resolution
        {
            get => _resolution; set { if (_resolution != value) { _resolution = value; IsModified = true; OnPropertyChanged(); } }
        }

        private string _algoName = string.Empty;

        public string AlgoName
        {
            get => _algoName; set { if (_algoName != value) { _algoName = value; IsModified = true; OnPropertyChanged(); } }
        }

        private bool _isCarachteristicRoughness = false;

        public bool IsCharacteristicRoughness
        {
            get => _isCarachteristicRoughness;
            set
            {
                if (_isCarachteristicRoughness != value)
                {
                    _isCarachteristicRoughness = value;
                    CharacteristicsChanged = true;
                    IsModified = true;
                    OnPropertyChanged();
                }
                if (_isCarachteristicRoughness)
                    IsShapeRemoval = true;
            }
        }

        private Length _roughnessTarget = 10.Nanometers();

        public Length RoughnessTarget
        {
            get => _roughnessTarget; set { if (_roughnessTarget != value) { _roughnessTarget = value; CharacteristicsChanged = true; IsModified = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _roughnessTolerance = new LengthTolerance(1, LengthToleranceUnit.Nanometer);

        public LengthTolerance RoughnessTolerance
        {
            get => _roughnessTolerance; set { if (_roughnessTolerance != value) { _roughnessTolerance = value; CharacteristicsChanged = true; IsModified = true; OnPropertyChanged(); } }
        }

        private bool _isCarachteristicStepHeight = false;

        public bool IsCharacteristicStepHeight
        {
            get => _isCarachteristicStepHeight;
            set
            {
                if (_isCarachteristicStepHeight != value)
                {
                    _isCarachteristicStepHeight = value;
                    CharacteristicsChanged = true;
                    OnPropertyChanged();
                    IsModified = true;
                }
                if (_isCarachteristicStepHeight)
                    IsShapeRemoval = true;
            }
        }

        private Length _stepHeightTarget = 10.Nanometers();

        public Length StepHeightTarget
        {
            get => _stepHeightTarget; set { if (_stepHeightTarget != value) { _stepHeightTarget = value; CharacteristicsChanged = true; IsModified = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _stepHeightTolerance = new LengthTolerance(1, LengthToleranceUnit.Nanometer);

        public LengthTolerance StepHeightTolerance
        {
            get => _stepHeightTolerance; set { if (_stepHeightTolerance != value) { _stepHeightTolerance = value; CharacteristicsChanged = true; IsModified = true; OnPropertyChanged(); } }
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
            //_recipeMeasure.SettingsAreValid = !_autoFocusSettings.IsEditing;
        }

        private double _redLightIntensity = 3;

        public double RedLightIntensity
        {
            get => _redLightIntensity; set { if (_redLightIntensity != value) { _redLightIntensity = value; IsModified = true; OnPropertyChanged(); } }
        }

        public TopImageAcquisitionContext MeasureContext { get; set; }

        private NanoTopoMeasureTools _tools;

        public IEnumerable<NanoTopoAcquisitionResolution> Resolutions { get; private set; }
        public IEnumerable<string> Algos { get; private set; }

        private List<string> _measureLightsId;

        public List<string> MeasureLightsId
        {
            get
            {
                if (_measureLightsId is null)
                    _measureLightsId = ServiceLocator.MeasureSupervisor.GetMeasureLightIds(MeasureType.NanoTopo)?.Result;
                return _measureLightsId;
            }
        }

        // This measure uses only the red light
        private string _redLightID => MeasureLightsId.FirstOrDefault();

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

        #endregion Properties

        private ObjectiveConfig GetObjectiveFromId(string deviceId)
        {
            return ServiceLocator.CamerasSupervisor.Objectives.FirstOrDefault(o => o.DeviceID == deviceId);
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            // We update the measure context
            MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            var newNanoTopoSettings = NanoTopoSettingsVM.NanoTopoSettingsMapper.Map<NanoTopoSettings>(this);

            newNanoTopoSettings.NbOfRepeat = 1;

            newNanoTopoSettings.CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;

            newNanoTopoSettings.ObjectiveId = SelectedObjective.DeviceID;

            newNanoTopoSettings.MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            if (!IsCharacteristicRoughness)
            {
                newNanoTopoSettings.RoughnessTarget = null;
                newNanoTopoSettings.RoughnessTolerance = null;
            }

            if (!IsCharacteristicStepHeight)
            {
                newNanoTopoSettings.StepHeightTarget = null;
                newNanoTopoSettings.StepHeightTolerance = null;
            }

            if (UseROI)
                newNanoTopoSettings.ROI = RoiHelpers.GetCenteredRegionOfInterest(RoiSize);
            if ((!(AutoFocusSettings is null)) && AutoFocusSettings.IsAutoFocusEnabled)
            {
                newNanoTopoSettings.AutoFocusSettings = AutoFocusSettings.GetAutoFocusSettings();
            }
            else
            {
                newNanoTopoSettings.AutoFocusSettings = null;
            }

            newNanoTopoSettings.LightId = _redLightID;

            if ((PostProcessingSettings.IsEnabled))
            {
                newNanoTopoSettings.PostProcessingSettings = new PostProcessingSettings();
                NanoTopoSettingsVM.NanoTopoSettingsMapper.Map<PostProcessingSettingsVM, PostProcessingSettings>(PostProcessingSettings, newNanoTopoSettings.PostProcessingSettings);
            }
            return newNanoTopoSettings;
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
            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
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
            if (Resolutions is null)
            {
                Resolutions = Enum.GetValues(typeof(NanoTopoAcquisitionResolution)).Cast<NanoTopoAcquisitionResolution>();
            }

            if ((Algos is null) || (Objectives is null))
            {
                _tools = ((NanoTopoMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(new NanoTopoSettings())?.Result);
                PostProcessingIsAvailable = _tools?.PostProcessingIsAvailable ?? false;
                PostProcessingSettings.MountainsConfig = ClassLocator.Default.GetInstance<MountainsConfiguration>();
                if (Algos is null && _tools != null)
                {
                    Algos = _tools.OrderedAlgoNames;
                    if (string.IsNullOrEmpty(AlgoName))
                        AlgoName = Algos.First();
                }
                if (Objectives is null && _tools != null)
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
            ServiceLocator.ProbesSupervisor.CurrentProbe = null;

            await ApplyLightSettingsAsync();
            ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;

            _isDisplayed = true;
            IsModified = false;
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is NanoTopoPointResult ntpPointResult))
                return;

            var resultDisplayVM = new NanoTopoResultDisplayVM(ntpPointResult, (NanoTopoSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);
            ServiceLocator.DialogService.ShowDialog<NanoTopoResultDisplay>(resultDisplayVM);
        }

        private async Task ApplyLightSettingsAsync()
        {
            if (!(MeasureLightsId is null))
            {
                foreach (var light in ServiceLocator.LightsSupervisor.Lights)
                {
                    var lightUsedForMeasure = _measureLightsId.Find(l => l == light.DeviceID);
                    if (lightUsedForMeasure is null)
                    {
                        await ServiceLocator.LightsSupervisor.SetLightIntensityAsync(light.DeviceID, 0);
                        light.IsLocked = true;
                    }
                    else
                    {
                        await ServiceLocator.LightsSupervisor.SetLightIntensityAsync(light.DeviceID, RedLightIntensity);
                        light.IsLocked = false;
                    }
                }
            }
        }

        public override void Hide()
        {
            AutoFocusSettings.IsEditing = false;
            SelectedObjective = null;
            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
            _isDisplayed = false;
        }

        private void LightsSupervisor_LightsChangedEvent(string lightID, double intensity)
        {
            // if not editing AutoFocus
            if (!AutoFocusSettings.IsEditing)
            {
                if (lightID == _redLightID)
                {
                    RedLightIntensity = intensity;
                }
                IsModified = true;
            }
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

            if (PostProcessingSettings.IsEnabled)
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
