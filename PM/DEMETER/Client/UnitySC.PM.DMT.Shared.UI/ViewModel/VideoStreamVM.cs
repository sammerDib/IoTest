using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Shared.UI.ViewModel
{
    public class VideoStreamVM : ObservableRecipient
    {
        private readonly object _lock = new object();

        protected readonly CameraSupervisor CameraSupervisor;
        
        protected readonly ScreenSupervisor ScreenSupervisor;

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        {
            get => _cameraBitmapSource;
            set => SetProperty(ref _cameraBitmapSource, value);
        }

        private ExposureSettingsWithAutoVM _exposureSettings;

        public ExposureSettingsWithAutoVM ExposureSettings
        {
            get => _exposureSettings;
            set => SetProperty(ref _exposureSettings, value);
        }

        private int _imageHeight;

        public int ImageHeight
        {
            get => _imageHeight;
            set => SetProperty(ref _imageHeight, value);
        }

        private int _imageWidth;

        public int ImageWidth
        {
            get => _imageWidth;
            set => SetProperty(ref _imageWidth, value);
        }

        private bool _isGrabbing;

        public bool IsGrabbing
        {
            get => _isGrabbing;
            set => SetProperty(ref _isGrabbing, value);
        }

        private Side _waferSide;

        public Side WaferSide
        {
            get => _waferSide;
            set => SetProperty(ref _waferSide, value);
        }

        private double _zoomBoxScale = 1.0;

        public double ZoomBoxScale
        {
            get => _zoomBoxScale;
            set
            {
                if (value != 0)
                {
                    SetProperty(ref _zoomBoxScale, value);
                }
            }
        }

        public VideoStreamVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, IMessenger messenger) : base(messenger)
        {
            CameraSupervisor = cameraSupervisor;
            ScreenSupervisor = screenSupervisor;
            _waferSide = waferSide;
        }

        public void StartGrab(bool propagateStartToServer = true)
        {
            lock (_lock)
            {
                PrepareGrab();
                IsGrabbing = true;
                ExposureSettings.PropertyChanged += ExposureSettingsPropertyChanged;
                ExposureSettings.ApplyExposureSettings.Execute(ExposureSettings);
                if (propagateStartToServer)
                {
                    CameraSupervisor.StartContinuousAcquisition(WaferSide);
                }
                Task.Factory.StartNew(DisplayTask, TaskCreationOptions.LongRunning);
            }
        }

        public void StopGrab(bool propagateStopToServer = true)
        {
            lock (_lock)
            {
                if (propagateStopToServer)
                {
                    CameraSupervisor.StopContinuousAcquisition(_waferSide);
                }
                IsGrabbing = false;
                ExposureSettings.PropertyChanged -= ExposureSettingsPropertyChanged;
            }
        }

        protected virtual void PrepareGrab()
        {
            // To be overridden when needed
        }

        protected virtual void ExposureSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExposureSettings.ExposureTimeMs) && _isGrabbing)
            {
                ExposureSettings.ApplyExposureSettings.Execute(ExposureSettings);
            }
            OnPropertyChanged(e);
        }

        protected virtual ServiceImage GetCameraImage(double scale, ROI roi)
        {
            return CameraSupervisor.GetRawImageWithStatistics(WaferSide, Int32Rect.Empty, scale, roi);
        }

        protected virtual void DisplayCameraImage(ServiceImage svcimage)
        {
            if (!_isGrabbing)
                return;

            if (svcimage == null)
                return;

            var bitmapImage = svcimage.WpfBitmapSource;
            CameraBitmapSource = bitmapImage;
        }

        private void DisplayTask()
        {
            try
            {
                while (_isGrabbing)
                {
                    double scale = Math.Min(1, ZoomBoxScale * 2);
                    var roi = new ROI();
                    roi.Rect = new Rect(0, 0, 1, 1);
                    var svcimg = GetCameraImage(scale, roi);

                    var app = Application.Current;
                    if (app == null)
                        _isGrabbing = false; // bidouille pour arrêter le thread quand l'application est fermée

                    if (_isGrabbing)
                        app.Dispatcher.Invoke(() => { DisplayCameraImage(svcimg); });
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
