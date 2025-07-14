using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public enum ExposureTypes
    {
        Auto,
        Manual
    }

    public class SettingVM : ObservableRecipient, ISettingVM
    {
        protected ScreenSupervisor ScreenSupervisor;
        protected CameraSupervisor CameraSupervisor;
        protected IDialogOwnerService DialogService;
        protected ILogger Logger;

        public SettingVM(Side waferSide, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, IDialogOwnerService dialogService, ILogger logger)
        {
            CameraSupervisor = cameraSupervisor;
            ScreenSupervisor = screenSupervisor;
            DialogService = dialogService;
            Logger = logger;
            WaferSide = waferSide;
        }

        /// <summary> Header dans l'IHM </summary>
        public string Header { get; protected set; }

        public Side WaferSide { get; protected set; }

        public bool IsEnabled { get; protected set; } = false;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "Calibrating";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }
    }
}
