using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class CameraScreenVideoStreamVM : CameraScreenSettingVM
    {
        private readonly CameraSupervisor _cameraSupervisor;
        private readonly object _lock = new object();
        private bool _isGrabbing = false;

        public CameraScreenVideoStreamVM(Side position, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor) : base(position)
        {
            ExposureSettings = exposureSettings;
            _cameraSupervisor = cameraSupervisor;
        }

        private System.Windows.Media.Imaging.BitmapSource _cameraBitmapSource;

        public System.Windows.Media.Imaging.BitmapSource CameraBitmapSource
        { get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); } } }

        public ExposureSettingsWithAutoVM ExposureSettings { get; set; }

        private bool _cameraTest = false;

        public bool CameraTest
        {
            get => _cameraTest; set { if (_cameraTest != value) { _cameraTest = value; OnPropertyChanged(); } }
        }

        private double _exposureTimeCameraTest;

        public double ExposureTimeCameraTest
        {
            get => _exposureTimeCameraTest; set { if (_exposureTimeCameraTest != value) { _exposureTimeCameraTest = value; OnPropertyChanged(); } }
        }

        private double _frameRate;

        public double FrameRate
        {
            get => _frameRate;
            set
            {
                if (_frameRate != value)
                {
                    _frameRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _imageWidth;

        public int ImageWidth
        {
            get => Math.Max(10, _imageWidth); set { if (_imageWidth != value) { _imageWidth = value; OnPropertyChanged(); } }
        }

        private int _imageHeight;

        public int ImageHeight
        {
            get => Math.Max(10, _imageHeight); set { if (_imageHeight != value) { _imageHeight = value; OnPropertyChanged(); } }
        }

        private double _zoomboxScale = 1;

        public double ZoomboxScale
        { get => _zoomboxScale; set { if (value == 0) return; if (_zoomboxScale != value) { _zoomboxScale = value; OnPropertyChanged(); } } }

        public virtual void SetExposureTime(double exposureTimeMs)
        {
            _cameraSupervisor.SetExposureTime(WaferSide, exposureTimeMs);
        }

        public void StartGrab()
        {
            lock (_lock)
            {
                if (!_isGrabbing)
                {
                    _isGrabbing = true;
                    ExposureSettings.PropertyChanged += ExposureSettingsPropertyChanged;
                    SetExposureTime(ExposureSettings.ExposureTimeMs);
                    _cameraSupervisor.StartContinuousAcquisition(WaferSide);
                    Task.Factory.StartNew(DisplayTask, TaskCreationOptions.LongRunning);
                }
            }
        }

        public void StopGrab()
        {
            lock (_lock)
            {
                if (_isGrabbing)
                {
                    _isGrabbing = false;
                    ExposureSettings.PropertyChanged -= ExposureSettingsPropertyChanged;
                    _cameraSupervisor.StopContinuousAcquisition(WaferSide);
                }
            }
        }

        private void DisplayTask()
        {
            try
            {
                while (_isGrabbing)
                {
                    double scale = Math.Min(1, ZoomboxScale * 2);
                    var roi = new ROI();
                    roi.Rect = new Rect(0, 0, 1, 1);
                    var svcimg = GetCameraImage(scale, roi);

                    var app = Application.Current;
                    if (app == null)
                        _isGrabbing = false;    // bidouille pour arrêter le thread quand l'application est fermée

                    if (_isGrabbing)
                        app.Dispatcher.Invoke(new Action(() => { DisplayCameraImage(svcimg); }));
                }
            }
            catch
            {
                // ignored
            }
        }

        protected virtual ServiceImage GetCameraImage(double scale, ROI roi)
        {
            return _cameraSupervisor.GetRawImageWithStatistics(WaferSide, Int32Rect.Empty, scale, roi);
        }

        protected virtual void DisplayCameraImage(ServiceImage svcimage)
        {
            if (!_isGrabbing)
                return;

            if (svcimage == null)
                return;

            var bitmapImage = svcimage.WpfBitmapSource;
            ImageHeight = (int)bitmapImage.Height;
            ImageWidth = (int)bitmapImage.Width;
            CameraBitmapSource = bitmapImage;
        }

        private void ExposureSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ExposureSettings.ExposureTimeMs):
                    if (_isGrabbing)
                    {
                        SetExposureTime(ExposureSettings.ExposureTimeMs);
                    }
                    break;
            }
        }
    }
}
