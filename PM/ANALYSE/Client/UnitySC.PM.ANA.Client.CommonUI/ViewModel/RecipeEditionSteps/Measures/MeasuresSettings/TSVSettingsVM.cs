using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Client.Proxy.Helpers;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.ANA.Client.Proxy.Axes;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings
{
    public class TSVSettingsVM : MeasureSettingsVM
    {
        private bool _isDisplayed = false;

        private readonly MeasureTSVConfiguration _tsvMeasureConfig;
        private readonly ReferentialSupervisor _referentialSupervisor;
        private readonly AxesSupervisor _axesSupervisor;

        public TSVSettingsVM(RecipeMeasureVM recipeMeasure)
        {
            RecipeMeasure = recipeMeasure;

            Objectives = new ObservableCollection<ObjectiveConfig>();
            AutoFocusSettings = new AutoFocusSettingsVM();

            _referentialSupervisor = ClassLocator.Default.GetInstance<ReferentialSupervisor>();
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();

            _tsvMeasureConfig = (MeasureTSVConfiguration)(ServiceLocator.MeasureSupervisor.GetMeasureConfiguration(MeasureType.TSV)?.Result);
            if (_tsvMeasureConfig != null)
            {
                DepthCorrection.CorrectionType = _tsvMeasureConfig.CorrectionTypeForDepth;
                WidthCorrection.CorrectionType = _tsvMeasureConfig.CorrectionTypeForCDWidth;
                LengthCorrection.CorrectionType = _tsvMeasureConfig.CorrectionTypeForCDLength;
                DColTSVDepthLabel = _tsvMeasureConfig.DColTSVDepthDefaultLabel;
                DColTSVCDWidthLabel = _tsvMeasureConfig.DColTSVCDWidthDefaultLabel;
                DColTSVCDLengthLabel = _tsvMeasureConfig.DColTSVCDLengthDefaultLabel;
                CanChangeDColLabels = _tsvMeasureConfig.CanChangeDColLabels;
            }

            DepthCorrection.PropertyChanged += Correction_PropertyChanged;
            WidthCorrection.PropertyChanged += Correction_PropertyChanged;
            LengthCorrection.PropertyChanged += Correction_PropertyChanged;
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
                ClassLocator.Default.GetInstance<ILogger>().Error("TSV Loading failed");
                return;
            }

            if (!(measureSettings is TSVSettings))
            {

                return;
            }

            IsLoading = true;

            var tsvSettings = measureSettings as TSVSettings;

            DepthTarget = tsvSettings.DepthTarget;
            DepthCorrection.Offset = tsvSettings.DepthCorrection?.Offset ?? 0.Micrometers();
            DepthCorrection.Coef = tsvSettings.DepthCorrection?.Coef ?? 1;
            DepthTolerance = tsvSettings.DepthTolerance;
            LengthTarget = tsvSettings.LengthTarget;
            LengthCorrection.Offset = tsvSettings.LengthCorrection?.Offset ?? 0.Micrometers();
            LengthCorrection.Coef = tsvSettings.LengthCorrection?.Coef ?? 1;
            LengthTolerance = tsvSettings.LengthTolerance;
            WidthTarget = tsvSettings.WidthTarget;
            WidthCorrection.Offset = tsvSettings.WidthCorrection?.Offset ?? 0.Micrometers();
            WidthCorrection.Coef = tsvSettings.WidthCorrection?.Coef ?? 1;
            WidthTolerance = tsvSettings.WidthTolerance;
            ShapeDetectionMode = tsvSettings.ShapeDetectionMode;
            if (!string.IsNullOrEmpty(tsvSettings.DColTSVDepthLabel))
                DColTSVDepthLabel = tsvSettings.DColTSVDepthLabel;
            else
                DColTSVDepthLabel = (_tsvMeasureConfig is null) ? "TSV Depth" : _tsvMeasureConfig.DColTSVDepthDefaultLabel;

            if (!string.IsNullOrEmpty(tsvSettings.DColTSVCDWidthLabel))
                DColTSVCDWidthLabel = tsvSettings.DColTSVCDWidthLabel;
            else
                DColTSVCDWidthLabel = (_tsvMeasureConfig is null) ? "TSV CD Width" : _tsvMeasureConfig.DColTSVCDWidthDefaultLabel;

            if (!string.IsNullOrEmpty(tsvSettings.DColTSVCDLengthLabel))
                DColTSVCDLengthLabel = tsvSettings.DColTSVCDLengthLabel;
            else
                DColTSVCDLengthLabel = (_tsvMeasureConfig is null) ? "TSV CD Length" : _tsvMeasureConfig.DColTSVCDLengthDefaultLabel;

            if (tsvSettings.ROI is null)
                UseROI = false;
            else
            {
                UseROI = true;
                RoiSize = RoiHelpers.GetSizeInPixels(tsvSettings.ROI, tsvSettings.MeasureContext.TopObjectiveContext.ObjectiveId);

            }
            Shape = tsvSettings.Shape;

            if (tsvSettings.AutoFocusSettings is null)
            {
                AutoFocusSettings = new AutoFocusSettingsVM();
            }
            else
            {
                AutoFocusSettings = AutoFocusSettingsVM.CreateFromAutoFocusSettings(tsvSettings.AutoFocusSettings);

                AutoFocusSettings.EnableWithoutEditing();
            }

            AutoFocusSettings.AreSettingsVisible = false;

            UpdateCompatibleMeasureTools();
            ProbeSelector.SelectedProbeId = tsvSettings.Probe.ProbeId;
            ProbeSelector.SetProbeSettings(tsvSettings.Probe);

            LightsIntensities.Clear();
            if (!(tsvSettings.MeasureContext is null))
            {
                foreach (var lightIntensity in tsvSettings.MeasureContext.Lights.Lights)
                {
                    LightsIntensities.Add(lightIntensity.DeviceID, lightIntensity.Intensity);
                }
            }
            if (_isDisplayed)
            {
                ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
                await ApplyLightSettings();
                ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;
            }
            CharacteristicsChanged = false;
            UpdateDepthInLiseHF();
            IsLoading = false;
            IsModified = false;
        }

        #region Properties

        public RecipeMeasureVM RecipeMeasure { get; set; }

        public IEnumerable<string> ToleranceUnits { get; private set; }

        private List<LayerSettings> _physicalLayers = null;
        private Length _depthTarget = 10.Micrometers();

        public Length DepthTarget
        {
            get => _depthTarget; set { if (_depthTarget != value) { _depthTarget = value; CharacteristicsChanged = true; UpdateGraphBand(); OnPropertyChanged(); } }
        }
  
        private ResultCorrectionSettingsVM _depthCorrection;

        public ResultCorrectionSettingsVM DepthCorrection
        {
            get
            {
                if (_depthCorrection is null)
                    _depthCorrection = new ResultCorrectionSettingsVM();
                return _depthCorrection;
            }
            set { if (_depthCorrection != value) { _depthCorrection = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }
 
        private LengthTolerance _depthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance DepthTolerance
        {
            get => _depthTolerance; set { if (_depthTolerance != value) { _depthTolerance = value; CharacteristicsChanged = true; UpdateGraphBand(); OnPropertyChanged(); } }
        }

        private Length _lengthTarget = 10.Micrometers();

        public Length LengthTarget
        {
            get => _lengthTarget; set { if (_lengthTarget != value) { _lengthTarget = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private ResultCorrectionSettingsVM _lengthCorrection;

        public ResultCorrectionSettingsVM LengthCorrection
        {
            get
            {
                if (_lengthCorrection is null)
                    _lengthCorrection = new ResultCorrectionSettingsVM();
                return _lengthCorrection;
            }
            set { if (_lengthCorrection != value) { _lengthCorrection = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private LengthTolerance _lengthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance LengthTolerance
        {
            get => _lengthTolerance; set { if (_lengthTolerance != value) { _lengthTolerance = value; CharacteristicsChanged = true; OnPropertyChanged(); } }
        }

        private Length _widthTarget = 10.Micrometers();

        public Length WidthTarget
        {
            get => _widthTarget;
            set
            {
                if (_widthTarget != value)
                {
                    _widthTarget = value;
                    CharacteristicsChanged = true;
                    if (Shape == TSVShape.Circle)
                    {
                        LengthTarget = value;
                    }
                    OnPropertyChanged();
                }
            }
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

        private LengthTolerance _widthTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance WidthTolerance
        {
            get => _widthTolerance;
            set
            {
                if (_widthTolerance != value)
                {
                    _widthTolerance = value;
                    CharacteristicsChanged = true;
                    if (Shape == TSVShape.Circle)
                    {
                        LengthTolerance = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private TSVShape _shape = TSVShape.Circle;

        public TSVShape Shape
        {
            get => _shape;
            set
            {
                if (_shape != value)
                {
                    _shape = value;
                    CharacteristicsChanged = true;
                    if (_shape == TSVShape.Circle)
                    {
                        LengthTarget = WidthTarget;
                        LengthTolerance = WidthTolerance;
                        CanSelectShapeDetectionMode = true;
                    }
                    else
                    {
                        CanSelectShapeDetectionMode = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private TSVMeasureTools _measureTools;

        public TSVMeasureTools MeasureTools
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
            get => _probeSelector; set { if (_probeSelector != value) { _probeSelector = value; OnPropertyChanged(); } }
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

        private bool _characteristicsChanged = true;

        public bool CharacteristicsChanged
        {
            get => _characteristicsChanged; set { if (_characteristicsChanged != value) { _characteristicsChanged = value; OnPropertyChanged(); } }
        }

        private ShapeDetectionModes _shapeDetectionMode = ShapeDetectionModes.Central;

        public ShapeDetectionModes ShapeDetectionMode
        {
            get => _shapeDetectionMode;
            set
            {
                if (_shapeDetectionMode != value)
                {
                    _shapeDetectionMode = value;
                    IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        private bool _canSelectShapeDetectionMode= true;

        public bool CanSelectShapeDetectionMode
        {
            get => _canSelectShapeDetectionMode;
            set => SetProperty(ref _canSelectShapeDetectionMode, value);
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

        public override bool ArePositionsOnDie
        {
            get
            {
                return !(RecipeMeasure.WaferMap is null);
            }
        }

        private string _dColTSVDepthLabel;

        public string DColTSVDepthLabel
        {
            get => _dColTSVDepthLabel; set { if (_dColTSVDepthLabel != value) { _dColTSVDepthLabel = value; IsModified = true;  OnPropertyChanged(); } }
        }

        private string _dColTSVCDWidthLabel;

        public string DColTSVCDWidthLabel
        {
            get => _dColTSVCDWidthLabel; set { if (_dColTSVCDWidthLabel != value) { _dColTSVCDWidthLabel = value; IsModified = true; OnPropertyChanged(); } }
        }

        private string _dColTSVCDLengthLabel;

        public string DColTSVCDLengthLabel
        {
            get => _dColTSVCDLengthLabel; set { if (_dColTSVCDLengthLabel != value) { _dColTSVCDLengthLabel = value; IsModified = true; OnPropertyChanged(); } }
        }

        private bool _canChangeDColLabels;

        public bool CanChangeDColLabels
        {
            get => _canChangeDColLabels; set { if (_canChangeDColLabels != value) { _canChangeDColLabels = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region Commands

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
                        UpdateCurrentProbeAndObjective();
                        UpdateDepthInLiseHF();
                        IsModified = true;
                    },
                    () => { return CharacteristicsChanged; }
                ));
            }
        }

        private void UpdateDepthInLiseHF()
        {
            if (ProbeSelector.SelectedProbe is SelectableLiseHFVM SelectedProbeLiseHF)
            {
                SelectedProbeLiseHF.ProbeInputParameters.DepthTarget = DepthTarget;
                SelectedProbeLiseHF.ProbeInputParameters.DepthTolerance = DepthTolerance;
            }
        }

        private AutoRelayCommand _startEditHardware;

        public AutoRelayCommand StartEditHardware
        {
            get
            {
                return _startEditHardware ?? (_startEditHardware = new AutoRelayCommand(
                    () =>
                    {
                        ChangeStageReferentialSettingsAndMove(true);
                        UpdateDepthInLiseHF();
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
                        ChangeStageReferentialSettingsAndMove(false);
                        ProbeSelector.IsEditing = false;
                        ServiceLocator.ProbesSupervisor.IsEditingProbe = false;

                        IsModified = true;

                    },
                    () => { return ProbeSelector.SelectedProbe?.IsCalibrationInProgress == false; }
                ));
            }
        }

        #endregion Commands

        public override MeasureSettingsBase GetSettingsWithoutPoints()
        {
            var newTSVSettings = new TSVSettings();
            newTSVSettings.NbOfRepeat = 1;
            newTSVSettings.CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
            newTSVSettings.WidthTarget = WidthTarget;
            newTSVSettings.WidthCorrection.Offset = WidthCorrection.Offset;
            newTSVSettings.WidthCorrection.Coef = WidthCorrection.Coef;
            newTSVSettings.WidthTolerance = WidthTolerance;
            newTSVSettings.LengthTarget = LengthTarget;
            newTSVSettings.LengthCorrection.Offset = Shape==TSVShape.Circle? WidthCorrection.Offset:LengthCorrection.Offset;
            newTSVSettings.LengthCorrection.Coef = Shape == TSVShape.Circle ? WidthCorrection.Coef : LengthCorrection.Coef;
            newTSVSettings.LengthTolerance = LengthTolerance;
            newTSVSettings.DepthTarget = DepthTarget;
            newTSVSettings.DepthCorrection.Offset = DepthCorrection.Offset;
            newTSVSettings.DepthCorrection.Coef = DepthCorrection.Coef;
            newTSVSettings.DepthTolerance = DepthTolerance;
            newTSVSettings.EllipseDetectionTolerance = LengthTarget ?? 0.Micrometers() / 5.0;
            newTSVSettings.DColTSVDepthLabel = DColTSVDepthLabel;
            newTSVSettings.DColTSVCDWidthLabel = DColTSVCDWidthLabel;
            newTSVSettings.DColTSVCDLengthLabel = DColTSVCDLengthLabel;
            newTSVSettings.Shape = Shape;
            newTSVSettings.ShapeDetectionMode = ShapeDetectionMode;
            newTSVSettings.MeasureContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;
            newTSVSettings.Probe = ProbeSelector.GetSelectedProbeSettings() ?? new ProbeSettings();
            newTSVSettings.PhysicalLayers = LayersHelper.GetPhysicalLayers(RecipeMeasure.EditedRecipe.Step.Id);

            if (newTSVSettings.PhysicalLayers != null && newTSVSettings.PhysicalLayers.Count > 0)
                _physicalLayers = newTSVSettings.PhysicalLayers.ToList();   
            else
                _physicalLayers = null;
            UpdateGraphBand();

            if (UseROI)
                newTSVSettings.ROI = RoiHelpers.GetCenteredRegionOfInterest(RoiSize);
            if ((!(AutoFocusSettings is null)) && AutoFocusSettings.IsAutoFocusEnabled)
            {
                newTSVSettings.AutoFocusSettings = AutoFocusSettings.GetAutoFocusSettings();
            }
            
            return newTSVSettings;
        }

        public void ChangeStageReferentialSettingsAndMove(bool applyProbeOffset)
        {
            var position = _axesSupervisor.GetCurrentPosition().Result;
            position = ServiceLocator.ReferentialSupervisor.ConvertTo(position, ReferentialTag.Stage)?.Result;
            _referentialSupervisor.SetSettings(new StageReferentialSettings() { EnableProbeSpotOffset = applyProbeOffset });
            _axesSupervisor.GotoPosition(position, PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Normal);
        }

        public override void Dispose()
        {
            foreach (var probe in ProbeSelector.Probes)
            {
                probe.Dispose();
            }

            if (!(AutoFocusSettings is null))
            {
                _autoFocusSettings.AutoFocusSettingsModified -= AutoFocusSettings_Modified;
                AutoFocusSettings.Dispose();
            }

            DepthCorrection.PropertyChanged -= Correction_PropertyChanged;
            WidthCorrection.PropertyChanged -= Correction_PropertyChanged;
            LengthCorrection.PropertyChanged -= Correction_PropertyChanged;
        }

        public override async Task PrepareToDisplayAsync()
        {
            // Enable Lights
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;

            await ApplyLightSettings();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UpdateCurrentProbeAndObjective();

                ProbeSelector.IsEditing = false;
            }));

            ServiceLocator.LightsSupervisor.LightsAreLocked = false;
            ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;
     
            _isDisplayed = true;
        }

        private void UpdateCurrentProbeAndObjective()
        {
            if (ProbeSelector.SelectedProbe?.SelectedObjective != null)
            {
                ServiceLocator.CamerasSupervisor.Objective = ProbeSelector.SelectedProbe.SelectedObjective;
            }
            else
            {
                ServiceLocator.CamerasSupervisor.Objective = ServiceLocator.CamerasSupervisor.MainObjective;
            }

            if (ProbeSelector.SelectedProbe?.ProbeMaterial?.ProbeId != null)
            {
                if (ProbeSelector.SelectedProbe.SelectedObjective != null)
                {
                    ServiceLocator.ProbesSupervisor.SetObjectiveToUse(ProbeSelector.SelectedProbe.SelectedObjective.DeviceID);
                }

                ServiceLocator.ProbesSupervisor.SetCurrentProbe(ProbeSelector.SelectedProbe.ProbeMaterial.ProbeId);
            }
            else
            {
                ServiceLocator.ProbesSupervisor.SetCurrentProbe(ServiceLocator.ProbesSupervisor.ProbeLiseUp.DeviceID);
            }

            _selectedObjective = ServiceLocator.CamerasSupervisor.Objective;
        }

        private async Task ApplyLightSettings()
        {
            foreach (var light in LightsIntensities.ToList())
            {
                await ServiceLocator.LightsSupervisor.SetLightIntensityAsync(light.Key, light.Value);
            }
        }

        public override void DisplayTestResult(MeasurePointResult result, string resultFolderPath, DieIndex dieIndex)
        {
            if (!(result is TSVPointResult tsvPointResult))
                return;

            var tsvResultDisplayVM = new TSVResultDisplayVM(tsvPointResult, (TSVSettings)GetSettingsWithoutPoints(), resultFolderPath, dieIndex);

            ServiceLocator.DialogService.ShowDialog<TSVResultDisplay>(tsvResultDisplayVM);
        }

        public override void Hide()
        {
            _referentialSupervisor.SetSettings(new StageReferentialSettings() { EnableProbeSpotOffset = false });
            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
            RecipeMeasure.PropertyChanged -= RecipeMeasure_PropertyChanged;
            AutoFocusSettings.IsEditing = false;
            _isDisplayed = false;
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

        public override bool AreSettingsValid(ObservableCollection<MeasurePointVM> measurePoints, bool forTestOnPoint = false)
        {
            if (CharacteristicsChanged)
            {
                ValidationErrorMessage = "Characteristics must be validated";
                return false;
            }

            if (ProbeSelector.IsEditing)
            {
                ValidationErrorMessage = "Hardware must be validated";
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

            ValidationErrorMessage = string.Empty;
            return true;
        }

        private void UpdateCompatibleMeasureTools()
        {
            MeasureTools = ((TSVMeasureTools)ServiceLocator.MeasureSupervisor.GetMeasureTools(GetSettingsWithoutPoints())?.Result);
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

        private void UpdateGraphBand()
        {
            if (_physicalLayers != null && _physicalLayers[0].Thickness < DepthTarget)
            {
                var passedLayersThichness = new Length(0.0, DepthTarget.Unit);
                foreach (var layer in _physicalLayers)
                {
                    if ((passedLayersThichness + layer.Thickness) > DepthTarget)
                        break;
                    passedLayersThichness += layer.Thickness;
                }
                GraphBandBegin = ((DepthTarget - passedLayersThichness) - DepthTolerance.GetAbsoluteTolerance(DepthTarget)).Micrometers;
            }
            else
                GraphBandBegin = (DepthTarget - DepthTolerance.GetAbsoluteTolerance(DepthTarget)).Micrometers;

            GraphBandEnd = (DepthTarget + DepthTolerance.GetAbsoluteTolerance(DepthTarget)).Micrometers;
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
            if (e.PropertyName==nameof(RecipeMeasure.RoiSize)) 
            {
                RoiSize=RecipeMeasure.RoiSize;
            }
        }

        #region PatternRec objectives

        public override void HideSettingsTab()
        {
            RecipeMeasure.PropertyChanged -= RecipeMeasure_PropertyChanged;
        }

        public override ObjectiveConfig GetObjectiveUsedByMeasure()
        {
            return ServiceLocator.CamerasSupervisor.Objective;
        }

        public override void SetObjectiveUsedByMeasure()
        {
            if (ProbeSelector.SelectedProbe != null)
            {
                ServiceLocator.CamerasSupervisor.Objective = ProbeSelector.SelectedProbe.SelectedObjective;
            }
        }

        #endregion
    }
}
