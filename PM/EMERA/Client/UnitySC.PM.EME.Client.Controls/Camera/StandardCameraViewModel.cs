using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.Shared.Image;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Controls.Camera
{
    public class StandardCameraViewModel : ObservableRecipient, IDisposable
    {
        private readonly CameraBench _camera;
        private readonly IMessenger _messenger;        

        public StandardCameraViewModel(CameraBench camera, IMessenger messenger)
        {
            _messenger = messenger;
            _camera = camera;
            double cameraResolution = _camera.Width * _camera.Height;
            double initialScaleResolution = Math.Min(1.0, Math.Sqrt(TargetResolution / cameraResolution));
            _camera.SetStreamedImageDimension(Int32Rect.Empty, initialScaleResolution);
            DefaultFullRoi = new Rect(0, 0, _camera.GetCameraInfo().Width, _camera.GetCameraInfo().Height);
        }
        private void Init()
        {
            _messenger?.Register<ServiceImageWithStatistics>(this, (_, image) => Image = image);
            _camera.PropertyChanged += CameraBench_PropertyChanged;
            IsStreaming = _camera.IsStreaming;
            ExposureTime = _camera.GetExposureTime();
            Gain = _camera.GetGain();
        }

        public void Refresh()
        {
            Init();
        }

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
            StartStreamingCommand?.NotifyCanExecuteChanged();
            StopStreamingCommand?.NotifyCanExecuteChanged();
        }

        private string FormatAsLengthText(double distanceInMicrometers)
        {
            if (distanceInMicrometers >= 1000)
            {
                return $"{distanceInMicrometers / 1000:G3} mm";
            }

            if (distanceInMicrometers > 1)
            {
                return $"{distanceInMicrometers:G3} µm";
            }

            return $"{distanceInMicrometers * 1000:G3} nm";
        }

        public bool UseRoi { get; set; } = false;

        public Rect DefaultFullRoi { get; }

        private Rect _roiRect = Rect.Empty;

        public Rect RoiRect
        {
            get
            {
                if (_roiRect == Rect.Empty)
                {
                    _roiRect = DefaultFullRoi;
                    OnPropertyChanged(nameof(RoiRect));
                }


                return _roiRect;
            }

            set { SetProperty(ref _roiRect, value); }
        }

        private ServiceImageWithStatistics _image;

        public ServiceImageWithStatistics Image
        {
            get => _image;
            set
            {
                SetProperty(ref _image, value);
                OnPropertyChanged(nameof(ScaleTextValue));
                OnPropertyChanged(nameof(FullImageWidth));
                OnPropertyChanged(nameof(FullImageHeight));
                OnPropertyChanged(nameof(ImageCropArea));
            }
        }

        public int FullImageWidth => (Image?.OriginalWidth) ?? 0;
        public int FullImageHeight => (Image?.OriginalHeight) ?? 0;
        public Int32Rect ImageCropArea
        {
            get
            {
                if (Image == null)
                {
                    return Int32Rect.Empty;
                }
                if (Image.AcquisitionRoi == Int32Rect.Empty)
                {
                    return new Int32Rect { X = 0, Y = 0, Width = Image.OriginalWidth, Height = Image.OriginalHeight };
                }
                return Image.AcquisitionRoi;
            }
        }

        private double _zoom;

        public double Zoom
        {
            get => _zoom;
            set
            {
                SetProperty(ref _zoom, value);
                OnPropertyChanged(nameof(ScaleTextValue));
            }
        }

        public int TargetResolution { get; set; } = 1920 * 1080;

        private Int32Rect _imagePortion;

        public Int32Rect ImagePortion
        {
            get => _imagePortion;
            set
            {
                if (Image == null || _imagePortion == value)
                {
                    return;
                }

                double currentResolution = value.Width * value.Height;
                double scaleResolution = Math.Min(1.0, Math.Sqrt(TargetResolution / currentResolution));
                _camera.SetStreamedImageDimension(value, scaleResolution);

                SetProperty(ref _imagePortion, value);
            }
        }

        public double ScaleLengthInPixel => 200;

        public string ScaleTextValue
        {
            get
            {
                if (_image == null)
                    return "";

                double scaleLengthInMicrometers = _camera.PixelSize.Micrometers * ScaleLengthInPixel / Zoom;
                return FormatAsLengthText(scaleLengthInMicrometers);
            }
        }

        private AsyncRelayCommand _startStreamingCommand;

        public IAsyncRelayCommand StartStreamingCommand
        {
            get
            {
                if (_startStreamingCommand == null)
                    _startStreamingCommand = new AsyncRelayCommand(() => _camera.StartStreamingAsync(), () => !IsStreaming);

                return _startStreamingCommand;
            }
        }

        private AsyncRelayCommand _stopStreamingCommand;

        public IAsyncRelayCommand StopStreamingCommand
        {
            get
            {
                if (_stopStreamingCommand == null)
                    _stopStreamingCommand = new AsyncRelayCommand(() => _camera.StopStreamingAsync(), () => IsStreaming);

                return _stopStreamingCommand;
            }
        }
        private AsyncRelayCommand _applyCameraSettings;

        public IAsyncRelayCommand ApplyCameraSettings
        {
            get
            {
                if (_applyCameraSettings == null)
                    _applyCameraSettings = new AsyncRelayCommand(PerformApplyCameraSettingsAsync);

                return _applyCameraSettings;
            }
        }

        private bool _isStreaming;

        public bool IsStreaming 
        { 
            get => _isStreaming; 
            set 
            {
                SetProperty(ref _isStreaming, value); 
                NotifyStreamingChange();
            } 
        }

        private bool _areCameraSettingsUsed;

        public bool AreCameraSettingsUsed { get => _areCameraSettingsUsed; set => SetProperty(ref _areCameraSettingsUsed, value); }

        public double MaxExposureTime => _camera.MaxExposureTime;

        public double MinExposureTime => _camera.MinExposureTime;

        private double _exposureTime;

        public double ExposureTime { get => _exposureTime; set => SetProperty(ref _exposureTime, value); }

        public double MaxGain => _camera.MaxGain;

        public double MinGain => _camera.MinGain;

        private double _gain;

        public double Gain { get => _gain; set => SetProperty(ref _gain, value); }
    
        private async Task PerformApplyCameraSettingsAsync()
        {
            await _camera.SetExposureTime(_exposureTime);
            await _camera.SetGain(_gain);
        }
        private void EnsureStreamingStopped()
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
                    var msg = $"Close camera error : {ex.Message}";
                    ClassLocator.Default.GetInstance<ILogger>()?.Error(ex, msg); ;
                    ClassLocator.Default.GetInstance<IMessenger>()?.Send(new Message(MessageLevel.Error, msg));
                }
            });
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EnsureStreamingStopped();
                if (_camera != null)
                {
                    _camera.PropertyChanged -= CameraBench_PropertyChanged;
                }
                _messenger.Unregister<ServiceImageWithStatistics>(this);              
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~StandardCameraViewModel()
        {
            Dispose(false);
        }
    }
}
