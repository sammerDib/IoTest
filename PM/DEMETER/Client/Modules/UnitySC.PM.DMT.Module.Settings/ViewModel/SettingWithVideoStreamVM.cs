using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class SettingWithVideoStreamVM : VideoStreamVM, ISettingVM
    {
        protected const int MinImageSize = 10;
        protected readonly ILogger Logger;
        protected readonly IDialogOwnerService DialogService;
        public string Header { get; protected set; }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _busyMessage;

        public string BusyMessage
        {
            get => _busyMessage;
            set => SetProperty(ref _busyMessage, value);
        }

        private bool _needsToStopGrabOnHiding = true;

        public bool NeedsToStopGrabOnHiding
        {
            get => _needsToStopGrabOnHiding;
            set => SetProperty(ref _needsToStopGrabOnHiding, value);
        }

        private bool _needsToStartGrabOnDisplay = true;

        public bool NeedsToStartGrabOnDisplay
        {
            get => _needsToStartGrabOnDisplay;
            set => SetProperty(ref _needsToStartGrabOnDisplay, value);
        }

        protected ScreenInfo ScreenProperties { get; set; }

        public SettingWithVideoStreamVM(Side position, ExposureSettingsWithAutoVM exposureSettings,
            CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, IMessenger messenger,
            IDialogOwnerService dialogService, ILogger logger)
            : base(position, cameraSupervisor, screenSupervisor, messenger)
        {
            ExposureSettings = exposureSettings;
            ScreenProperties = ScreenSupervisor.GetScreenInfo(position);
            Logger = logger;
            DialogService = dialogService;
        }

        protected override ServiceImage GetCameraImage(double scale, ROI roi)
        {
            return CameraSupervisor.GetCameraImage(WaferSide);
        }

        protected override void DisplayCameraImage(ServiceImage svcimage)
        {
            if (!IsGrabbing)
                return;

            if (svcimage == null)
                return;

            ImageHeight = svcimage.DataHeight;
            ImageWidth = svcimage.DataWidth;
            var bitmapImage = svcimage.WpfBitmapSource;
            CameraBitmapSource = bitmapImage;
        }
    }
}
