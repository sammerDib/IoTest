using System;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class SystemUniformityVM : SettingVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private double _exposureTimeAlreadyUsed;

        public SystemUniformityVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;

            Header = "System uniformity";
            IsEnabled = true;
            ExposureSettings = exposureSettings;
            CamCalibAcquisition = new CamCalibAcquisitionVM("Bright-field image");
        }

        public ExposureSettingsWithAutoVM ExposureSettings { get; private set; }

        private CamCalibAcquisitionVM _camCalibAcquisition;

        public CamCalibAcquisitionVM CamCalibAcquisition
        {
            get { return _camCalibAcquisition; }
            set { if (_camCalibAcquisition != value) { _camCalibAcquisition = value; OnPropertyChanged(); } }
        }

        private BitmapSource _lastAcquiredImage;

        public BitmapSource LastAcquiredImage
        { get { return _lastAcquiredImage; } set { _lastAcquiredImage = value; OnPropertyChanged(); } }

        private AsyncRelayCommand _acquireCamCalibImage;

        public AsyncRelayCommand AcquireCamCalibImage
        {
            get
            {
                return _acquireCamCalibImage ?? (_acquireCamCalibImage = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "Acquiring Image";
                        IsBusy = true;
                        try
                        {
                            var lastAcquiredServiceImage = await _calibrationSupervisor.AcquireBrightFieldImageAsync(WaferSide, ExposureSettings.ExposureTimeMs);
                            lastAcquiredServiceImage.WpfBitmapSource.Freeze();
                            LastAcquiredImage = lastAcquiredServiceImage.WpfBitmapSource;
                            _camCalibAcquisition.IsAcquired = true;
                            _exposureTimeAlreadyUsed = ExposureSettings.ExposureTimeMs;
                        }
                        catch (Exception ex)
                        {
                            DialogService.ShowMessageBox($"Failed to acquire the calibration image :\n{ex.Message}",
                                "Image Acquisition", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            IsBusy = false;
                            SystemUniformityCalibration.NotifyCanExecuteChanged();
                        }
                    }
                ));
            }
        }

        private AutoRelayCommand _removeCamCalibImage;

        public AutoRelayCommand RemoveCamCalibImage
        {
            get
            {
                return _removeCamCalibImage ?? (_removeCamCalibImage = new AutoRelayCommand(
                    () =>
                    {
                        _calibrationSupervisor.RemoveBrightFieldImage(WaferSide);
                        SystemUniformityCalibration.NotifyCanExecuteChanged();
                        LastAcquiredImage = null;
                    }
                ));
            }
        }

        private AsyncRelayCommand _executeSystemUniformityCalibration;

        public AsyncRelayCommand SystemUniformityCalibration
        {
            get
            {
                return _executeSystemUniformityCalibration ?? (_executeSystemUniformityCalibration = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "System uniformity calibration";
                        IsBusy = true;
                        try
                        {
                            await _calibrationSupervisor.CalibrateSystemUniformityAsync(WaferSide);
                        }
                        catch (Exception ex)
                        {
                            var message = $"The system uniformity calibration failed : {ex.Message}";
                            DialogService.ShowMessageBox(message, "Calibration", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    },
                    () => _camCalibAcquisition.IsAcquired
                ));
            }
        }

        #region ITabManager implementation

        public void Display()
        {
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;
            ExposureSettings.PropertyChanged += ExposureSettings_PropertyChanged;
            AskIfExposureTimeCanBeChanged();
            SystemUniformityCalibration.NotifyCanExecuteChanged();
        }

        public bool CanHide() => true;

        public void Hide()
        {
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;
            ExposureSettings.PropertyChanged -= ExposureSettings_PropertyChanged;
            _calibrationSupervisor.RemoveBrightFieldImage(Side.Front);
            _calibrationSupervisor.RemoveBrightFieldImage(Side.Back);
            CamCalibAcquisition.IsAcquired = false;
            LastAcquiredImage = null;
        }

        #endregion ITabManager implementation

        private void ExposureSettings_AutoExposureStarted(object sender, EventArgs e)
        {
            BusyMessage = "Computing Exposure";
            IsBusy = true;
        }

        private void ExposureSettings_AutoExposureTerminated(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        private void ExposureSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExposureSettingsWithAutoVM.ExposureTimeMs))
            {
                AskIfExposureTimeCanBeChanged();
            }
        }

        private void AskIfExposureTimeCanBeChanged()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_camCalibAcquisition.IsAcquired && ExposureSettings.ExposureTimeMs != _exposureTimeAlreadyUsed)
                {
                    var result = DialogService.ShowMessageBox($"If you change the exposure time, the acquisition already done will be lost. Do you want to change the exposure time ?", "Exposure method", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        ResetAcquisitions();
                    }
                    else
                    {
                        ExposureSettings.ExposureTimeMs = _exposureTimeAlreadyUsed;
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void ResetAcquisitions()
        {
            _camCalibAcquisition.IsAcquired = false;
            _calibrationSupervisor.RemoveBrightFieldImage(WaferSide);
            LastAcquiredImage = null;
        }
    }
}
