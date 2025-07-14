using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Shared;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.EME.Client.Modules.TestApps.Acquisition
{
    public class AcquisitionViewModel : ObservableRecipient
    {
        private readonly CameraBench _camera;
        private readonly IMessenger _messenger;
        private readonly string _acquisitionDirectory;


        public AcquisitionViewModel(CameraBench camera, FilterWheelBench filterWheelBench, IMessenger messenger, string acquisitionDirectory = null)
        {
            _camera = camera;
            _messenger = messenger;
            _filterWheelBench = filterWheelBench;
            _acquisitionDirectory = acquisitionDirectory
                                    ?? Path.Combine(
                                        ClassLocator.Default.GetInstance<EmeClientConfiguration>().TestAppsCapturePath,
                                        "Emera");

            _camera.PropertyChanged += CameraBench_PropertyChanged;

            Init();
        }

        private void Init()
        {
            ExposureTime = _camera.GetExposureTime();
            Gain = _camera.GetGain();
            NormalizationMax = _camera.NormalizationMax;
        }

        #region Properties

        private void CameraBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsStreaming")
            {
                IsStreaming = _camera.IsStreaming;
                NotifyStreamingChange();
            }
        }

        private void NotifyStreamingChange()
        {
            _startStreamingCommand?.NotifyCanExecuteChanged();
            _stopStreamingCommand?.NotifyCanExecuteChanged();
        }

        private FilterWheelBench _filterWheelBench;

        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }
        
        private double _exposureTime;
        public double ExposureTime
        {
            get => _exposureTime;
            set
            {
                SetProperty(ref _exposureTime, value);
                ParametersChanged = true;
            }
        }

        private double _gain;
        public double Gain
        {
            get => _gain;
            set
            {
                SetProperty(ref _gain, value);
                ParametersChanged = true;
            }
        }

        private bool _areCameraSettingsUsed;
        public bool AreCameraSettingsUsed
        {
            get => _areCameraSettingsUsed;
            set => SetProperty(ref _areCameraSettingsUsed, value);
        }

        public double MaxExposureTime { get => _camera.MaxExposureTime; }
        public double MinExposureTime { get => _camera.MinExposureTime; }

        public double MaxGain { get => _camera.MaxGain; }
        public double MinGain { get => _camera.MinGain; }

        private int _normalizationMax;

        public int NormalizationMax
        {
            get => _normalizationMax;
            set
            {
                SetProperty(ref _normalizationMax, value);
                ParametersChanged = true;
            }
        }

        private int _normalizationMin;

        public int NormalizationMin
        {
            get => _normalizationMin;
            set
            {
                SetProperty(ref _normalizationMin, value);
                ParametersChanged = true;
            }
        }

        private bool _isStreaming;
        public bool IsStreaming { get => _isStreaming; set => SetProperty(ref _isStreaming, value); }

        private StepStates _stepState = StepStates.NotDone;
        public StepStates StepState
        {
            get => _stepState;
            set
            {
                if (_stepState != value)
                {
                    _stepState = value;
                    if (_stepState != StepStates.Error)
                    {
                        ToolTipMessage = null;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _toolTipMessage;
        public string ToolTipMessage
        {
            get => _toolTipMessage; set { if (_toolTipMessage != value) { _toolTipMessage = value; OnPropertyChanged(); } }
        }

        private bool _parametersChanged;
        private bool ParametersChanged
        {
            get => _parametersChanged;
            set
            {
                SetProperty(ref _parametersChanged, value);
                ApplyConfiguration.NotifyCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        private AsyncRelayCommand _captureImageCommand;

        public IAsyncRelayCommand CaptureImageCommand
        {
            get
            {
                if (_captureImageCommand == null)
                    _captureImageCommand = new AsyncRelayCommand(CaptureImageAsync);

                return _captureImageCommand;
            }
        }

        private async Task CaptureImageAsync()
        {
            try
            {
                StepState = StepStates.InProgress;
                var image = await _camera.SingleAcquisitionAsync();
                if (image == null)
                {
                    string errorMsg = "Impossible to capture the image.";
                    StepState = StepStates.Error;
                    ToolTipMessage = errorMsg;
                    _messenger?.Send(new Message(MessageLevel.Warning, errorMsg));
                    return;
                }
                //Save Image
                string filePath = Path.Combine(_acquisitionDirectory, $"Image-{DateTime.Now:yyyy-MM-dd-HHmmss}.tiff");
                await _camera.SaveImageAsync(image, filePath);
                string msg = $"Image saved under: {filePath}";
                _messenger?.Send(new Message(MessageLevel.Information, msg));

                await Task.Delay(250);

                StepState = StepStates.Done;
                ToolTipMessage = "Image saved";
            }
            catch (Exception ex)
            {
                StepState = StepStates.Error;
                ToolTipMessage = ex.Message;
            }
        }

        private AsyncRelayCommand _applyConfiguration;
        public IAsyncRelayCommand ApplyConfiguration
        {
            get
            {
                if (_applyConfiguration == null)
                    _applyConfiguration = new AsyncRelayCommand(PerformApplyConfigurationAsync, CanApplySettings);

                return _applyConfiguration;
            }
        }

        private bool CanApplySettings()
        {
            return ParametersChanged;
        }

        private void CheckAndApplyNormalizationSettings()
        {
            if (NormalizationMax > _camera.MaxPixelValue)
            {
                NormalizationMax = _camera.MaxPixelValue;
            }
            if (NormalizationMin > _camera.MaxPixelValue)
            {
                NormalizationMin = _camera.MaxPixelValue;
            }
            if (NormalizationMax < NormalizationMin)
            {
                NormalizationMax = NormalizationMin;
            }
            _camera.NormalizationMax = NormalizationMax;
            _camera.NormalizationMin = NormalizationMin;
        }

        private async Task PerformApplyConfigurationAsync()
        {
            ParametersChanged = false;
            await _camera.SetExposureTime(_exposureTime);
            await _camera.SetGain(_gain);

            CheckAndApplyNormalizationSettings();
        }

        private AsyncRelayCommand _startStreamingCommand;
        public IAsyncRelayCommand StartStreamingCommand
        {
            get
            {
                if (_startStreamingCommand == null)
                    _startStreamingCommand = new AsyncRelayCommand(() => _camera.StartStreamingAsync(), () => !_camera.IsStreaming);

                return _startStreamingCommand;
            }
        }

        private AsyncRelayCommand _stopStreamingCommand;
        public IAsyncRelayCommand StopStreamingCommand
        {
            get
            {
                if (_stopStreamingCommand == null)
                    _stopStreamingCommand = new AsyncRelayCommand(() => _camera.StopStreamingAsync(), () => _camera.IsStreaming);

                return _stopStreamingCommand;
            }
        }
        #endregion

        public void Close()
        {
            if (!_camera.IsStreaming)
                return;

            Application.Current?.Dispatcher.Invoke(async () =>
            {
                try
                {
                    await _camera.StopStreamingAsync();
                }
                catch (Exception ex)
                {
                    string msg = $"Close camera error : {ex.Message}";
                    ClassLocator.Default.GetInstance<ILogger>()?.Error(ex, msg);
                    _messenger.Send(new Message(MessageLevel.Error, msg));
                }
            });
        }

        internal void Refresh()
        {
            Init();
        }
    }
}
