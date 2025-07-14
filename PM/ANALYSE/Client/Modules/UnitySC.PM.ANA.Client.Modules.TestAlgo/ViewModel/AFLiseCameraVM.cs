using System;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class AFLiseCameraVM : AlgoBaseVM, IDisposable
    {
        public AFLiseCameraVM() : base("Autofocus")
        {
            ClassLocator.Default.GetInstance<AlgosSupervisor>().AutoFocusChangedEvent += AFLiseCameraVM_AutofocusChangedEvent;
            UseCurrentZPosition = true;
            CanDoAutofocus = false;
        }

        public void AutoFocusType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AutoFocusSettings.IsAutoFocusEnabled))
            {
                AutofocusResult = null;
                StartAF.NotifyCanExecuteChanged();
            }
        }

        private AutofocusResult _autofocusResult;

        public AutofocusResult AutofocusResult
        {
            get => _autofocusResult; set { if (_autofocusResult != value) { _autofocusResult = value; OnPropertyChanged(nameof(AutofocusResult)); } }
        }

        private AutoRelayCommand _startAF;

        public AutoRelayCommand StartAF
        {
            get
            {
                return _startAF ?? (_startAF = new AutoRelayCommand(
                () => startAutofocus(),
                () => AutoFocusSettings.IsAutoFocusEnabled));
            }
        }

        private void startAutofocus()
        {
            if (AutoFocusSettings.Type == AutoFocusType.Camera || AutoFocusSettings.Type == AutoFocusType.LiseAndCamera)
            {
                if (!ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.IsGrabbing)
                {
                    ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.StartStreaming();
                }
            }

            AutofocusResult = null;
            IsBusy = true;

            var autofocusInput = CreateAutoFocusInput();
            ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAutoFocus(autofocusInput);
        }

        internal AutoFocusSettings GetAutoFocusSettings()
        {
            var autoFocusConfig = new AutoFocusSettings();
            autoFocusConfig.CameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
            autoFocusConfig.ProbeId = ServiceLocator.ProbesSupervisor.ProbeLiseUp.DeviceID;
            autoFocusConfig.Type = AutoFocusSettings.Type;
            autoFocusConfig.LiseOffsetX = AutoFocusSettings.LiseOffsetX;
            autoFocusConfig.LiseOffsetY = AutoFocusSettings.LiseOffsetY;
            autoFocusConfig.LiseGain = AutoFocusSettings.LiseGain;
            autoFocusConfig.CameraScanRange = AutoFocusSettings.CameraScanRange;
            autoFocusConfig.UseCurrentZPosition = UseCurrentZPosition;

            // Set camera objective
            var topImageAcquisitionContext = ServiceLocator.ContextSupervisor.GetTopImageAcquisitionContext()?.Result;
            if (topImageAcquisitionContext != null)
            {
                topImageAcquisitionContext.TopObjectiveContext = new TopObjectiveContext(AutoFocusSettings.CameraObjective.DeviceID);
                autoFocusConfig.ImageAutoFocusContext = topImageAcquisitionContext;
            }

            // Set lise objective
            autoFocusConfig.LiseAutoFocusContext = new TopObjectiveContext(AutoFocusSettings.LiseObjective.DeviceID);
            return autoFocusConfig;
        }

        internal AutofocusInput CreateAutoFocusInput()
        {
            AutoFocusSettings settings = GetAutoFocusSettings();
            XYPosition currentPosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result.ToXYPosition();
            XYPositionContext context = new XYPositionContext(currentPosition);
            return new AutofocusInput(settings, context);
        }

        private void AFLiseCameraVM_AutofocusChangedEvent(AutofocusResult afCameraResult)
        {
            UpdateAutofocusResult(afCameraResult);
        }

        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () => ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAutoFocus(),
              () => true));
            }
        }

        public void UpdateAutofocusResult(AutofocusResult afResult)
        {
            AutofocusResult = afResult;
            if (afResult.Status.IsFinished)
                IsBusy = false;
        }

        private AutoFocusSettingsVM _autoFocusSettings;

        public AutoFocusSettingsVM AutoFocusSettings
        {
            get
            {
                return _autoFocusSettings ?? (_autoFocusSettings = new AutoFocusSettingsVM());
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
                    _autoFocusSettings.AutoFocusSettingsModified += AutoFocusSettings_Modified;
                    OnPropertyChanged();
                }
            }
        }

        private void AutoFocusSettings_Modified(object sender, EventArgs e)
        {
            IsModified = true;
        }

        private bool _isModified;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }

        private bool _useCurrentZPosition;

        public bool UseCurrentZPosition
        {
            get => _useCurrentZPosition; set { if (_useCurrentZPosition != value) { _useCurrentZPosition = value; OnPropertyChanged(nameof(UseCurrentZPosition)); } }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AutoFocusSettings.Dispose();
            }
            AutofocusResult = null;
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
