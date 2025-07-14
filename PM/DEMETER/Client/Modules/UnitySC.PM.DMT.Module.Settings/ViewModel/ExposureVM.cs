using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class ExposureVM : SettingVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;

        public ExposureVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger)
            : base(waferSide, cameraSupervisor, screenSupervisor, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;

            Header = "Exposure Time";
            IsEnabled = true;
            CamCalibAcquisitionVm = new CamCalibAcquisitionVM("Bright-field image");
        }
        
        #region ITabManager implementation

        public void Display()
        {
            IsActive = true;
            ExecuteCalibrationCommand.NotifyCanExecuteChanged();
            GetGoldenValuesCommand.NotifyCanExecuteChanged();
            if (ExposureMatchingInputs is null)
            {
                ExposureMatchingInputs = _calibrationSupervisor.GetExposureMatchingInputs(WaferSide);    
            }
            
        }

        public bool CanHide()
        {
            return !IsBusy;
        }

        public void Hide()
        {
            IsActive = false;
            _calibrationSupervisor.RemoveBrightFieldImage(WaferSide);
            CamCalibAcquisitionVm.IsAcquired = false;
            LastAcquiredImage = null;
            CalibrationResultMessage = "";
        }

        #endregion ITabManager implementation
        
        private ExposureMatchingInputs _exposureMatchingInputs;

        public ExposureMatchingInputs ExposureMatchingInputs
        {
            get => _exposureMatchingInputs;
            set
            {
                if (_exposureMatchingInputs != value)
                {
                    _exposureMatchingInputs = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(GoldenValues));
                }
            }
        }

        public ExposureMatchingGoldenValues GoldenValues => ExposureMatchingInputs.GoldenValuesBySide[WaferSide];

        private CamCalibAcquisitionVM _camCalibAcquisition;
        
        public CamCalibAcquisitionVM CamCalibAcquisitionVm
        {
            get => _camCalibAcquisition;
            set
            {
                if (_camCalibAcquisition != value)
                {
                    _camCalibAcquisition = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private BitmapSource _lastAcquiredImage;

        public BitmapSource LastAcquiredImage
        {
            get => _lastAcquiredImage;
            set
            {
                _lastAcquiredImage = value;
                OnPropertyChanged();
            }
        }

        private string _calibrationResult = "";
        
        public string CalibrationResultMessage
        {
            get => _calibrationResult;
            set
            {
                if (_calibrationResult != value)
                {
                    _calibrationResult = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCalibrationSuccessful = true;
        
        public bool IsCalibrationSuccessful
        {
            get => _isCalibrationSuccessful;
            set
            {
                if (_isCalibrationSuccessful != value)
                {
                    _isCalibrationSuccessful = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isGoldenTool;
        
        public bool IsGoldenTool
        {
            get => _isGoldenTool;
            set
            {
                if (_isGoldenTool != value)
                {
                    _isGoldenTool = value;
                    OnPropertyChanged();
                    (value ? GetGoldenValuesCommand : ExecuteCalibrationCommand).NotifyCanExecuteChanged();
                }
            }
        }

        private RelayCommand _importGoldenToolInputsCommand;

        public RelayCommand ImportGoldenToolInputsCommand
        {
            get => _importGoldenToolInputsCommand ?? (_importGoldenToolInputsCommand = new RelayCommand(() =>
            {
                var openFileDialogSettings = new OpenFileDialogSettings
                {
                    Multiselect = false,
                    DefaultExt = ".xml",
                    Filter = "XML File|*.xml",
                    CheckFileExists = true,
                    Title = "Select a Demeter Golden Values file",
                    InitialDirectory =
                        Path.GetFullPath(Path.Combine(_calibrationSupervisor.GetCalibrationBaseFolder(), @"\..\..")),
                };
                if (DialogService.ShowOpenFileDialog(openFileDialogSettings).GetValueOrDefault(false))
                {
                    string filePath = openFileDialogSettings.FileName;
                    try
                    {
                        var inputs = XML.Deserialize<ExposureMatchingInputs>(filePath);
                        if (!inputs.GoldenValuesBySide.ContainsKey(WaferSide))
                        {
                            DialogService.ShowMessageBox(
                                $"The provided file is for {inputs.GoldenValues[0].Side} side and you're trying to calibrate the {WaferSide} side. Please check you've chosen the correct file.",
                                "Side mismatch", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            ExposureMatchingInputs = inputs;
                        }
                    }
                    catch (Exception e)
                    {
                        DialogService.ShowMessageBox("Error parsing Exposure matching inputs file: " + e.Message,
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }));
        }

        private async Task<bool> AcquireImageAsync()
        {
            BusyMessage = "Acquiring image";
            try
            {
                var lastAcquiredImage = await _calibrationSupervisor.AcquireBrightFieldImageAsync(WaferSide,
                    ExposureMatchingInputs.AcquisitionExposureTimeMs);
                lastAcquiredImage.WpfBitmapSource.Freeze();
                LastAcquiredImage = lastAcquiredImage.WpfBitmapSource;
                _camCalibAcquisition.IsAcquired = true;
                return true;
            }
            catch (Exception ex)
            {
                DialogService.ShowMessageBox($"Failed to acquire the calibration image :\n{ex.Message}",
                    "Image Acquisition", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private AsyncRelayCommand _executeCalibrationCommand;
        
        public AsyncRelayCommand ExecuteCalibrationCommand =>
            _executeCalibrationCommand ?? (_executeCalibrationCommand = new AsyncRelayCommand(
                async () =>
                {
                    try
                    {
                        IsBusy = true;
                        BusyMessage = "Acquiring image";
                        if (await AcquireImageAsync())
                        {
                            BusyMessage = "Computing calibration coefficient";
                            double calibrationResultValue =
                                _calibrationSupervisor.CalibrateExposure(WaferSide, ExposureMatchingInputs);
                            CalibrationResultMessage =
                                $"Calibration successful. Resulting exposure matching coefficient: {calibrationResultValue:0.000}";
                            IsCalibrationSuccessful = true;    
                        }
                        else
                        {
                            IsCalibrationSuccessful = false;
                            CalibrationResultMessage = "Calibration failed. Unable to acquire calibration image.";
                        }
                    }
                    catch (Exception ex)
                    {
                        IsCalibrationSuccessful = false;
                        CalibrationResultMessage =
                            $"Calibration failed. Unable to compute exposure matching coefficient : {ex.Message}";
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                },
                () => !_isGoldenTool));
        
        private AsyncRelayCommand _getGoldenValuesCommand;

        public AsyncRelayCommand GetGoldenValuesCommand =>
            _getGoldenValuesCommand ?? (_getGoldenValuesCommand = new AsyncRelayCommand(
                async () =>
                {
                    IsBusy = true;
                    BusyMessage = "Acquiring image";
                    if (await AcquireImageAsync())
                    {
                        BusyMessage = "Computing golden values";
                        var inputs = _calibrationSupervisor.GetGoldenValues(WaferSide);
                        if (!(inputs is null))
                        {
                            var saveSettings = new SaveFileDialogSettings
                            {
                                DefaultExt = ".xml",
                                FileName = $"DMT-GoldenTool-{WaferSide}-ExposureMatchingInputs.xml",
                                Filter = "XML Files|*.xml",
                                Title = "Save exposure time matching golden values to",
                                InitialDirectory = Path.GetFullPath(Path.Combine(_calibrationSupervisor.GetCalibrationBaseFolder(), @"..\.."))
                            };
                            if (DialogService.ShowSaveFileDialog(saveSettings).GetValueOrDefault(false))
                            {
                                try
                                {
                                    inputs.Serialize(saveSettings.FileName);
                                    DialogService.ShowMessageBox($"Successfully saved golden values file to {saveSettings.FileName}", "Save successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                catch (Exception e)
                                {
                                    DialogService.ShowMessageBox($"Error while saving golden values file : {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }

                    IsBusy = false;
                },
                () => _isGoldenTool));
    }
}
