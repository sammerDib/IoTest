using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Modules.TestApps.Acquisition;
using UnitySC.PM.EME.Client.Modules.TestApps.Camera;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.TestApps
{
    public class TestAppsVM : ObservableRecipient, IMenuContentViewModel
    {
        #region Properties

        public bool IsEnabled => true;

        private bool _canCloseState = true;
        public bool CanCloseState
        {
            get => _canCloseState; set { if (_canCloseState != value) { _canCloseState = value; OnPropertyChanged(); } }
        }

        private CameraViewModel _cameraViewModel;
        public CameraViewModel CameraViewModel
        {
            get
            {
                if (_cameraViewModel == null)
                {
                    var cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
                    var messenger = ClassLocator.Default.GetInstance<IMessenger>();
                    _cameraViewModel = new CameraViewModel(cameraBench, messenger);
                }
                return _cameraViewModel;
            }
        }

        private AcquisitionViewModel _acquisitionViewModel;

        public AcquisitionViewModel AcquisitionViewModel
        {
            get
            {
                if (_acquisitionViewModel == null)
                {
                    var cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
                    var filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
                    var messenger = ClassLocator.Default.GetInstance<IMessenger>();
                    _acquisitionViewModel = new AcquisitionViewModel(cameraBench, filterWheelBench, messenger);
                }
                return _acquisitionViewModel;
            }
        }

        private string _calibrationPath;
        public string CalibrationPath
        {
            get
            {
                if (_calibrationPath == null)
                {
                    var calibrationSupervisor = ClassLocator.Default.GetInstance<ICalibrationService>();
                    _calibrationPath = calibrationSupervisor.GetCalibrationPath().Result;
                }
                return _calibrationPath;
            }
        }
        #endregion

        public bool CanClose()
        {           
            if (!CanCloseState)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't close");
                return CanCloseState;
            }
            AcquisitionViewModel.Close();

            return CanCloseState;
        }

        public void Refresh()
        {
            AcquisitionViewModel.Refresh();
        }
    }
}
