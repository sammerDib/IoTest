using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera.Device;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel
{
    public class TestCameraViewModel : TabViewModelBase
    {
        private readonly CameraBench _camera;
        private readonly IMessenger _messenger;
        private const int LowestResAllowed = 176;

        public TestCameraViewModel(CameraBench camera, IMessenger messenger)
        {
            _camera = camera;
            _messenger = messenger;
            messenger?.Register<ServiceImageWithStatistics>(this, (_, image) => Image = image);

            Init();
        }

        private void Init()
        {
            Gain = _camera.GetGain();
            ExposureTime = _camera.GetExposureTime();
            Width = _camera.Width;
            Height = _camera.Height;
            ColorModes = _camera.GetColorModes();
            SelectedColorMode = _camera.GetColorMode();

            double cameraResolution = _camera.Width * _camera.Height;
            double scaleResolution = Math.Min(1.0, Math.Sqrt(1920 * 1080 / cameraResolution));
            _camera.SetStreamedImageDimension(Int32Rect.Empty, scaleResolution);
        }

        #region Properties

        private ServiceImage _image;

        public ServiceImage Image { get => _image; set => SetProperty(ref _image, value); }

        private bool _isStreaming;

        public bool IsStreaming
        {
            get => _isStreaming; set
            {
                SetProperty(ref _isStreaming, value);
                SingleShotCommand.NotifyCanExecuteChanged();
            }
        }

        private double _gain;

        public double Gain
        {
            get => _gain; set
            {
                SetProperty(ref _gain, value);
                ParametersChanged = true;
            }
        }

        private double _exposureTime;

        public double ExposureTime
        {
            get => _exposureTime; set
            {
                SetProperty(ref _exposureTime, value);
                ParametersChanged = true;
            }
        }

        private int _width;
        public int Width
        {
            get => _width; set
            {
                SetProperty(ref _width, value);
                ParametersChanged = true;
            }
        }

        private int _height;
        public int Height
        {
            get => _height; set
            {
                SetProperty(ref _height, value);
                ParametersChanged = true;
            }
        }
        private bool _parametersChanged = false;
        private bool ParametersChanged
        {
            get => _parametersChanged;
            set
            {
                SetProperty(ref _parametersChanged, value);
                ApplyConfiguration.NotifyCanExecuteChanged();
            }
        }

        private List<ColorMode> _colorModes;

        public List<ColorMode> ColorModes { get => _colorModes; set => SetProperty(ref _colorModes, value); }

        private ColorMode _selectedColorMode;

        public ColorMode SelectedColorMode
        {
            get => _selectedColorMode;
            set
            {
                SetProperty(ref _selectedColorMode, value);
                ParametersChanged = true;
            }
        }

        public MatroxCameraInfo CameraInfo { get => _camera.GetMatroxCameraInfo(); }

        public int MinWidthAllowed { get => Math.Max(CameraInfo.MinWidth, LowestResAllowed); }
        public int MinHeightAllowed { get => Math.Max(CameraInfo.MinHeight, LowestResAllowed); }

        public double AppliedExposureTime { get => _camera.GetExposureTime(); }

        public double AppliedGain { get => _camera.GetGain(); }

        public double FrameRate { get => _camera.GetFrameRate(); }

        public ColorMode ColorMode { get => _camera.GetColorMode(); }
        #endregion

        #region Commands
        private AsyncRelayCommand _startStreamingCommand;

        public IAsyncRelayCommand StartStreamingCommand
        {
            get
            {
                if (_startStreamingCommand == null)
                    _startStreamingCommand = new AsyncRelayCommand(StartStreamingAsync);

                return _startStreamingCommand;
            }
        }

        private async Task StartStreamingAsync()
        {
            await _camera.StartStreamingAsync();
            IsStreaming = true;
        }

        private AsyncRelayCommand _stopStreamingCommand;

        public IAsyncRelayCommand StopStreamingCommand
        {
            get
            {
                if (_stopStreamingCommand == null)
                    _stopStreamingCommand = new AsyncRelayCommand(StopStreamingAsync);

                return _stopStreamingCommand;
            }
        }

        private async Task StopStreamingAsync()
        {
            await _camera.StopStreamingAsync();
            IsStreaming = false;
        }

        private AsyncRelayCommand _singleShotCommand;

        public IAsyncRelayCommand SingleShotCommand
        {
            get
            {
                if (_singleShotCommand == null)
                    _singleShotCommand = new AsyncRelayCommand(SingleShotAsync, () => !IsStreaming);

                return _singleShotCommand;
            }
        }

        private async Task SingleShotAsync()
        {
            try
            {
                Image = await _camera.SingleAcquisitionAsync();
            }
            catch (Exception)
            {
                _messenger?.Send(new Message(MessageLevel.Warning, "Failed to perform a single image acquisition."));
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

        private async Task PerformApplyConfigurationAsync()
        {
            ParametersChanged = false;
            bool wasStreaming = IsStreaming;
            if (wasStreaming)
            {
                await StopStreamingAsync();
            }

            await _camera.SetExposureTime(_exposureTime);
            OnPropertyChanged(nameof(AppliedExposureTime));

            await _camera.SetGain(_gain);
            OnPropertyChanged(nameof(AppliedGain));

            Rect aoi = new Rect(0, 0, Width, Height);
            _camera.SetAOI(aoi);

            OnPropertyChanged(nameof(FrameRate));
            OnPropertyChanged(nameof(CameraInfo));

            await _camera.SetColorModeAsync(SelectedColorMode);
            OnPropertyChanged(nameof(ColorMode));

            if (wasStreaming)
            {
                await StartStreamingAsync();
            }
        }
        #endregion

        public void Close()
        {
            if (IsStreaming)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await _camera.StopStreamingAsync();
                    ResetCameraSettings();
                    IsStreaming = false;
                });
            }
            else
            {
                ResetCameraSettings();
            }

        }

        internal void Refresh()
        {
            Init();
        }
        private void ResetCameraSettings()
        {
            Rect originalAOI = new Rect(0, 0, CameraInfo.MaxWidth, CameraInfo.MaxHeight);
            _camera.SetAOI(originalAOI);
            OnPropertyChanged(nameof(CameraInfo));
        }
        private bool CanApplySettings()
        {
            return ParametersChanged;
        }
    }
}
