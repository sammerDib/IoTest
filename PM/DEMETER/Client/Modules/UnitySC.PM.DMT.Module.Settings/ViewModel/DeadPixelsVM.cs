using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class DeadPixelsVM : SettingVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly DeadPixelsManager _deadPixelsManager;

        private ObservableCollection<SelectableDeadPixel> _deadPixels;

        public ObservableCollection<SelectableDeadPixel> DeadPixels
        {
            get
            {
                if (_deadPixels == null)
                    _deadPixels = new ObservableCollection<SelectableDeadPixel>();
                return _deadPixels;
            }
        }

        public DeadPixelsVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;

            Header = "Dead Pixels";
            IsEnabled = true;
            _cameraInfo = cameraSupervisor.GetCameraInfo(waferSide);
            _whitePixelsExposureTimeMs = GetWhitePixelExposureTimeFromHardwareLimit();
            _blackPixelsExposureTimeMs = GetBlackPixelExposureTimeFromCalibrationInputs();

            _deadPixelsManager = new DeadPixelsManager();
        }

        #region ITabManager implementation

        public void Display()
        {
        }

        public bool CanHide()
        {
            // if there is something to save
            if ((_whiteDeadPixelsSearched && _blackDeadPixelsSearched) && !_deadPixelsSaved)
            {
                if (DialogService.ShowMessageBox($"You have not saved the dead pixels. Do you want to save them before leaving ?", "Save DeadPixels", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    DoSaveDeadPixels();
            }
            return true;
        }

        public void Hide()
        {
        }

        #endregion ITabManager implementation

        public int WhitePixelsThreshold { get; set; } = 150;

        public int BlackPixelsThreshold { get; set; } = 10;

        public int? NbWhiteDeadPixels => _deadPixelsManager.WhiteDeadPixels?.Count;

        public int? NbBlackDeadPixels => _deadPixelsManager.BlackDeadPixels?.Count;

        private bool _whiteDeadPixelsSearched => NbWhiteDeadPixels != null;

        private bool _blackDeadPixelsSearched => NbBlackDeadPixels != null;

        private bool _deadPixelsSaved = false;

        private DeadPixelTypes _currentDeadPixelType = DeadPixelTypes.WhitePixel;

        private BitmapSource _cameraBitmapSourceWhite;

        private BitmapSource _cameraBitmapSourceBlack;

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        { get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); OnPropertyChanged(nameof(ImageWidth)); OnPropertyChanged(nameof(ImageHeight)); } } }

        public double ImageWidth { get => (_cameraBitmapSource == null) ? 0 : _cameraBitmapSource.Width; }

        public double ImageHeight { get => (_cameraBitmapSource == null) ? 0 : _cameraBitmapSource.Height; }

        private double _whitePixelsExposureTimeMs;

        public double WhiteDeadPixelsExposureTimeMs
        {
            get => _whitePixelsExposureTimeMs;
            set
            {
                if (_whitePixelsExposureTimeMs != value)
                {
                    _whitePixelsExposureTimeMs = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _blackPixelsExposureTimeMs;

        public double BlackDeadPixelsExposureTimeMs
        {
            get => _blackPixelsExposureTimeMs;
            set
            {
                if (_blackPixelsExposureTimeMs != value)
                {
                    _blackPixelsExposureTimeMs = value;
                    OnPropertyChanged();
                }
            }
        }

        private CameraInfo _cameraInfo;

        public CameraInfo CameraInformation => _cameraInfo;

        public DeadPixelTypes CurrentDeadPixelType
        {
            get
            {
                return _currentDeadPixelType;
            }

            set
            {
                if (_currentDeadPixelType == value)
                {
                    return;
                }

                _currentDeadPixelType = value;

                UpdateCurrentDeadPixels();
                UpdateCurentBitmap();

                OnPropertyChanged();
            }
        }

        private RelayCommand _startDeadPixelsSearch;

        public RelayCommand StartDeadPixelsSearch
        {
            get
            {
                return _startDeadPixelsSearch ?? (_startDeadPixelsSearch = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => AcquireImageAndSearchDeadPixels());
                    }
                ));
            }
        }

        private RelayCommand _saveDeadPixels;

        public RelayCommand SaveDeadPixels
        {
            get
            {
                return _saveDeadPixels ?? (_saveDeadPixels = new RelayCommand(
                    () =>
                    {
                        DoSaveDeadPixels();
                    },
                    () => { return _whiteDeadPixelsSearched && _blackDeadPixelsSearched && !_deadPixelsSaved; }
                ));
            }
        }

        private void DoSaveDeadPixels()
        {
           
            try
            {
                _calibrationSupervisor.UpdateAndSaveDeadPixels(WaferSide);
            }
            catch (Exception e)
            {
                DialogService.ShowMessageBox($"Unable to update dead pixels for {WaferSide}: {e.Message}", "Update DeadPixels", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _deadPixelsSaved = true;
            SaveDeadPixels.NotifyCanExecuteChanged();
        }

        private void AcquireImageAndSearchDeadPixels()
        {
            if (CurrentDeadPixelType == DeadPixelTypes.WhitePixel)
            {
                _deadPixelsManager.WhiteDeadPixels = null;
            }
            else
            {
                _deadPixelsManager.BlackDeadPixels = null;
            }

            IsBusy = true;

            var exposureTimeMs = _currentDeadPixelType == DeadPixelTypes.WhitePixel ? WhiteDeadPixelsExposureTimeMs : BlackDeadPixelsExposureTimeMs;
            CameraSupervisor.SetExposureTime(WaferSide, exposureTimeMs);
            if (_currentDeadPixelType == DeadPixelTypes.BlackPixel)
            {
                ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, false);
            }

            ServiceImageWithDeadPixels svcimg = null;
            try
            {
                svcimg = _calibrationSupervisor.AcquireDeadPixelsImageForSideAndType(WaferSide, CurrentDeadPixelType, (CurrentDeadPixelType == DeadPixelTypes.WhitePixel) ? WhitePixelsThreshold : BlackPixelsThreshold);
            }
            catch (Exception ex)
            {
                var message = $"Dead pixels calibration failed :\n{ex.Message}";
                Application.Current.Dispatcher.Invoke(() =>
                    DialogService.ShowMessageBox(message, "Dead Pixels Calibration",
                        MessageBoxButton.OK, MessageBoxImage.Error));
            }
            finally
            {
                if (_currentDeadPixelType == DeadPixelTypes.BlackPixel)
                {
                    ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black, false);
                }
            }
            if (svcimg == null)
            {
                IsBusy = false;
                return;
            }
            if (svcimg.CalibrationStatus != DeadPixelsCalibrationStatus.Success)
            {
                IsBusy = false;
                var deadPixelTypeString = CurrentDeadPixelType == DeadPixelTypes.WhitePixel ? "white" : "black";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DialogService.ShowMessageBox($"Too many {deadPixelTypeString} found (number of dead pixels found: {svcimg.NumberOfDeadPixelsFound}, maximum threshold for current camera: {svcimg.MaximumDeadPixelThreshold}). Cannot compute calibration.", "Dead pixels search", MessageBoxButton.OK, MessageBoxImage.Error); ;
                });
                return;
            }

            if (CurrentDeadPixelType == DeadPixelTypes.WhitePixel)
            {
                _cameraBitmapSourceWhite = svcimg.Image.WpfBitmapSource;
                _cameraBitmapSourceWhite.Freeze();
                _deadPixelsManager.WhiteDeadPixels = svcimg.DeadPixels;
            }
            else
            {
                _cameraBitmapSourceBlack = svcimg.Image.WpfBitmapSource;
                _cameraBitmapSourceBlack.Freeze();
                _deadPixelsManager.BlackDeadPixels = svcimg.DeadPixels;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateCurrentDeadPixels();
                UpdateCurentBitmap();
                _deadPixelsSaved = false;
                SaveDeadPixels.NotifyCanExecuteChanged();
            });

            IsBusy = false;
        }

        private void UpdateCurrentDeadPixels()
        {
            DeadPixels.Clear();

            if (_currentDeadPixelType == DeadPixelTypes.WhitePixel)
            {
                if (_deadPixelsManager.WhiteDeadPixels != null)
                {
                    foreach (var deadPixel in _deadPixelsManager.WhiteDeadPixels)
                    {
                        DeadPixels.Add(new SelectableDeadPixel(deadPixel));
                    }
                }

                OnPropertyChanged(nameof(NbWhiteDeadPixels));
            }
            else
            {
                if (_deadPixelsManager.BlackDeadPixels != null)
                {
                    foreach (var deadPixel in _deadPixelsManager.BlackDeadPixels)
                    {
                        DeadPixels.Add(new SelectableDeadPixel(deadPixel));
                    }
                }

                OnPropertyChanged(nameof(NbBlackDeadPixels));
            }
        }

        private void UpdateCurentBitmap()
        {
            if (_currentDeadPixelType == DeadPixelTypes.WhitePixel)
            {
                CameraBitmapSource = _cameraBitmapSourceWhite;
            }
            else
                CameraBitmapSource = _cameraBitmapSourceBlack;
        }

        private double GetWhitePixelExposureTimeFromHardwareLimit()
        {
            return _cameraInfo.MaxExposureTimeMs;
        }

        private double GetBlackPixelExposureTimeFromCalibrationInputs()
        {
            return _calibrationSupervisor.GetDefaultBlackDeadPixelCalibrationExposureTime(WaferSide);
        }
    }
}
