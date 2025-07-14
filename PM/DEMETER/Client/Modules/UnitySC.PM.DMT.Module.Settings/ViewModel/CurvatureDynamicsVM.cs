using System;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class CurvatureDynamicsVM : SettingWithVideoStreamVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;

        private bool _closedTab = false;

        public CurvatureDynamicsVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger, IMessenger messenger)
            : base(waferSide, exposureSettings, cameraSupervisor, screenSupervisor, messenger, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;
            Header = "Curvature Dynamics";
        }

        #region ITabManager implementation

        public void Display()
        {
            IsActive = true;
            StartGrab(NeedsToStartGrabOnDisplay);
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);
        }

        public bool CanHide() => (!IsBusy);

        public void Hide()
        {
            _closedTab = true;
            IsActive = false;
            StopGrab(NeedsToStopGrabOnHiding);
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
        }

        #endregion ITabManager implementation

        private AsyncRelayCommand _executeCalibrationAsyncCommand;

        public AsyncRelayCommand ExecuteCalibrationAsyncCommand
        {
            get
            {
                return _executeCalibrationAsyncCommand ?? (_executeCalibrationAsyncCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        BusyMessage = "Calibrating";
                        StopGrab();
                        try
                        {
                            float calibrationCoefficient =
                                await _calibrationSupervisor.CalibrateCurvatureDynamicsAsync(WaferSide);
                            Application.Current.Dispatcher.Invoke(() =>
                                                                      DialogService
                                                                          .ShowMessageBox($"Curvature Calibration is completed successfully. The coefficient is {calibrationCoefficient:F5}",
                                                                           "Curvature Calibration",
                                                                           MessageBoxButton.OK,
                                                                           MessageBoxImage.Information));
                        }
                        catch (Exception ex)
                        {
                            var message = $"Curvature Calibration failed :\n{ex.Message}";
                            Application.Current.Dispatcher.Invoke(() =>
                                                                      DialogService
                                                                          .ShowMessageBox(message,
                                                                           "Curvature Calibration",
                                                                           MessageBoxButton.OK,
                                                                           MessageBoxImage.Error));
                        }
                        finally
                        {
                            if (!_closedTab)
                            {
                                StartGrab();
                                ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);
                            }
                        }
                    }
                ));
            }
        }
    }
}
