using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class PerspectiveVM : SettingVM, ITabManager, IRecipient<RecipeMessage>
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;

        public PerspectiveVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;

            Header = "Perspective";
            IsEnabled = true;

            ExposureSettings = exposureSettings;
        }

        private string _tempFolder = "c:\\Temp\\DEMETER";

        private bool _isImageAcquired = false;
        private bool _isCalibrationDone = false;

        private string _imageFile;
        private string _calibFile;
        private bool _isAcquiringImage = false;
        public ExposureSettingsWithAutoVM ExposureSettings { get; set; }

        private void ExposureSettings_AutoExposureTerminated(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        private void ExposureSettings_AutoExposureStarted(object sender, EventArgs e)
        {
            BusyMessage = "Computing Exposure";
            IsBusy = true;
        }

        #region ITabManager implementation

        public void Display()
        {
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;

            IsActive = true;
        }

        public bool CanHide() => (!IsBusy);

        public void Hide()
        {
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;

            IsActive = false;
        }

        #endregion ITabManager implementation

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        { get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); } } }

        private string _calibWaferXmlFile;

        public string CalibWaferXmlFile
        {
            get => _calibWaferXmlFile; set { if (_calibWaferXmlFile != value) { _calibWaferXmlFile = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _browseCalibWaferXmlFile;

        public AutoRelayCommand BrowseCalibWaferXmlFile
        {
            get
            {
                return _browseCalibWaferXmlFile ?? (_browseCalibWaferXmlFile = new AutoRelayCommand(
                    () =>
                    {
                        var defaultPath = Path.Combine(Directory.GetParent(_calibrationSupervisor.GetCalibrationBaseFolder()).FullName, @"CalibrationInputs\Perspective");

                        // I didn't succeed to set the initial directory with DialogService.ShowOpenFileDialog
                        OpenFileDialog openFileDialog = new OpenFileDialog();

                        openFileDialog.Filter = "Xml|*.xml";
                        openFileDialog.InitialDirectory = defaultPath;
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == true)
                        {
                            CalibWaferXmlFile = openFileDialog.FileName;
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _acquireImage;

        public AutoRelayCommand AcquireImage
        {
            get
            {
                return _acquireImage ?? (_acquireImage = new AutoRelayCommand(
                    () =>
                    {
                        _isAcquiringImage = true;
                        PrepareAcquireImage();
                    },
                    () => { return true; }
                ));
            }
        }

        private async void PrepareAcquireImage()
        {
            BusyMessage = "Acquiring Image";
            IsBusy = true;

            ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, false);

            await Task.Run(() =>
            {
                Application.Current?.Dispatcher.Invoke(() => AcquireOneImage());
            });
        }

        private void AcquireOneImage()
        {
            CameraSupervisor.SetExposureTime(WaferSide, ExposureSettings.ExposureTimeMs);

            var svcimg = CameraSupervisor.GetCameraImage(WaferSide);
            if (svcimg == null)
                return;

            var bitmapSource = svcimg.WpfBitmapSource;
            bitmapSource.Freeze();

            if (svcimg.IsSaturated)
            {
                DialogService.ShowMessageBox($"The acquired image is saturated, you should decrease the exposure time", "Image Acquisition", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var app = Application.Current;
            app.Dispatcher.Invoke(new Action(() => { CameraBitmapSource = bitmapSource; }));

            try
            {
                Directory.CreateDirectory(_tempFolder);

                _imageFile = Path.Combine(_tempFolder, $"PerspCalibImage_{Enum.GetName(typeof(Side), WaferSide)}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.tif");

                svcimg.SaveToFile(_imageFile);
            }
            catch (Exception)
            {
                DialogService.ShowMessageBox($"Failed to save the image to the file {_imageFile}", "Image Acquisition", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black, false);
            _isImageAcquired = true;
            _isAcquiringImage = false;
            _isCalibrationDone = false;
            IsBusy = false;
        }

        public void Receive(RecipeMessage message)
        {
            if (!_isAcquiringImage)
                return;

            switch (message.Status.State)
            {
                case DMTRecipeState.ExecutionComplete:
                    Task.Run(() =>
                    {
                        Application.Current?.Dispatcher.Invoke(() => AcquireOneImage());
                    });
                    break;

                case DMTRecipeState.Executing:
                    break;

                default:
                    // Afficher un message pour dire que l'autoexposure a échoué
                    DialogService.ShowMessageBox($"The auto exposure failed :\n{message.Status.Message}", "AutoExposure", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsBusy = false;
                    _isAcquiringImage = false;
                    break;
            }
        }

        private AutoRelayCommand _executeCalibration;

        public AutoRelayCommand ExecuteCalibration
        {
            get
            {
                return _executeCalibration ?? (_executeCalibration = new AutoRelayCommand(
                    () =>
                    {
                        BusyMessage = "Calibrating";
                        IsBusy = true;

                        var task = LaunchCalibrationTool();
                    },
                    () => _isImageAcquired && !string.IsNullOrEmpty(_calibWaferXmlFile)
                ));
            }
        }

        private async Task LaunchCalibrationTool()
        {
            _calibFile = Path.Combine(_tempFolder, $"PerspCalib_{Enum.GetName(typeof(Side), WaferSide)}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.DMT");

            // Part 1: use ProcessStartInfo class.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "ADCCalibDMT\\ADCCalibDMT.exe";

            // Part 2: set arguments.
            startInfo.Arguments = $"-image \"{_imageFile}\" -xml \"{_calibWaferXmlFile}\" -ouput \"{_calibFile}\"";
            Logger.Information("ADCCalib Arguments: " + startInfo.Arguments);

            await Task.Run(() =>
            {
                try
                {
                    // Part 3: start with the info we specified.
                    // ... Call WaitForExit.
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                        // If the calibration file is present
                        if (File.Exists(_calibFile))
                        {
                            _isCalibrationDone = true;
                            Application.Current?.Dispatcher.Invoke(() => ApplyCalibration());
                        }
                    }
                }
                catch
                {
                }
            });

            IsBusy = false;

            if (!_isCalibrationDone)
            {
                DialogService.ShowMessageBox($"The calibration failed", "Perspective Calibration", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Error("Perspective Calibration failed");
            }
        }

        private AutoRelayCommand _useCalibration;

        public AutoRelayCommand UseCalibration
        {
            get
            {
                return _useCalibration ?? (_useCalibration = new AutoRelayCommand(
                    () =>
                    {
                        ApplyCalibration();

                        // Code to execute
                    },
                    () => { return _isCalibrationDone; }
                ));
            }
        }

        private void ApplyCalibration()
        {
            // We retrieve the path to the DEMETER calib file
            var serverCalibFilePath = _calibrationSupervisor.GetPerspectiveCalibrationFullFilePath(WaferSide);

            var serverCalibFileFullPath = Path.GetFullPath(serverCalibFilePath);

            try
            {
                File.Copy(_calibFile, serverCalibFileFullPath, true);
                _calibrationSupervisor.ReloadPerspectiveCalibrationForSide(WaferSide);
            }
            catch (Exception ex)
            {
                DialogService.ShowMessageBox($"The copy of the calibration file to {serverCalibFileFullPath} failed", "Perspective Calibration", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Error(ex, $"The copy of the calibration file to {serverCalibFileFullPath} failed");
                return;
            }
        }
    }

    public static class ProcessExtensions
    {
        public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void Process_Exited(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;

            try
            {
                if (process.HasExited)
                {
                    return;
                }

                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                process.Exited -= Process_Exited;
            }
        }
    }
}
