using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Modules.Settings.View.Designer;
using UnitySC.PM.DMT.Shared.UI.Extensions;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.DMT.Shared.UI.ViewModel;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public class HighAngleDarkFieldMaskVM : SettingWithVideoStreamVM, ITabManager
    {
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly IDialogOwnerService _dialogService;

        public HighAngleDarkFieldMaskVM(Side waferSide, ExposureSettingsWithAutoVM exposureSettings, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor,
            CalibrationSupervisor calibrationSupervisor, IDialogOwnerService dialogService, ILogger logger, IMessenger messenger)
            : base(waferSide, exposureSettings, cameraSupervisor, screenSupervisor, messenger, dialogService, logger)
        {
            _calibrationSupervisor = calibrationSupervisor;
            _dialogService = dialogService;

            Header = "High Angle Dark-field Mask";
        }

        private ScreenInfo _screenInfo;

        private ObservableCollection<DrawingItem> _drawingIntems = new ObservableCollection<DrawingItem>();

        public ObservableCollection<DrawingItem> DrawingItems
        {
            get => _drawingIntems; set { if (_drawingIntems != value) { _drawingIntems = value; OnPropertyChanged(); } }
        }

        public int ScreenWidth => _screenInfo.Width;

        public int ScreenHeight => _screenInfo.Height;

        private AutoRelayCommand _designerDragCompleted;

        public AutoRelayCommand DesignerDragCompleted
        {
            get
            {
                return _designerDragCompleted ?? (_designerDragCompleted = new AutoRelayCommand(
                    () =>
                    {
                        DisplayMask();
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _applyMaskImage;

        public AutoRelayCommand ApplyMaskImage
        {
            get
            {
                return _applyMaskImage ?? (_applyMaskImage = new AutoRelayCommand(
                    () =>
                    {
                        var maskBitmap = CreateMaskImage();
                        ServiceImage image = new ServiceImage();
                        image.CreateFromBitmap(maskBitmap);

                        if (_calibrationSupervisor.SetHighAngleDarkFieldMaskForSide(WaferSide, image))
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => { _dialogService.ShowMessageBox("The screen image for high angle dark-field measure has successfully been set", "High angle dark-field mask", MessageBoxButton.OK, MessageBoxImage.Information); }));
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => { _dialogService.ShowMessageBox("Unable to set the screen image for high angle dark-field measure", "High angle dark-field mask", MessageBoxButton.OK, MessageBoxImage.Error); }));
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _exportMaskImageSettings;

        public AutoRelayCommand ExportMaskImageSettings
        {
            get
            {
                return _exportMaskImageSettings ?? (_exportMaskImageSettings = new AutoRelayCommand(
                    () =>
                    {
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string filePath = saveFileDialog.FileName;
                            ExportData(filePath, DrawingItems);
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _importMaskImageSettings;

        public AutoRelayCommand ImportMaskImageSettings
        {
            get
            {
                return _importMaskImageSettings ?? (_importMaskImageSettings = new AutoRelayCommand(
                    () =>
                    {
                        var openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                        if (openFileDialog.ShowDialog() == true)
                        {
                            string filePath = openFileDialog.FileName;
                            DrawingItems = ImportDataFromXml(filePath);
                            DisplayMask();
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _testMask;

        public AutoRelayCommand TestMask
        {
            get
            {
                return _testMask ?? (_testMask = new AutoRelayCommand(
                    () =>
                    {
                        DisplayMask();
                    },
                    () => { return true; }
                ));
            }
        }

        private void DisplayMask()
        {
            // Code to execute
            var maskBitmap = CreateMaskImage();

            ScreenSupervisor.DisplayImage(WaferSide, maskBitmap);
        }

        private BitmapSource CreateMaskImage()
        {
            int imageWidth = _screenInfo.Width;
            int imageHeight = _screenInfo.Height;

            // Create the Rectangle
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();

            // Draw the background
            context.DrawRectangle(Brushes.White, null, new Rect(0, 0, imageWidth, imageHeight));

            foreach (var drawItem in DrawingItems)
            {
                if (!drawItem.IsVisible)
                    continue;

                if (drawItem is EllipseDrawingItem)
                {
                    var ellipseDrawItem = drawItem as EllipseDrawingItem;

                    context.DrawEllipse(new SolidColorBrush(Colors.Black), null, new Point(ellipseDrawItem.X + ellipseDrawItem.Width / 2, ellipseDrawItem.Y + ellipseDrawItem.Height / 2), ellipseDrawItem.Width / 2, ellipseDrawItem.Height / 2);
                }

                if (drawItem is PolygonDrawingItem)
                {
                    var pathDrawingItem = drawItem as PolygonDrawingItem;
                    DrawPolygonOrPolyline(context, new SolidColorBrush(Colors.Black), null, pathDrawingItem.Points.ToArray(), FillRule.EvenOdd, true);
                }
            }

            //context.DrawRectangle(Brushes.Red, null, new Rect(200, 20, 200, 150));
            context.Close();

            // Create the Bitmap and render the rectangle onto it.
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);

            var bitmap24 = bmp.ConvertToRGB24();

#if DEBUG
            string outputFile = "C:/temp/Test.bmp";
            // Save the image to a location on the disk.
            var encoder = new BmpBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (var fileStream = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
#endif

            return bitmap24;
        }

        // Draw a polygon or polyline.
        private void DrawPolygonOrPolyline(
            DrawingContext drawingContext,
            Brush brush, Pen pen, Point[] points, FillRule fill_rule,
            bool isClosed)
        {
            // Make a StreamGeometry to hold the drawing objects.
            StreamGeometry geo = new StreamGeometry();
            geo.FillRule = fill_rule;

            // Open the context to use for drawing.
            using (StreamGeometryContext context = geo.Open())
            {
                // Start at the first point.
                context.BeginFigure(points[0], true, isClosed);

                // Add the points after the first one.
                context.PolyLineTo(points.Skip(1).ToArray(), true, false);
            }

            // Draw.
            drawingContext.DrawGeometry(brush, pen, geo);
        }

        #region ITabManager implementation

        public void Display()
        {
            IsActive = true;
            StartGrab(NeedsToStartGrabOnDisplay);

            if (_screenInfo == null)
            {
                _screenInfo = ScreenSupervisor.GetScreenInfo(WaferSide);
                OnPropertyChanged(nameof(ScreenWidth));
                OnPropertyChanged(nameof(ScreenHeight));
                if (_screenInfo == null)
                    Logger.Error("Get screen infos failed");

                // We create the drawingItems

                var ellipse = new EllipseDrawingItem();
                ellipse.X = _screenInfo.Width / 3 - 100;
                ellipse.Y = _screenInfo.Height / 3;
                ellipse.Width = _screenInfo.Width / 3 + 200;
                ellipse.Height = 2 * _screenInfo.Height / 3;
                ellipse.PropertyChanged += DrawingItem_PropertyChanged;
                DrawingItems.Add(ellipse);

                var polygon = new PolygonDrawingItem();
                polygon.Points = new List<Point>() { new Point(_screenInfo.Width / 3, 0), new Point(2 * _screenInfo.Width / 3, 0), new Point(2 * _screenInfo.Width / 3 - 100, _screenInfo.Height / 3), new Point(_screenInfo.Width / 3 + 100, _screenInfo.Height / 3) };
                polygon.PropertyChanged += DrawingItem_PropertyChanged;
                DrawingItems.Add(polygon);

                var polygon2 = new PolygonDrawingItem();
                polygon2.PropertyChanged += DrawingItem_PropertyChanged;
                polygon2.Points = new List<Point>() { new Point(10, 0), new Point(50, 0), new Point(50, 50), new Point(10, 50) };
                polygon2.IsVisible = false;
                DrawingItems.Add(polygon2);
            }

            DisplayMask();
            ExposureSettings.AutoExposureStarted += ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated += ExposureSettings_AutoExposureTerminated;
        }

        private void DrawingItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
                DisplayMask();
        }

        public bool CanHide() => true;

        public void Hide()
        {
            IsActive = false;
            StopGrab(NeedsToStopGrabOnHiding);
            ScreenSupervisor.SetScreenColor(WaferSide, Colors.Black, false);
            ExposureSettings.AutoExposureStarted -= ExposureSettings_AutoExposureStarted;
            ExposureSettings.AutoExposureTerminated -= ExposureSettings_AutoExposureTerminated;
        }

        #endregion ITabManager implementation

        private void ExposureSettings_AutoExposureTerminated(object sender, EventArgs e)
        {
            IsBusy = false;
            StartGrab();
        }

        private void ExposureSettings_AutoExposureStarted(object sender, EventArgs e)
        {
            BusyMessage = "Computing Exposure";
            IsBusy = true;
            StopGrab();
        }

        public void ExportData(string filePath, ObservableCollection<DrawingItem> drawingItems)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HighAngleDarkFieldImageSettingData));
                serializer.Serialize(writer, new HighAngleDarkFieldImageSettingData { DrawingImageItems = drawingItems });
            }
        }

        public ObservableCollection<DrawingItem> ImportDataFromXml(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HighAngleDarkFieldImageSettingData));
                HighAngleDarkFieldImageSettingData data = (HighAngleDarkFieldImageSettingData)serializer.Deserialize(reader);
                return (data.DrawingImageItems);
            }
        }
    }
}
