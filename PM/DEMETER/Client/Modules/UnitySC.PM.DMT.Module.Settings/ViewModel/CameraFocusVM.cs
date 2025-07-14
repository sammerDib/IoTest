using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class CameraFocusVM : SettingWithVideoStreamVM, ITabManager
    {
        private int SubImagesNumber;

        public CameraFocusVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            IDialogOwnerService dialogService, ILogger logger, IMessenger messenger)
            : base(waferSide, exposureSettings, cameraSupervisor, screenSupervisor, messenger, dialogService, logger)
        {
            Header = "Camera Focus";
            switch (cameraSupervisor.GetOpticalMountShape(waferSide))
            {
                case Service.Interface.OpticalMount.OpticalMountShape.Cross:
                case Service.Interface.OpticalMount.OpticalMountShape.SquarePlusCenter:
                    SubImagesNumber = 5;
                    break;

                default:
                    SubImagesNumber = 3;
                    break;
            }
        }

        #region ITabManager implementation

        public void Display()
        {
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);

            IsActive = true;

            StartGrab(NeedsToStartGrabOnDisplay);

            _focusDataItems = new List<FocusDataVM>();
            for (int i = 0; i < SubImagesNumber; i++)
            {
                _focusDataItems.Add(new FocusDataVM());
            }
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;
        }

        public bool CanHide() => true;

        public void Hide()
        {
            StopGrab(NeedsToStopGrabOnHiding);
            IsActive = false;
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;
        }

        #endregion ITabManager implementation

        private void ExposureSettings_AutoExposureTerminated(object sender, EventArgs e)
        {
            IsBusy = false;
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);
            StartGrab();
        }

        private void ExposureSettings_AutoExposureStarted(object sender, EventArgs e)
        {
            BusyMessage = "Computing Exposure";
            IsBusy = true;
            StopGrab();
        }

        private List<FocusDataVM> _focusDataItems;

        public List<FocusDataVM> FocusDataItems
        {
            get => _focusDataItems; set { if (_focusDataItems != value) { _focusDataItems = value; OnPropertyChanged(); } }
        }

        private int _waferSize = 300;

        public int WaferSize
        {
            get => _waferSize; set { if (_waferSize != value) { _waferSize = value; OnPropertyChanged(); } }
        }

        private int _patternSize = 500;

        public int PatternSize
        {
            get => _patternSize; set { if (_patternSize != value) { _patternSize = value; OnPropertyChanged(); } }
        }

        protected override ServiceImage GetCameraImage(double scale, ROI roi)
        {
            return CameraSupervisor.GetImageWithFocus(WaferSide, scale, WaferSize, PatternSize);
        }

        protected override void DisplayCameraImage(ServiceImage svcimage)
        {
            if (!IsGrabbing)   // Ne pas mettre à jour l'IHM si on est en train de fermer la vue
                return;

            if (svcimage == null)
                return;

            var bitmapImage = svcimage.WpfBitmapSource;
            var subImagesProperties = ((ServiceImageWithFocus)svcimage).SubImagesProperties;

            foreach ((int index, var subImageProperty) in subImagesProperties.Select((item, index) => (index, item)))
            {
                _focusDataItems[index].UpdateFrom(subImageProperty);
            }
            ImageWidth = (int)bitmapImage.Width;
            ImageHeight = (int)bitmapImage.Height;
            CameraBitmapSource = bitmapImage;
        }

        private AutoRelayCommand _reset;

        public AutoRelayCommand Reset
        {
            get
            {
                return _reset ?? (_reset = new AutoRelayCommand(
                    () =>
                    {
                        foreach (var focusDataItem in _focusDataItems)
                        {
                            focusDataItem.Reset();
                        }
                    },
                    () => { return true; }
                ));
            }
        }
    }
}
