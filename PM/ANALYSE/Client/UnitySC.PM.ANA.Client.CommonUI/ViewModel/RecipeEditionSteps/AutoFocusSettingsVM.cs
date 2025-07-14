using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using AutoMapper;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public enum AvailableAutoFocus
    {
        Camera,
        Lise,
        CameraAndLise
    }

    public class AutoFocusLight : ObservableObject
    {
        #region Fields

        private string _deviceID = null;
        private double _intensity = 0;
        private string _name = null;

        #endregion Fields

        #region Properties

        public string DeviceID
        {
            get => _deviceID; set { if (_deviceID != value) { _deviceID = value; OnPropertyChanged(); } }
        }

        public double Intensity
        {
            get => _intensity; set { if (_intensity != value) { _intensity = value; OnPropertyChanged(); } }
        }

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        #endregion Properties
    }

    public class AutoFocusSettingsVM : StepBaseVM, IDisposable
    {
        #region events

        public event EventHandler AutoFocusSettingsModified;

        #endregion events

        #region Fields

        public static IMapper AutofocusMapper;
        private Dictionary<string, bool> _lightsLockStatus = new Dictionary<string, bool>();
        private Dictionary<string, double> _previousLightsIntensities = new Dictionary<string, double>();
        private ObjectiveConfig _previousObjective;

        #endregion Fields

        #region Public Constructors

        static AutoFocusSettingsVM()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AutoFocusSettings, AutoFocusSettingsVM>().ReverseMap();
            });
            AutofocusMapper = configuration.CreateMapper();
        }

        public AutoFocusSettingsVM()
        {
            CameraScanRanges = Enum.GetValues(typeof(ScanRangeType)).Cast<ScanRangeType>().Where(e => !e.Equals(ScanRangeType.Configured));

            var cameras = ServiceLocator.CamerasSupervisor.Cameras;

            var liseObjectiveSelectorID = (ServiceLocator.ProbesSupervisor.ProbeLiseUp.Configuration as ProbeConfigurationLiseVM).Cameras.FirstOrDefault()?.ObjectivesSelectorID;
            if (!(liseObjectiveSelectorID is null))
            {
                LiseObjectives = ServiceLocator.ProbesSupervisor.ObjectivesSelectors.FirstOrDefault(os => os.DeviceID == liseObjectiveSelectorID).Objectives.Where(o => o.ObjType == ObjectiveConfig.ObjectiveType.NIR);
            }
            _liseObjective = LiseObjectives.FirstOrDefault();

            var cameraObjectiveSelectorID = ServiceLocator.CamerasSupervisor.GetMainCamera()?.Configuration?.ObjectivesSelectorID;
            if (!(cameraObjectiveSelectorID is null))
            {
                CameraObjectives = ServiceLocator.ProbesSupervisor.ObjectivesSelectors.FirstOrDefault(os => os.DeviceID == ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.ObjectivesSelectorID).Objectives;
            }
            _cameraObjective = CameraObjectives.FirstOrDefault();

            AutoFocusLights = new ObservableCollection<AutoFocusLight>();
            ServiceLocator.AlgosSupervisor.AutoFocusChangedEvent += AutoFocusChangedEvent;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Dispose()
        {
            IsAutoFocusEnabled = false;
            ServiceLocator.AlgosSupervisor.AutoFocusChangedEvent -= AutoFocusChangedEvent;
            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
        }

        internal void EnableWithoutEditing()
        {
            _isAutoFocusEnabled = true;
            OnPropertyChanged(nameof(IsAutoFocusEnabled));
        }

        #endregion Public Methods

        #region Internal Methods

        internal static AutoFocusSettingsVM CreateFromAutoFocusSettings(AutoFocusSettings autoFocus)
        {
            if (autoFocus is null)
                return null;
            var newAutoConfigurationsVM = AutoFocusSettingsVM.AutofocusMapper.Map<AutoFocusSettingsVM>(autoFocus);

            if (!(autoFocus.ImageAutoFocusContext is null))
            {
                // Update the lights
                foreach (var light in ServiceLocator.LightsSupervisor.Lights)
                {
                    var contextLight = autoFocus.ImageAutoFocusContext.Lights.Lights.Find(l => l.DeviceID == light.DeviceID);
                    if (!(contextLight is null) && (contextLight.Intensity != 0))
                    {
                        var newLight = CreateLight(contextLight.DeviceID, light.Name, light.Position, contextLight.Intensity);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            newAutoConfigurationsVM.AutoFocusLights.Add(newLight);
                        }));
                    }
                }
            }

            if (!(autoFocus.LiseAutoFocusContext is null))
            {
                newAutoConfigurationsVM._liseObjective = newAutoConfigurationsVM.LiseObjectives.FirstOrDefault(o => o.DeviceID == autoFocus.LiseAutoFocusContext.ObjectiveId);
            }

            if (autoFocus.ImageAutoFocusContext is TopImageAcquisitionContext autoFocusTopImageAcquisitionContext)
            {
                newAutoConfigurationsVM._cameraObjective = newAutoConfigurationsVM.CameraObjectives.FirstOrDefault(o => o.DeviceID == autoFocusTopImageAcquisitionContext.TopObjectiveContext.ObjectiveId);
            }

            return newAutoConfigurationsVM;
        }

        #endregion Internal Methods

        #region Private Methods

        private static AutoFocusLight CreateLight(string deviceID, string name, ModulePositions position, double intensity)
        {
            var newLight = new AutoFocusLight();
            newLight.Intensity = intensity;
            newLight.DeviceID = deviceID;
            newLight.Name = (position == ModulePositions.Up) ? "TOP - " + name : "BOTTOM - " + name;
            return newLight;
        }

        private void ApplyAFLightsIntesities()
        {
            foreach (var light in ServiceLocator.LightsSupervisor.Lights)
            {
                var lightUsedInAF = AutoFocusLights.FirstOrDefault(l => l.DeviceID == light.DeviceID);

                ServiceLocator.LightsSupervisor.SetLightIntensity(light.DeviceID, (lightUsedInAF is null) ? 0 : lightUsedInAF.Intensity);
            }

            if (AutoFocusLights.Count == 0)
            {
                ServiceLocator.LightsSupervisor.SetLightIntensity(ServiceLocator.LightsSupervisor.GetMainLight().DeviceID, ServiceLocator.RecipeManager.EditedRecipe?.Alignment?.AutoLight?.LightIntensity ?? 2);
            }
        }

        private void ApplyAFObjectives()
        {
            switch (Type)
            {
                case AutoFocusType.LiseAndCamera:
                case AutoFocusType.Camera:
                    ServiceLocator.CamerasSupervisor.Objective = CameraObjective;
                    break;

                case AutoFocusType.Lise:
                    ServiceLocator.CamerasSupervisor.Objective = LiseObjective;
                    break;

                default:
                    break;
            }
        }

        private void AutoFocusChangedEvent(AutofocusResult autoFocusResult)
        {
            if (autoFocusResult.Status.IsFinished)
            {
                if (autoFocusResult.Status.State == FlowState.Success)
                {
                    StepState = StepStates.Done;
                    TestScore = (int)(autoFocusResult.QualityScore * 100);
                    ResultZPosition = autoFocusResult.ZPosition;
                }
                else
                {
                    StepState = StepStates.Error;
                    ErrorMessage = autoFocusResult.Status.Message;
                }
                IsTestInProgress = false;
            }
        }

        private void LightsSupervisor_LightsChangedEvent(string lightID, double intensity)
        {
            UpdateLights();
        }

        private void UpdateLights()
        {
            var curPosition = 0;
            foreach (var light in ServiceLocator.LightsSupervisor.Lights)
            {
                if (light.Intensity == 0)
                {
                    var lightToRemove = AutoFocusLights.FirstOrDefault(l => l.DeviceID == light.DeviceID);
                    if (!(lightToRemove is null))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            AutoFocusLights.Remove(lightToRemove);
                        }));
                    }
                }
                else
                {
                    var existingLight = AutoFocusLights.FirstOrDefault(l => l.DeviceID == light.DeviceID);
                    if (existingLight is null)
                    {
                        var newLight = CreateLight(light.DeviceID, light.Name, light.Position, light.Intensity);

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            AutoFocusLights.Insert(curPosition, newLight);
                        }));
                    }
                    else
                    {
                        existingLight.Intensity = light.Intensity;
                    }
                    curPosition++;
                }
            }

            if (!IsTestInProgress)
            {
                StepState = StepStates.NotDone;
            }
        }

        #endregion Private Methods

        #region Properties

        private ScanRangeType _cameraScanRange = ScanRangeType.Medium;
        private bool _isAutoFocusEnabled = false;
        private bool _areSettingsVisible = false;
        private bool _isEditing = false;
        private bool _isTestInProgress = false;
        private double _liseGain = 1.8;
        private Length _liseOffsetX = 0.Micrometers();
        private Length _liseOffsetY = 0.Micrometers();
        private double _resultZPosition = 0;
        private double _score = 0;
        private double _testScore = 0;

        private AutoFocusType _type = AutoFocusType.Lise;

        public ObservableCollection<AutoFocusLight> AutoFocusLights { get; private set; }

        public ScanRangeType CameraScanRange
        {
            get => _cameraScanRange; set { if (_cameraScanRange != value) { _cameraScanRange = value; StepState = StepStates.NotDone; OnPropertyChanged(); } }
        }

        public IEnumerable<ScanRangeType> CameraScanRanges { get; private set; }

        public IEnumerable<ObjectiveConfig> CameraObjectives { get; private set; }

        private ObjectiveConfig _cameraObjective = null;

        public ObjectiveConfig CameraObjective
        {
            get => _cameraObjective;
            set
            {
                if (_cameraObjective != value)
                {
                    _cameraObjective = value;
                    OnPropertyChanged();
                    if (!(_cameraObjective is null) && ((Type == AutoFocusType.LiseAndCamera) || (Type == AutoFocusType.Camera)))
                        ServiceLocator.CamerasSupervisor.Objective = _cameraObjective;
                }
            }
        }

        public IEnumerable<ObjectiveConfig> LiseObjectives { get; private set; }

        private ObjectiveConfig _liseObjective = null;

        public ObjectiveConfig LiseObjective
        {
            get => _liseObjective;
            set
            {
                if (_liseObjective != value)
                {
                    _liseObjective = value;
                    OnPropertyChanged();
                    if (!(_liseObjective is null) && ((Type == AutoFocusType.LiseAndCamera) || (Type == AutoFocusType.Lise)))
                        ServiceLocator.CamerasSupervisor.Objective = _liseObjective;
                }
            }
        }

        public bool AreSettingsVisible
        {
            get => _areSettingsVisible;
            set
            {
                if (_areSettingsVisible != value)
                {
                    _areSettingsVisible = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    if (_isEditing)
                    {
                        StartEdition();
                    }
                    else
                    {
                        StopEdition();
                    }
                    OnPropertyChanged();
                }
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
                    if (_isModified)
                    {
                        AutoFocusSettingsModified?.Invoke(this, null);
                    }
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAutoFocusEnabled
        {
            get => _isAutoFocusEnabled;
            set
            {
                if (_isAutoFocusEnabled != value)
                {
                    _isAutoFocusEnabled = value;
                    if (_isAutoFocusEnabled)
                    {
                        IsEditing = true;
                        AreSettingsVisible = true;
                    }
                    else
                    {
                        IsEditing = false;
                        AreSettingsVisible = false;
                        IsModified = true;
                    }

                    OnPropertyChanged();
                }
            }
        }

        public bool IsTestInProgress
        {
            get => _isTestInProgress; set { if (_isTestInProgress != value) { _isTestInProgress = value; OnPropertyChanged(); } }
        }

        public double LiseGain
        {
            get => _liseGain; set { if (_liseGain != value) { _liseGain = value; StepState = StepStates.NotDone; OnPropertyChanged(); } }
        }

        public Length LiseOffsetX
        {
            get => _liseOffsetX; set { if (_liseOffsetX != value) { _liseOffsetX = value; StepState = StepStates.NotDone; OnPropertyChanged(); } }
        }

        public Length LiseOffsetY
        {
            get => _liseOffsetY; set { if (_liseOffsetY != value) { _liseOffsetY = value; StepState = StepStates.NotDone; OnPropertyChanged(); } }
        }

        public double ResultZPosition
        {
            get => _resultZPosition; set { if (_resultZPosition != value) { _resultZPosition = value; OnPropertyChanged(); } }
        }

        public double Score
        {
            get => _score; set { if (_score != value) { _score = value; OnPropertyChanged(); } }
        }

        public double TestScore
        {
            get => _testScore; set { if (_testScore != value) { _testScore = value; OnPropertyChanged(); } }
        }

        public AutoFocusType Type
        {
            get => _type; set { if (_type != value) { _type = value; StepState = StepStates.NotDone; OnPropertyChanged(); } }
        }

        internal AutoFocusSettings GetAutoFocusSettings()
        {
            if (!IsAutoFocusEnabled)
                return null;
            var autoFocusConfig = AutofocusMapper.Map<AutoFocusSettingsVM, AutoFocusSettings>(this);

            if ((autoFocusConfig.Type == AutoFocusType.Camera) || (autoFocusConfig.Type == AutoFocusType.LiseAndCamera))
            {
                autoFocusConfig.CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
                var topImageAcquisitionContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;
                if (topImageAcquisitionContext != null)
                {
                    // Set up lights that are not part of the autofocus at 0 intensity.
                    foreach (var light in topImageAcquisitionContext.Lights.Lights)
                    {
                        var autoFocusLight = AutoFocusLights.FirstOrDefault(l => light.DeviceID == l.DeviceID);
                        if (autoFocusLight is null)
                            light.Intensity = 0;
                        else
                            light.Intensity = autoFocusLight.Intensity;
                    }

                    if (!(CameraObjective is null))
                    {
                        // Set up the camera objective.
                        topImageAcquisitionContext.TopObjectiveContext = new TopObjectiveContext(CameraObjective.DeviceID);
                    }

                    autoFocusConfig.ImageAutoFocusContext = topImageAcquisitionContext;
                }
            }

            autoFocusConfig.ProbeId = ServiceLocator.ProbesSupervisor.CurrentProbeLise.DeviceID;

            if ((autoFocusConfig.Type == AutoFocusType.Lise) || (autoFocusConfig.Type == AutoFocusType.LiseAndCamera))
            {
                if (!(LiseObjective is null))
                {
                    autoFocusConfig.LiseAutoFocusContext = new TopObjectiveContext(LiseObjective.DeviceID);
                }
            }

            autoFocusConfig.UseCurrentZPosition = true;

            return autoFocusConfig;
        }

        internal AutofocusInput CreateAutoFocusInput()
        {
            AutoFocusSettings settings = GetAutoFocusSettings();
            XYPosition currentPosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result.ToXYPosition();
            XYPositionContext context = new XYPositionContext(currentPosition);
            return new AutofocusInput(settings, context);
        }

        private void ApplyLightsLockStatus(Dictionary<string, bool> lightsLockStatus)
        {
            foreach (var lightLockStatus in lightsLockStatus)
            {
                var lightToUpdate = ServiceLocator.LightsSupervisor.Lights.Find(l => lightLockStatus.Key == l.DeviceID);
                if (!(lightToUpdate is null))
                    lightToUpdate.IsLocked = lightLockStatus.Value;
            }
        }

        private Dictionary<string, bool> GetLightsLockStatus()
        {
            var lightsLockStatus = new Dictionary<string, bool>();
            foreach (var light in ServiceLocator.LightsSupervisor.Lights)
            {
                lightsLockStatus.Add(light.DeviceID, light.IsLocked);
            }
            return lightsLockStatus;
        }

        private void RestorePreviousLightsIntensities()
        {
            // "_ =" to disable warning CS4014
            _ = Task.Run(() =>
            {
                foreach (var lightIntensity in _previousLightsIntensities.ToList())
                {
                    ServiceLocator.LightsSupervisor.SetLightIntensity(lightIntensity.Key, lightIntensity.Value);
                }
            });
        }

        private ProbeBaseVM previousCurrentProbe;

        private void StartEdition()
        {
            _lightsLockStatus = GetLightsLockStatus();
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;
            StorePreviousLightsIntensities();
            _previousObjective = ServiceLocator.CamerasSupervisor.Objective;
            ApplyAFLightsIntesities();
            ApplyAFObjectives();
            previousCurrentProbe = ServiceLocator.ProbesSupervisor.CurrentProbe;
            ServiceLocator.ProbesSupervisor.CurrentProbe = ServiceLocator.ProbesSupervisor.ProbeLiseUp;
            ServiceLocator.ProbesSupervisor.CurrentProbeLise = ServiceLocator.ProbesSupervisor.ProbeLiseUp;

            ServiceLocator.LightsSupervisor.LightsChangedEvent += LightsSupervisor_LightsChangedEvent;
        }

        private void StopEdition()
        {
            ServiceLocator.LightsSupervisor.LightsChangedEvent -= LightsSupervisor_LightsChangedEvent;
            ServiceLocator.ProbesSupervisor.CurrentProbe = previousCurrentProbe;
            ApplyLightsLockStatus(_lightsLockStatus);
            RestorePreviousLightsIntensities();
            ServiceLocator.CamerasSupervisor.Objective = _previousObjective;
        }

        private void StorePreviousLightsIntensities()
        {
            _previousLightsIntensities.Clear();
            foreach (var light in ServiceLocator.LightsSupervisor.Lights)
            {
                _previousLightsIntensities.Add(light.DeviceID, light.Intensity);
            }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _edit;

        private AutoRelayCommand _startTestAutofocus;

        private AutoRelayCommand _stopTestAutoFocus;

        private AutoRelayCommand _submit;

        private AutoRelayCommand _startAutoFocusCamera;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
              () =>
              {
                  IsEditing = true;
              },
              () => { return true; }));
            }
        }

        public AutoRelayCommand StartTestAutoFocus
        {
            get
            {
                return _startTestAutofocus ?? (_startTestAutofocus = new AutoRelayCommand(
                    () =>
                    {
                        IsTestInProgress = true;
                        StepState = StepStates.InProgress;
                        AutofocusInput autoFocusInput = CreateAutoFocusInput();
                        ServiceLocator.AlgosSupervisor.StartAutoFocus(autoFocusInput);
                    },
                    () => { return IsAutoFocusEnabled && IsEditing; }
                ));
            }
        }

        public AutoRelayCommand StopTestAutoFocus
        {
            get
            {
                return _stopTestAutoFocus ?? (_stopTestAutoFocus = new AutoRelayCommand(
                    () =>
                    {
                        ServiceLocator.AlgosSupervisor.CancelAutoFocus();
                        IsTestInProgress = false;
                        StepState = StepStates.NotDone;
                    },
                    () => { return true; }
                ));
            }
        }

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
              () =>
              {
                  IsEditing = false;
                  IsModified = true;
              },

              () => { return true; }));
            }
        }

        public AutoRelayCommand StartAutoFocusCamera
        {
            get
            {
                return _startAutoFocusCamera ?? (_startAutoFocusCamera = new AutoRelayCommand(
                    () =>
                    {
                        IsTestInProgress = true;
                        StepState = StepStates.InProgress;
                        AutofocusInput autoFocusInput = CreateAutoFocusInput();
                        autoFocusInput.Settings.UseCurrentZPosition = false;
                        ServiceLocator.AlgosSupervisor.StartAutoFocus(autoFocusInput);
                    },

                    () => { return IsAutoFocusEnabled && IsEditing; }));
            }
        }

        public AutoRelayCommand StartAutofocusForObjectiveCalibrationOnAllZAxisRange
        {
            get
            {
                return _startAutoFocusCamera ?? (_startAutoFocusCamera = new AutoRelayCommand(
                    () =>
                    {
                        IsTestInProgress = true;
                        StepState = StepStates.InProgress;
                        AutofocusInput autoFocusInput = CreateAutoFocusInput();
                        autoFocusInput.Settings.UseCurrentZPosition = true;
                        autoFocusInput.Settings.CameraScanRange = ScanRangeType.AllAxisRange;

                        // Algorithm SumOfModifiedLaplacien, works significatively well on refCam for objectives
                        // with small magnification. For objectives with high magnification, it works not
                        // significatively well but most importantly, if the image contains too few details,
                        // it doesn't works anymore.
                        if (_cameraObjective.Magnification < 20)
                        {
                            autoFocusInput.Settings.AutofocusModifiedLaplacien = true;
                        }

                        ServiceLocator.AlgosSupervisor.StartAutoFocus(autoFocusInput);
                    },

                    () => { return IsAutoFocusEnabled && IsEditing; }));
            }
        }

        #endregion RelayCommands
    }
}
