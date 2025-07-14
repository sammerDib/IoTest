using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class GlobalTopoVM : SettingVM, ITabManager, IDisposable
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private double _exposureTimeAlreadyUsed;
        private CamCalibAcquisitionVM _currentCamCalibAcquisition;
        private readonly ScreenInfo _screenInfo;
        private readonly List<Fringe> _fringes;
        private bool _canSaveCameraCalibrationBackup;
        private bool _canSaveSystemCalibrationBackup;
        private bool _isScreenWhite;

        public GlobalTopoVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;

            Header = "Topography";
            IsEnabled = true;
            ExposureSettings = exposureSettings;
            _screenInfo = ScreenSupervisor.GetScreenInfo(WaferSide);
            _fringes = ScreenSupervisor.GetAvailableFringes();
            _canSaveCameraCalibrationBackup = _calibrationSupervisor.DoesGlobalTopoCameraCalibrationExist(waferSide);
            _canSaveSystemCalibrationBackup = _calibrationSupervisor.DoesGlobalTopoSystemCalibrationExist(waferSide);
            GetFringesPeriodValues();
            InitializeCamCalibAcquisitions();
        }

        public ExposureSettingsWithAutoVM ExposureSettings { get; private set; }

        private ObservableCollection<CamCalibAcquisitionVM> _camCalibAcquisitions;

        public ObservableCollection<CamCalibAcquisitionVM> CamCalibAcquisitions
        {
            get { return _camCalibAcquisitions; }
            set { if (_camCalibAcquisitions != value) { _camCalibAcquisitions = value; OnPropertyChanged(); } }
        }

        private List<int> _periods;

        public List<int> Periods
        {
            get { if (_periods == null) { _periods = GetFringesPeriodValues(); } return _periods; }
        }

        private BitmapSource _lastAcquiredImage;

        public BitmapSource LastAcquiredImage
        { get { return _lastAcquiredImage; } set { _lastAcquiredImage = value; OnPropertyChanged(); } }

        private AsyncRelayCommand<CamCalibAcquisitionVM> _acquireCamCalibImage;

        public AsyncRelayCommand<CamCalibAcquisitionVM> AcquireCamCalibImage
        {
            get
            {
                return _acquireCamCalibImage ?? (_acquireCamCalibImage = new AsyncRelayCommand<CamCalibAcquisitionVM>(
                    async (camCalibAcquisition) =>
                    {
                        _currentCamCalibAcquisition = camCalibAcquisition;
                        BusyMessage = "Acquiring Image";
                        IsBusy = true;
                        try
                        {
                            var lastAcquiredServiceImage = await _calibrationSupervisor.AcquireCameraCalibrationImageAsync
                            (WaferSide, _currentCamCalibAcquisition.Name, ExposureSettings.ExposureTimeMs);
                            lastAcquiredServiceImage.WpfBitmapSource.Freeze();
                            LastAcquiredImage = lastAcquiredServiceImage.WpfBitmapSource;
                            _currentCamCalibAcquisition.IsAcquired = true;
                            _exposureTimeAlreadyUsed = ExposureSettings.ExposureTimeMs;

                            // We add an optional acqusition if needed
                            if (!CamCalibAcquisitions.Any(cc => !cc.IsMandatory && !cc.IsAcquired))
                            {
                                var newIndex = CamCalibAcquisitions.Count(cc => !cc.IsMandatory) + 1;
                                _camCalibAcquisitions.Add(new CamCalibAcquisitionVM($"Extra acquisition {newIndex}", false));
                            }
                        }
                        catch (Exception ex)
                        {
                            DialogService.ShowMessageBox($"Failed to acquire the image {_currentCamCalibAcquisition?.Name} :\n{ex.Message}",
                                "Image Acquisition", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            IsBusy = false;
                            ExecuteCalibrationCam.NotifyCanExecuteChanged();
                        }
                    }
                ));
            }
        }

        private AutoRelayCommand<CamCalibAcquisitionVM> _removeCamCalibImage;

        public AutoRelayCommand<CamCalibAcquisitionVM> RemoveCamCalibImage
        {
            get
            {
                return _removeCamCalibImage ?? (_removeCamCalibImage = new AutoRelayCommand<CamCalibAcquisitionVM>(
                    (camCalibAcquisition) =>
                    {
                        _calibrationSupervisor.RemoveCameraCalibrationImage(WaferSide, camCalibAcquisition.Name);
                        ExecuteCalibrationCam.NotifyCanExecuteChanged();
                    }
                ));
            }
        }

        private RelayCommand _displayWhite;

        public RelayCommand DisplayWhite
        {
            get
            {
                return _displayWhite ?? (_displayWhite = new RelayCommand(
                    () =>
                    {
                        ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);
                        _isScreenWhite = true;
                        DisplayBlack.NotifyCanExecuteChanged();
                        DisplayWhite.NotifyCanExecuteChanged();
                    },
                    () => !_isScreenWhite
                ));
            }
        }

        private RelayCommand _displayBlack;

        public RelayCommand DisplayBlack
        {
            get
            {
                return _displayBlack ?? (_displayBlack = new RelayCommand(
                    () =>
                    {
                        ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
                        _isScreenWhite = false;
                        DisplayWhite.NotifyCanExecuteChanged();
                        DisplayBlack.NotifyCanExecuteChanged();
                    },
                    () => _isScreenWhite
                ));
            }
        }

        private AsyncRelayCommand _executeCalibrationCam;

        public AsyncRelayCommand ExecuteCalibrationCam
        {
            get
            {
                return _executeCalibrationCam ?? (_executeCalibrationCam = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "Calibrating Camera";
                        IsBusy = true;
                        try
                        {
                            await _calibrationSupervisor.CalibrateCameraAsync(WaferSide);
                            _canSaveCameraCalibrationBackup = _calibrationSupervisor.DoesGlobalTopoCameraCalibrationExist(WaferSide);
                            DialogService.ShowMessageBox("Camera calibration success", "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            var message = $"Global Topo Camera Calibration failed :\n{ex.Message}";
                            DialogService.ShowMessageBox(message, "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            ExecuteCalibrationSys.NotifyCanExecuteChanged();
                            SaveCameraCalibrationBackup.NotifyCanExecuteChanged();
                            IsBusy = false;
                        }
                    },
                    () => CanCalibrateCam()
                ));
            }
        }

        private AsyncRelayCommand _saveCameraCalibrationBackup;

        public AsyncRelayCommand SaveCameraCalibrationBackup
        {
            get
            {
                return _saveCameraCalibrationBackup ?? (_saveCameraCalibrationBackup = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "save camera calibration backup";
                        IsBusy = true;
                        try
                        {
                            await _calibrationSupervisor.SaveCameraCalibrationBackupAsync(WaferSide);
                            _canSaveCameraCalibrationBackup = false;
                            DialogService.ShowMessageBox("Camera calibration backup file successfully saved", "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            var message = $"Failed to save camera calibration backup file :\n{ex.Message}";
                            DialogService.ShowMessageBox(message, "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    },
                    () => _canSaveCameraCalibrationBackup
                ));
            }
        }

        private AsyncRelayCommand _executeCalibrationSys;

        public AsyncRelayCommand ExecuteCalibrationSys
        {
            get
            {
                return _executeCalibrationSys ?? (_executeCalibrationSys = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "Calibrating System";
                        IsBusy = true;
                        try
                        {
                            await _calibrationSupervisor.CalibrateSystemAsync(WaferSide, Periods, ExposureSettings.ExposureTimeMs);
                            _canSaveSystemCalibrationBackup = _calibrationSupervisor.DoesGlobalTopoSystemCalibrationExist(WaferSide);
                            DialogService.ShowMessageBox("System calibration success", "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            var message = $"Global Topo System Calibration failed :\n{ex.Message}";
                            DialogService.ShowMessageBox(message, "Global Topo Calibration", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            SaveSystemCalibrationBackup.NotifyCanExecuteChanged();
                            IsBusy = false;
                        }
                    },
                    () => CanCalibrateSys()
                ));
            }
        }

        private AsyncRelayCommand _saveSytemCalibrationBackup;

        public AsyncRelayCommand SaveSystemCalibrationBackup
        {
            get
            {
                return _saveSytemCalibrationBackup ?? (_saveSytemCalibrationBackup = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "save system calibration backup";
                        IsBusy = true;
                        try
                        {
                            await _calibrationSupervisor.SaveSystemCalibrationBackupAsync(WaferSide);
                            _canSaveSystemCalibrationBackup = false;
                            DialogService.ShowMessageBox("System calibration backup file successfully saved", "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            var message = $"Failed to save system calibration backup file :\n{ex.Message}";
                            DialogService.ShowMessageBox(message, "Global Topo Calibration",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    },
                    () => _canSaveSystemCalibrationBackup
                ));
            }
        }

        public void Display()
        {
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;
            ExposureSettings.PropertyChanged += ExposureSettings_PropertyChanged;
            AskIfExposureTimeCanBeChanged();
            ExecuteCalibrationCam.NotifyCanExecuteChanged();
            SaveCameraCalibrationBackup.NotifyCanExecuteChanged();
            ExecuteCalibrationSys.NotifyCanExecuteChanged();
            SaveSystemCalibrationBackup.NotifyCanExecuteChanged();
        }

        public bool CanHide() => true;

        public void Hide()
        {
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;
            ExposureSettings.PropertyChanged -= ExposureSettings_PropertyChanged;
            LastAcquiredImage = null;
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
            foreach (var camCalibAcquisition in CamCalibAcquisitions)
            {
                camCalibAcquisition.IsAcquired = false;
            }
            _calibrationSupervisor.ClearCameraCalibrationImages();
        }

        public void Dispose()
        {
            ResetAcquisitions();
        }

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
                if (IsThereAnyAcquisition() && (ExposureSettings.ExposureTimeMs != _exposureTimeAlreadyUsed))
                {
                    var result = DialogService.ShowMessageBox($"If you change the exposure time, the acquisitions already done will be lost. Do you want to change the exposure time ?", "Exposure method", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        ResetAcquisitions();
                    }
                    else

                        ExposureSettings.ExposureTimeMs = _exposureTimeAlreadyUsed;
                }
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private bool CanCalibrateCam()
        {
            if (CamCalibAcquisitions.Any(cc => !cc.IsAcquired && cc.IsMandatory))
            {
                return false;
            }

            return true;
        }

        private bool CanCalibrateSys()
        {
            return _calibrationSupervisor.DoesGlobalTopoCameraCalibrationExist(WaferSide);
        }

        private void ResetAcquisitions()
        {
            foreach (var camCalibAcquisition in CamCalibAcquisitions)
            {
                camCalibAcquisition.IsAcquired = false;
            }
            _calibrationSupervisor.ClearCameraCalibrationImages();
            LastAcquiredImage = null;
        }

        private bool IsThereAnyAcquisition()
        {
            if (CamCalibAcquisitions.Any(cc => cc.IsAcquired))
                return true;

            return false;
        }

        private void InitializeCamCalibAcquisitions()
        {
            var imagesName = _calibrationSupervisor.GetCameraCalibrationImageNames();
            _camCalibAcquisitions = new ObservableCollection<CamCalibAcquisitionVM>();
            foreach (var imageName in imagesName)
            {
                _camCalibAcquisitions.Add(new CamCalibAcquisitionVM(imageName));
            }
            _camCalibAcquisitions.Add(new CamCalibAcquisitionVM("Extra acquisition 1", false));
        }

        private List<int> GetFringesPeriodValues()
        {
            var periods = new List<int>(3);
            if (_fringes != null && _fringes.Count > 0)
            {
                // Period3 is the first period greater than the height of the screen
                var period3Index = _fringes.FindIndex(fringe => fringe.Period > _screenInfo.Height);
                if (period3Index > 2)
                {
                    periods.Add(_fringes[0].Period);
                    periods.Add(_fringes[period3Index / 2].Period); // Period2 is the period in the middle
                    periods.Add(_fringes[period3Index].Period);
                }
            }
            return periods;
        }
    }
}
