using System;
using System.ComponentModel;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class TestCameraVM : VideoStreamVM, ITabManager
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private double _minExposureTimeMs = 10;

        public double MinExposureTimeMs
        {
            get => _minExposureTimeMs;
            set => SetProperty(ref _minExposureTimeMs, value);
        }

        private double _maxExposureTimeMs = 500;

        public double MaxExposureTimeMs
        {
            get => _maxExposureTimeMs;
            set => SetProperty(ref _maxExposureTimeMs, value);
        }

        private string _serialNumber;

        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private int _sensorResolutionX;

        public int SensorResolutionX
        {
            get => _sensorResolutionX;
            set => SetProperty(ref _sensorResolutionX, value);
        }

        private int _sensorResolutionY;

        public int SensorResolutionY
        {
            get => _sensorResolutionY;
            set => SetProperty(ref _sensorResolutionY, value);
        }

        public TestCameraVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService, IMessenger messenger) : base(
            waferSide, cameraSupervisor, screenSupervisor, messenger)
        {
            ExposureSettings = new ExposureSettingsWithAutoVM(waferSide, MeasureType.BrightFieldMeasure,
                cameraSupervisor, screenSupervisor, algorithmsSupervisor, dialogService)
            {
                EditExposureTime = 80, ExposureTimeMs = 80, ExposureTimeStatus = ExposureTimeStatus.Valid
            };
            ExposureSettings.ApplyExposureSettings.CanExecuteChanged += HandleApplyExposureSettingsCanExecuteChanged;
        }

        private void HandleApplyExposureSettingsCanExecuteChanged(object sender, EventArgs e)
        {
            if (sender is ExposureSettingsWithAutoVM settingsVM && settingsVM == ExposureSettings)
            {
                OnPropertyChanged(nameof(ExposureSettings.ApplyExposureSettings));
            }
        }

        private void AcquireOneImage()
        {
            CameraSupervisor.SetExposureTime(WaferSide, ExposureSettings.ExposureTimeMs);
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.White);

            var svcimg = CameraSupervisor.GetCameraImage(WaferSide);
            if (svcimg == null)
                return;
            CameraBitmapSource = svcimg.WpfBitmapSource;
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black, false);
        }

        private RelayCommand _startVideoStreamCommand;

        public RelayCommand StartVideoStreamCommand
        {
            get => _startVideoStreamCommand ?? (_startVideoStreamCommand = new RelayCommand(() =>
            {
                ScreenSupervisor.SetScreenColor(WaferSide, Colors.White);
                StartGrab();
            }, () => !IsGrabbing));
        }

        private RelayCommand _stopVideoStreamCommand;

        public RelayCommand StopVideoStreamCommand
        {
            get => _stopVideoStreamCommand ?? (_stopVideoStreamCommand = new RelayCommand(() =>
            {
                StopGrab();
                ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
            }, () => IsGrabbing));
        }

        private RelayCommand _acquireOneImageCommand;

        public RelayCommand AcquireOneImageCommand
        {
            get => _acquireOneImageCommand ?? (_acquireOneImageCommand = new RelayCommand(AcquireOneImage, () => !IsGrabbing));
        }

        public void Display()
        {
            IsActive = true;
            var cameraInfo = CameraSupervisor.GetCameraInfo(WaferSide);
            if (!(cameraInfo is null))
            {
                MinExposureTimeMs = cameraInfo.MinExposureTimeMs;
                MaxExposureTimeMs = cameraInfo.MaxExposureTimeMs;
                ExposureSettings.ExposureTimeMs = MinExposureTimeMs;
                ExposureSettings.EditExposureTime = MinExposureTimeMs;
                SerialNumber = cameraInfo.SerialNumber;
                SensorResolutionX = cameraInfo.Width;
                SensorResolutionY = cameraInfo.Height;
                ImageHeight = cameraInfo.Height;
                ImageWidth = cameraInfo.Width;
            }
            else
            {
                SerialNumber = null;
                SensorResolutionX = 0;
                SensorResolutionY = 0;
            }
        }

        public bool CanHide()
        {
            return true;
        }

        public void Hide()
        {
            IsActive = false;
            if (IsGrabbing)
            {
                StopGrab();
            }
        }

        protected override ServiceImage GetCameraImage(double scale, ROI roi)
        {
            return CameraSupervisor.GetCameraImage(WaferSide);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (nameof(IsGrabbing) == e.PropertyName)
            {
                StartVideoStreamCommand.NotifyCanExecuteChanged();
                StopVideoStreamCommand.NotifyCanExecuteChanged();
                AcquireOneImageCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
