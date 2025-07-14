using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using UnitySC.PM.ANA.Client.CommonUI.View.Help;
using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.Help;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class EdgeTrimSettingsVM : MeasureSettingsVM
    {
        public static IMapper EdgeTrimSettingsMapper;
        static EdgeTrimSettingsVM()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EdgeTrimSettings, EdgeTrimSettingsVM>().ReverseMap();
                cfg.CreateMap<AutoFocusSettings, AutoFocusSettingsVM>().ReverseMap();
                cfg.CreateMap<ResultCorrectionAnyUnitSettings, ResultCorrectionAnyUnitSettingsVM>().ReverseMap();
                cfg.CreateMap<ResultCorrectionSettings, ResultCorrectionSettingsVM>().ReverseMap();

            });
            EdgeTrimSettingsMapper = configuration.CreateMapper();
        }

        public EdgeTrimSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            var edgeTrimMeasureConfig = (MeasureEdgeTrimConfiguration)(ServiceLocator.MeasureSupervisor.GetMeasureConfiguration(MeasureType.EdgeTrim)?.Result);
            if (edgeTrimMeasureConfig != null)
            {
                HeightCorrection.CorrectionType = edgeTrimMeasureConfig.CorrectionTypeForHeight;
                WidthCorrection.CorrectionType = edgeTrimMeasureConfig.CorrectionTypeForWidth;
            }
            RecipeMeasure = recipeMeasure;
            AutoFocusSettings = new AutoFocusSettingsVM();
            AutoFocusSettings.Type = AutoFocusType.Camera;
            Objectives = new ObservableCollection<ObjectiveConfig>();

            UpdateScanSize();

            HeightCorrection.PropertyChanged += Correction_PropertyChanged;
            WidthCorrection.PropertyChanged += Correction_PropertyChanged;

            recipeMeasure.MeasurePoints.CustomPointsManagement = new EdgeTrimCustomPointsManagementVM(recipeMeasure.MeasurePoints);
            recipeMeasure.MeasurePoints.CanAddPointsWithImages = false;
        }

        private void Correction_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CharacteristicsChanged = true;
        }

        public override async Task LoadSettingsAsync(MeasureSettingsBase measureSettings)
        {
            // If a loading was already in progress, we wait
            if (!SpinWait.SpinUntil(() => !IsLoading, 20000))
            {
                ClassLocator.Default.GetInstance<ILogger>().Error("EdgeTrim Loading failed");
                return;
            }

            if (!(measureSettings is EdgeTrimSettings edgeTrimSettings))
                return;

            IsLoading = true;

            HeightTarget= edgeTrimSettings.HeightTarget;
            HeightCorrection.Offset = edgeTrimSettings.HeightCorrection?.Offset ?? 0.Micrometers();
            HeightCorrection.Coef = edgeTrimSettings.HeightCorrection?.Coef ?? 1;
            HeightTolerance=edgeTrimSettings.HeightTolerance;
            WidthTarget=edgeTrimSettings.WidthTarget;
            IsWidthMeasured = edgeTrimSettings.IsWidthMeasured;
            WidthCorrection.Offset = edgeTrimSettings.WidthCorrection?.Offset ?? 0.Micrometers();
            WidthCorrection.Coef = edgeTrimSettings.WidthCorrection?.Coef ?? 1;
            WidthTolerance=edgeTrimSettings.WidthTolerance ;
            PreEdgeScanSize=edgeTrimSettings.PreEdgeScanSize??20.Micrometers() ;
            PostEdgeScanSize = edgeTrimSettings.PostEdgeScanSize??80.Micrometers();
            TopEdgeExclusionSize = edgeTrimSettings.TopEdgeExclusionSize??0.Micrometers();
            BottomEdgeExclusionSize = edgeTrimSettings.BottomEdgeExclusionSize??0.Micrometers();
            StepSize =edgeTrimSettings.StepSize;

            if (edgeTrimSettings.AutoFocusSettings is null)
            {
                AutoFocusSettings = new AutoFocusSettingsVM();
                AutoFocusSettings.Type = AutoFocusType.Camera;
            }
            else
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(edgeTrimSettings.AutoFocusSettings);
                AutoFocusSettings.EnableWithoutEditing();
            }
            AutoFocusSettings.AreSettingsVisible = false;

            UpdateCompatibleMeasureTools();
            ProbeSelector.SelectedProbeId = edgeTrimSettings.ProbeSettings.ProbeId;
            ProbeSelector.SetProbeSettings(edgeTrimSettings.ProbeSettings);

            LightsIntensities.Clear();
            if (!(edgeTrimSettings.MeasureContext is null))
            {
                foreach (var lightIntensity in edgeTrimSettings.MeasureContext.Lights.Lights)
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

        private bool _isWidthMeasured = true;

        public bool IsWidthMeasured
        {
            get => _isWidthMeasured;
            set { if (_isWidthMeasured != value) { _isWidthMeasured = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private Length _widthTarget = 50.Micrometers();

        public Length WidthTarget
        {
            get
            {
                if (_widthTarget is null)
                    _widthTarget = new Length(0,LengthUnit.Micrometer);
                return _widthTarget;
            }
            set { if (_widthTarget != value) { _widthTarget = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _widthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance WidthTolerance
        {
            get => _widthTolerance; 
            set { if (_widthTolerance != value) { _widthTolerance = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        
        private ResultCorrectionSettingsVM _widthCorrection;

        public ResultCorrectionSettingsVM WidthCorrection
        {
            get
            {
                if (_widthCorrection is null)
                    _widthCorrection = new ResultCorrectionSettingsVM();
                return _widthCorrection;
            }
            set { if (_widthCorrection != value) { _widthCorrection = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }


        private Length _heightTarget = 10.Micrometers();

        public Length HeightTarget
        {
            get
            {
                if (_heightTarget is null)
                    _heightTarget = new Length(0, LengthUnit.Micrometer);
                return _heightTarget;
            }
            set { if (_heightTarget != value) { _heightTarget = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _heightTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance HeightTolerance
        {
            get => _heightTolerance; 
            set { if (_heightTolerance != value) { _heightTolerance = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }


        private ResultCorrectionSettingsVM _heightCorrection;

        public ResultCorrectionSettingsVM HeightCorrection
        {
            get
            {
                if (_heightCorrection is null)
                    _heightCorrection = new ResultCorrectionSettingsVM();
                return _heightCorrection;
            }
            set { if (_heightCorrection != value) { _heightCorrection = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }
   
 
        private ObservableCollection<ObjectiveConfig> _objectives;

        public ObservableCollection<ObjectiveConfig> Objectives
        {
            get => _objectives; 
            set => SetProperty(ref _objectives, value);
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
            get => _characteristicsChanged; 
            set => SetProperty(ref _characteristicsChanged, value);
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

        public TopImageAcquisitionContext MeasureContext { get; set; }

        public override bool ArePositionsOnDie
        {
            get
            {
                return false;
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
            set => SetProperty(ref _lightsIntensities, value);
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

        private EdgeTrimMeasureTools _measureTools;

        public EdgeTrimMeasureTools MeasureTools
        {
            get => _measureTools;
            set
            {
                if (_measureTools != value)
                {
                    _measureTools = value;
                    UpdateProbes();
                    OnPropertyChanged();
                }
            }
        }

        private ProbeSelectorVM _probeSelector = new ProbeSelectorVM();

        public ProbeSelectorVM ProbeSelector
        {
            get => _probeSelector; 
            set => SetProperty(ref _probeSelector, value);
        }

        private bool _isEditingHardware = false;

        public bool IsEditingHardware
        {
            get => _isEditingHardware;
            set => SetProperty(ref _isEditingHardware, value);
        }

        private bool _isScanSizeChangedByUser = false;

        private Length _preEdgeScanSize = 20.Micrometers();

        public Length PreEdgeScanSize
        {
            get => _preEdgeScanSize;
            set
            {
                if (SetProperty(ref _preEdgeScanSize, value))
                {
                    _isScanSizeChangedByUser = true;
                    IsModified = true;
                    OnPropertyChanged(nameof(TotalScanSize));
                }
            }
        }

        private Length _postEdgeScanSize = 80.Micrometers();

        public Length PostEdgeScanSize
        {
            get => _postEdgeScanSize;
            set
            {
                if (SetProperty(ref _postEdgeScanSize, value))
                {
                    _isScanSizeChangedByUser = true;
                    IsModified = true;
                    OnPropertyChanged(nameof(TotalScanSize));
                }
            }
        }

        public Length TotalScanSize => PreEdgeScanSize + PostEdgeScanSize;

        private Length _topEdgeExclusionSize = 0.Micrometers();

        public Length TopEdgeExclusionSize
        {
            get => _topEdgeExclusionSize;
            set
            {
                if (SetProperty(ref _topEdgeExclusionSize, value))
                {
                    IsModified = true;
                }
            }
        }

        private Length _bottomEdgeExclusionSize = 0.Micrometers();

        public Length BottomEdgeExclusionSize
        {
            get => _bottomEdgeExclusionSize;
            set
            {
                if (SetProperty(ref _bottomEdgeExclusionSize, value))
                {
                    _isScanSizeChangedByUser = true;
                    IsModified = true;
                }
            }
        }

        private Length _stepSize = 10.Micrometers();

        public Length  StepSize
        {
            get => _stepSize;
            set
            {
                if (SetProperty(ref _stepSize, value))
                {
                    IsModified = true;
                }
            }
        }

        #endregion Properties

        #region Commands

        private AutoRelayCommand _displayMeasureSettingsHelpCommand;

        public AutoRelayCommand DisplayMeasureSettingsHelpCommand
        {
            get
            {
                return _displayMeasureSettingsHelpCommand ?? (_displayMeasureSettingsHelpCommand = new AutoRelayCommand(
                    () =>
                    {
                        var helpImageDisplay=new HelpImageDisplayVM("pack://application:,,,/UnitySC.PM.ANA.Client.CommonUI;component/Styles/Help Images/EdgeScanHelp.png");
                       
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<HelpImageDisplay>(helpImageDisplay);
                    },
                    () => { return true; }
                ));
            }
        }



        private AutoRelayCommand _submitCharacteristics;

        public AutoRelayCommand SubmitCharacteristics
        {
            get
            {
                return _submitCharacteristics ?? (_submitCharacteristics = new AutoRelayCommand(
                    () =>
                    {
                        CharacteristicsChanged = false;
                        UpdateCompatibleMeasureTools();
                        UpdateScanSize();
                        IsModified = true;
                    },
                    () => { return CharacteristicsChanged; }
                ));
            }
        }

        private void UpdateScanSize()
        {
            var defaultScanSizeWidthFactor = 2.5;
            if (_isScanSizeChangedByUser)
                return;

            if (IsWidthMeasured)
            {
                _preEdgeScanSize = defaultScanSizeWidthFactor * WidthTarget*0.2;
                _postEdgeScanSize = defaultScanSizeWidthFactor * WidthTarget * 0.8;
            }
            else
            {
                _preEdgeScanSize = 10.Micrometers();
                _postEdgeScanSize = 90.Micrometers();
            }

            OnPropertyChanged(nameof(PreEdgeScanSize));
            OnPropertyChanged(nameof(PostEdgeScanSize));
            OnPropertyChanged(nameof(TotalScanSize));
        }

        private AutoRelayCommand _startEditHardware;

        public AutoRelayCommand StartEditHardware
        {
            get
            {
                return _startEditHardware ?? (_startEditHardware = new AutoRelayCommand(
                    () =>
                    {
                        IsEditingHardware = true;
                        ProbeSelector.IsEditing = true;
                        ServiceLocator.ProbesSupervisor.IsEditingProbe = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _submitHardware;

        public AutoRelayCommand SubmitHardware
        {
            get
            {
                return _submitHardware ?? (_submitHardware = new AutoRelayCommand(
                    () =>
                    {
                        IsEditingHardware = false;
                        ProbeSelector.IsEditing = false;
                        ServiceLocator.ProbesSupervisor.IsEditingProbe = false;

                        IsModified = true;
                    },
                    () => { return ProbeSelector.SelectedProbe?.IsCalibrationInProgress == false; }
                ));
            }
        }

        #endregion Commands

        private ObjectiveConfig GetObjectiveFromId(string deviceId)
        {
            return ServiceLocator.CamerasSupervisor.Objectives.FirstOrDefault(o => o.DeviceID == deviceId);
        }

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            // We update the measure context
            MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            var newEdgeTrimSettings = EdgeTrimSettingsVM.EdgeTrimSettingsMapper.Map<EdgeTrimSettings>(this);

            newEdgeTrimSettings.NbOfRepeat = 1;

            newEdgeTrimSettings.MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;

            newEdgeTrimSettings.HeightTarget = HeightTarget;
            newEdgeTrimSettings.HeightCorrection = HeightCorrection.GetResultCorrectionSetting();
            newEdgeTrimSettings.HeightTolerance = HeightTolerance;
            newEdgeTrimSettings.WidthTarget = WidthTarget;
            newEdgeTrimSettings.IsWidthMeasured = IsWidthMeasured;
            newEdgeTrimSettings.WidthCorrection = WidthCorrection.GetResultCorrectionSetting();
            newEdgeTrimSettings.WidthTolerance = WidthTolerance;
            newEdgeTrimSettings.PreEdgeScanSize = PreEdgeScanSize;
            newEdgeTrimSettings.PostEdgeScanSize = PostEdgeScanSize;
            newEdgeTrimSettings.TopEdgeExclusionSize = TopEdgeExclusionSize;
            newEdgeTrimSettings.BottomEdgeExclusionSize = BottomEdgeExclusionSize;
            newEdgeTrimSettings.StepSize = StepSize;

            newEdgeTrimSettings.ProbeSettings= ProbeSelector.GetSelectedProbeSettings() ?? new ProbeSettings();

            if ((!(AutoFocusSettings is null)) && AutoFocusSettings.IsAutoFocusEnabled)
            {
                newEdgeTrimSettings.AutoFocusSettings = AutoFocusSettings.GetAutoFocusSettings();
            }
            else
            {
                newEdgeTrimSettings.AutoFocusSettings = null;
            }

            return newEdgeTrimSettings;
        }

        public override void Dispose()
        {
            if (!(AutoFocusSettings is null))
            {
                _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                AutoFocusSettings.Dispose();
            }
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

            //_tools = ((EdgeTrimMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(new EdgeTrimSettings())?.Result);
  

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

            IsModified = false;
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is EdgeTrimPointResult edgeTrimPointResult))
                return;

            var edgeTrimPointData = edgeTrimPointResult.Datas.LastOrDefault() as EdgeTrimPointData;
            if (edgeTrimPointData is null)
            {
                return;
            }

            var resultDisplayVM = new EdgeTrimResultDisplayVM(edgeTrimPointResult, (EdgeTrimSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);
            ServiceLocator.DialogService.ShowDialog<EdgeTrimResultDisplay>(resultDisplayVM);
        }

        public override void Hide()
        {
            AutoFocusSettings.IsEditing = false;
            SelectedObjective = null;
        }

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {
            if (CharacteristicsChanged)
            {
                ValidationErrorMessage = "Characteristics must be validated";
                return false;
            }

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

            if (IsWidthMeasured && (TotalScanSize < WidthTarget))
            {
                ValidationErrorMessage = "The total scan size is not long enough to measure the width";
                return false;
            }

            if (StepSize > TotalScanSize/2)
            {
                ValidationErrorMessage = "The step size is too long";
                return false;
            }

            if (TopEdgeExclusionSize + BottomEdgeExclusionSize > TotalScanSize)
            {
                ValidationErrorMessage = "The exclusions are too long";
                return false;
            }

            if (ProbeSelector.SelectedProbe is null)
            {
                ValidationErrorMessage = "A probe must be selected";
                return false;
            }

            if (ProbeSelector.IsEditing)
            {
                ValidationErrorMessage = "Hardware must be validated";
                return false;
            }
            ValidationErrorMessage = string.Empty;
            return true;
        }

        private void UpdateCompatibleMeasureTools()
        {
            MeasureTools = ((EdgeTrimMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(GetSettingsWithoutPoints())?.Result);
        }

        private void UpdateProbes()
        {
            foreach (var compatibleProbe in MeasureTools.Probes)
            {
                ProbeSelector.AddProbe(compatibleProbe);
            }

            // Remove the probes that are not anymore compatible
            foreach (var probe in ProbeSelector.Probes.ToList())
            {
                if (!MeasureTools.Probes.Any(p => p.ProbeId == probe.ProbeMaterial.ProbeId))
                {
                    probe.Dispose();
                    ProbeSelector.Probes.Remove(probe);
                }
            }
        }

        public override void DisplaySettingsTab()
        {
            RecipeMeasure.DisplayROI = false;

            // Used to retrieve the modifications of the ROI
            RecipeMeasure.PropertyChanged += RecipeMeasure_PropertyChanged;
        }

        private void RecipeMeasure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        public override void HideSettingsTab()
        {
            RecipeMeasure.PropertyChanged -= RecipeMeasure_PropertyChanged;
        }

        #region PatternRec objectives

        public override ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            return ProbeSelector.SelectedProbe?.SelectedObjective;
        }

        #endregion
    }
}
