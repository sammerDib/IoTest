using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Extensions;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class AlignmentVM : SettingWithVideoStreamVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;

        private double _horizontalCrossOffset;

        private double _horizontalLine1Position = double.NaN;

        private double _horizontalLine2Position = double.NaN;

        private bool _isCrossDisplayedOnScreen;
        private readonly bool _isGrabbing = true;

        private double _verticalLine1Position = double.NaN;

        private double _verticalLine2Position = double.NaN;

        public AlignmentVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger, IMessenger messenger)
            : base(waferSide, exposureSettings, cameraSupervisor, screenSupervisor, messenger, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;
            Header = "Alignment";
        }

        public bool IsCrossDisplayedOnScreen
        {
            get => _isCrossDisplayedOnScreen;
            set
            {
                if (_isCrossDisplayedOnScreen != value)
                {
                    _isCrossDisplayedOnScreen = value;
                    UpdateScreen();
                    OnPropertyChanged();
                }
            }
        }

        public double HorizontalCrossOffset
        {
            get => _horizontalCrossOffset;
            set
            {
                if (_horizontalCrossOffset != value)
                {
                    _horizontalCrossOffset = value;
                    UpdateScreen();
                    OnPropertyChanged();
                }
            }
        }

        public double HorizontalLine1Position
        {
            get => _horizontalLine1Position;
            set
            {
                if (_horizontalLine1Position != value)
                {
                    _horizontalLine1Position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(VerticalDistance));
                    CenterHorizontalLine.NotifyCanExecuteChanged();
                }
            }
        }

        public double HorizontalLine2Position
        {
            get => _horizontalLine2Position;
            set
            {
                if (_horizontalLine2Position != value)
                {
                    _horizontalLine2Position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(VerticalDistance));
                }
            }
        }

        public double HorizontalDistance => double.IsNaN(Math.Abs(VerticalLine2Position - VerticalLine1Position))
            ? 0
            : Math.Abs(VerticalLine2Position - VerticalLine1Position);

        public double VerticalDistance => double.IsNaN(Math.Abs(HorizontalLine2Position - HorizontalLine1Position))
            ? 0
            : Math.Abs(HorizontalLine2Position - HorizontalLine1Position);

        public double VerticalLine1Position
        {
            get => _verticalLine1Position;
            set
            {
                if (_verticalLine1Position != value)
                {
                    _verticalLine1Position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HorizontalDistance));
                    CenterVerticalLine.NotifyCanExecuteChanged();
                }
            }
        }

        public double VerticalLine2Position
        {
            get => _verticalLine2Position;
            set
            {
                if (_verticalLine2Position != value)
                {
                    _verticalLine2Position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HorizontalDistance));
                }
            }
        }

        private void UpdateScreen()
        {
            if (_isCrossDisplayedOnScreen)
            {
                if (ScreenProperties != null)
                {
                    int thickness = _calibrationSupervisor.GetAlignmentVerticalLineThicknessInPixels();
                    double HorizontalSize =
                        (ScreenProperties.Width - 2 * thickness) * ScreenProperties.PixelPitchHorizontal;

                    // check if horizontal offset is valid
                    if (HorizontalCrossOffset > HorizontalSize / 2)
                    {
                        HorizontalCrossOffset = HorizontalSize / 2;
                        return;
                    }

                    if (HorizontalCrossOffset < -HorizontalSize / 2)
                    {
                        HorizontalCrossOffset = -HorizontalSize / 2;
                        return;
                    }

                    var screenImage =
                        CreateScreenImage(
                            (int)(ScreenProperties.Width / 2 +
                                  HorizontalCrossOffset / ScreenProperties.PixelPitchHorizontal), thickness);

                    ScreenSupervisor.DisplayImage(WaferSide, screenImage);
                }
            }
            else
            {
                ScreenSupervisor.SetScreenColor(WaferSide, Colors.White, true);
            }
        }

        private BitmapSource CreateScreenImage(int horizontalPos, int thickness)
        {
            int imageWidth = ScreenProperties.Width;
            int imageHeight = ScreenProperties.Height;

            // Create the Rectangle
            var visual = new DrawingVisual();
            var context = visual.RenderOpen();

            context.DrawRectangle(Brushes.Black, null, new Rect(0, 0, imageWidth, imageHeight));
            context.DrawLine(new Pen(new SolidColorBrush(Colors.White), thickness), new Point(horizontalPos, 0),
                new Point(horizontalPos, imageHeight));
            context.Close();

            // Create the Bitmap and render the rectangle onto it.
            var bmp = new RenderTargetBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);

            var bitmap24 = bmp.ConvertToRGB24();

            return bitmap24;
        }

        private void ExposureSettings_AutoExposureTerminated(object sender, EventArgs e)
        {
            IsBusy = false;
            UpdateScreen();
            StartGrab();
        }

        private void ExposureSettings_AutoExposureStarted(object sender, EventArgs e)
        {
            BusyMessage = "Computing Exposure";
            IsBusy = true;
            StopGrab();
        }

        #region ITabManager implementation

        public void Display()
        {
            IsActive = true;
            StartGrab(NeedsToStartGrabOnDisplay);

            if (ScreenProperties == null)
            {
                ScreenProperties = ScreenSupervisor.GetScreenInfo(WaferSide);
                if (ScreenProperties == null)
                    Logger.Error("Get screen infos failed");
            }

            if (double.IsNaN(HorizontalLine1Position))
            {
                HorizontalLine1Position = 0;
            }

            if (double.IsNaN(HorizontalLine2Position))
            {
                HorizontalLine2Position = ScreenProperties.Width / 2;
            }

            if (double.IsNaN(VerticalLine1Position))
            {
                VerticalLine1Position = 0;
            }

            if (double.IsNaN(VerticalLine2Position))
            {
                VerticalLine2Position = ScreenProperties.Height / 2;
            }
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;
            UpdateScreen();
        }

        public bool CanHide()
        {
            return true;
        }

        public void Hide()
        {
            IsBusy = false;
            StopGrab(NeedsToStopGrabOnHiding);
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black);
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;
        }

        #endregion ITabManager implementation

        #region RelayCommands

        private AutoRelayCommand _centerHorizontalLine;

        public AutoRelayCommand CenterHorizontalLine =>
            _centerHorizontalLine ?? (_centerHorizontalLine = new AutoRelayCommand(
                () =>
                {
                    HorizontalLine1Position = ImageHeight / 2;
                },
                () => { return HorizontalLine1Position != ImageHeight / 2; }
            ));

        private AutoRelayCommand _centerVerticalLine;

        public AutoRelayCommand CenterVerticalLine =>
            _centerVerticalLine ?? (_centerVerticalLine = new AutoRelayCommand(
                () =>
                {
                    VerticalLine1Position = ImageWidth / 2;
                },
                () => { return VerticalLine1Position != ImageWidth / 2; }
            ));

        #endregion RelayCommands
    }
}
