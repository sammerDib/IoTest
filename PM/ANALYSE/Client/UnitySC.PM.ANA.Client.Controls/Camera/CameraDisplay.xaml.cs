using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Exceptions;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for CameraDisplay.xaml
    /// </summary>
    public partial class CameraDisplay : UserControl
    {
        private int _zoomFactor = 5;

        // Minimal distance (square) to detect a measure instead of a move
        private const int MinMoveDistanceMeasure2 = 500;

        private bool _measureInProgress = false;
        private ILogger _logger;

        private Point? _currentMousePosition = null;

        public bool IsVideoDisplayed
        {
            get { return (bool)GetValue(IsVideoDisplayedProperty); }
            set { SetValue(IsVideoDisplayedProperty, value); }
        }

        public static readonly DependencyProperty IsVideoDisplayedProperty =
            DependencyProperty.Register(nameof(IsVideoDisplayed), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));


        public PixelInformation CurrentPixelInformation
        {
            get { return (PixelInformation)GetValue(CurrentPixelInformationProperty); }
            set { SetValue(CurrentPixelInformationProperty, value); }
        }

        public static readonly DependencyProperty CurrentPixelInformationProperty =
            DependencyProperty.Register(nameof(CurrentPixelInformation), typeof(PixelInformation), typeof(CameraDisplay), new PropertyMetadata(null));

        public PixelInformation CenterPixelInformation
        {
            get { return (PixelInformation)GetValue(CenterPixelInformationProperty); }
            set { SetValue(CenterPixelInformationProperty, value); }
        }

        public static readonly DependencyProperty CenterPixelInformationProperty =
            DependencyProperty.Register(nameof(CenterPixelInformation), typeof(PixelInformation), typeof(CameraDisplay), new PropertyMetadata(null));

        public double MeanPixelValue
        {
            get { return (double)GetValue(MeanPixelValueProperty); }
            set { SetValue(MeanPixelValueProperty, value); }
        }

        public static readonly DependencyProperty MeanPixelValueProperty =
            DependencyProperty.Register(nameof(MeanPixelValue), typeof(double), typeof(CameraDisplay), new PropertyMetadata(double.NaN));

        public double CameraPositionX
        {
            get { return (double)GetValue(CameraPositionXProperty); }
            set { SetValue(CameraPositionXProperty, value); }
        }

        public static readonly DependencyProperty CameraPositionXProperty =
            DependencyProperty.Register(nameof(CameraPositionX), typeof(double), typeof(CameraDisplay), new PropertyMetadata(double.NaN));

        public double CameraPositionY
        {
            get { return (double)GetValue(CameraPositionYProperty); }
            set { SetValue(CameraPositionYProperty, value); }
        }

        public static readonly DependencyProperty CameraPositionYProperty =
            DependencyProperty.Register(nameof(CameraPositionY), typeof(double), typeof(CameraDisplay), new PropertyMetadata(double.NaN));

        public bool UsePixelUnit
        {
            get { return (bool)GetValue(UsePixelUnitProperty); }
            set { SetValue(UsePixelUnitProperty, value); }
        }

        public static readonly DependencyProperty UsePixelUnitProperty =
            DependencyProperty.Register("UsePixelUnit", typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false));

        public bool MoveIsEnabled
        {
            get { return (bool)GetValue(MoveIsEnabledProperty); }
            set { SetValue(MoveIsEnabledProperty, value); }
        }

        public static readonly DependencyProperty MoveIsEnabledProperty =
            DependencyProperty.Register("MoveIsEnabled", typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));

        #region ROI

        // Roi size in pixels on the full image
        public Size RoiSize
        {
            get { return (Size)GetValue(RoiSizeProperty); }
            set { SetValue(RoiSizeProperty, value); }
        }

        public static readonly DependencyProperty RoiSizeProperty =
            DependencyProperty.Register(nameof(RoiSize), typeof(Size), typeof(CameraDisplay), new PropertyMetadata(new Size(0, 0)));

        public bool IsRoiSelectorVisible
        {
            get { return (bool)GetValue(IsRoiSelectorVisibleProperty); }
            set { SetValue(IsRoiSelectorVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsRoiSelectorVisibleProperty =
            DependencyProperty.Register(nameof(IsRoiSelectorVisible), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false));

        // Roi Rect on the full image
        public Rect RoiRect
        {
            get { return (Rect)GetValue(RoiRectProperty); }
            set { SetValue(RoiRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoiRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoiRectProperty =
            DependencyProperty.Register(nameof(RoiRect), typeof(Rect), typeof(CameraDisplay), new PropertyMetadata(new Rect(0, 0, 0, 0)));

        public bool IsCenteredROI
        {
            get { return (bool)GetValue(IsCenteredROIProperty); }
            set { SetValue(IsCenteredROIProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCenteredROIProperty.  This enables centred roi...
        public static readonly DependencyProperty IsCenteredROIProperty =
            DependencyProperty.Register("IsCenteredROI", typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));

        #endregion ROI

        #region Horizontal Line Selector

        public bool IsHorizontalLineSelectorVisible
        {
            get { return (bool)GetValue(IsHorizontalLineSelectorVisibleProperty); }
            set { SetValue(IsHorizontalLineSelectorVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsHorizontalLineSelectorVisibleProperty =
            DependencyProperty.Register(nameof(IsHorizontalLineSelectorVisible), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false));

        public double HorizontalLinePosition
        {
            get { return (double)GetValue(HorizontalLinePositionProperty); }
            set { SetValue(HorizontalLinePositionProperty, value); }
        }

        public static readonly DependencyProperty HorizontalLinePositionProperty =
            DependencyProperty.Register(nameof(HorizontalLinePosition), typeof(double), typeof(CameraDisplay), new PropertyMetadata(0d, OnHorizontalLinePositionChanged, CoerceHorizontalLinePosition));

        private static object CoerceHorizontalLinePosition(DependencyObject d, object baseValue)
        {
            return (d as CameraDisplay)?.GetValidHorizontalLinePosition((double)baseValue);
        }

        private double GetValidHorizontalLinePosition(double horizontalLinePos)
        {
            if ((CameraImage.Source is null) || (CameraImage.Source.Height == 0))
                return horizontalLinePos;

            if (horizontalLinePos < 0)
                horizontalLinePos = 0;
            if (horizontalLinePos > CameraImage.Source.Height)
                horizontalLinePos = CameraImage.Source.Height;
            return horizontalLinePos;
        }

        private static void OnHorizontalLinePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Horizontal Line Selector

        #region Measure Display

        private Point? _newMeasurePointFrom;

        [Browsable(false)]
        public Point MeasurePointFrom
        {
            get { return (Point)GetValue(MeasurePointFromProperty); }
            set { SetValue(MeasurePointFromProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointFromProperty =
            DependencyProperty.Register(nameof(MeasurePointFrom), typeof(Point), typeof(CameraDisplay), new PropertyMetadata(new Point()));

        [Browsable(false)]
        public Point MeasurePointTo
        {
            get { return (Point)GetValue(MeasurePointToProperty); }
            set { SetValue(MeasurePointToProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointToProperty =
            DependencyProperty.Register(nameof(MeasurePointTo), typeof(Point), typeof(CameraDisplay), new PropertyMetadata(new Point()));

        [Browsable(false)]
        public string MeasureValueDisplay
        {
            get { return (string)GetValue(MeasureValueDisplayProperty); }
            set { SetValue(MeasureValueDisplayProperty, value); }
        }

        public static readonly DependencyProperty MeasureValueDisplayProperty =
            DependencyProperty.Register(nameof(MeasureValueDisplay), typeof(string), typeof(CameraDisplay), new PropertyMetadata(string.Empty));

        public bool MeasureDisplayVisible
        {
            get { return (bool)GetValue(MeasureDisplayVisibleProperty); }
            set { SetValue(MeasureDisplayVisibleProperty, value); }
        }

        public static readonly DependencyProperty MeasureDisplayVisibleProperty =
            DependencyProperty.Register(nameof(MeasureDisplayVisible), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false, OnMeasureDisplayVisibleChanged));

        private static void OnMeasureDisplayVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CameraDisplay).UpdateMeasureValueDisplay();
        }

        public bool IsMeasureToolAvailable
        {
            get { return (bool)GetValue(IsMeasureToolAvailableProperty); }
            set { SetValue(IsMeasureToolAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsMeasureToolAvailableProperty =
            DependencyProperty.Register(nameof(IsMeasureToolAvailable), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));


        public bool IsZoomOnCenterAvailable
        {
            get { return (bool)GetValue(IsZoomOnCenterAvailableProperty); }
            set { SetValue(IsZoomOnCenterAvailableProperty, value); }
        }

        public static readonly DependencyProperty IsZoomOnCenterAvailableProperty =
            DependencyProperty.Register(nameof(IsZoomOnCenterAvailable), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));


        public bool IsZoomed
        {
            get { return (bool)GetValue(IsZoomedProperty); }
            set { SetValue(IsZoomedProperty, value); }
        }

        public static readonly DependencyProperty IsZoomedProperty =
            DependencyProperty.Register(nameof(IsZoomed), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false, UpdateZoomStatus));


        public bool IsZoomedOnCenter
        {
            get { return (bool)GetValue(IsZoomedOnCenterProperty); }
            set { SetValue(IsZoomedOnCenterProperty, value); }
        }

        public static readonly DependencyProperty IsZoomedOnCenterProperty =
            DependencyProperty.Register(nameof(IsZoomedOnCenter), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(false, UpdateZoomStatus));


        public Point ZoomPosition
        {
            get { return (Point)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register(nameof(ZoomPosition), typeof(Point), typeof(CameraDisplay), new PropertyMetadata(new Point(), UpdateZoomStatus));

        private static void UpdateZoomStatus(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CameraDisplay).UpdateZoomStatus();
        }

        private void UpdateZoomStatus()
        {
            if ((!IsZoomed) && (!IsZoomedOnCenter))
            {
                DisplayGrid.RenderTransform = null;
            }
            else
            {
                if (IsZoomedOnCenter)
                {
                    ZoomOnPosition(new Point(CameraImage.ActualWidth/2, CameraImage.ActualHeight/2));
                }
                else
                {
                    ZoomOnPosition(ZoomPosition);
                }
            }

            UpdateCursor();
        }

        private void ZoomOnPosition(Point zoomPosition)
        {
            TranslateTransform myTranslate = new TranslateTransform();
            myTranslate.X = -(zoomPosition.X + (ActualWidth - CameraImage.ActualWidth) / 2) * _zoomFactor + ActualWidth / 2;
            myTranslate.Y = -(zoomPosition.Y + (ActualHeight - CameraImage.ActualHeight) / 2) * _zoomFactor + ActualHeight / 2;
            ScaleTransform myScaleTransform = new ScaleTransform();
            myScaleTransform.ScaleY = _zoomFactor;
            myScaleTransform.ScaleX = _zoomFactor;

            TransformGroup myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(myScaleTransform);
            myTransformGroup.Children.Add(myTranslate);
            DisplayGrid.RenderTransform = myTransformGroup;
        }

        #endregion Measure Display

        #region Scale Display

        public bool ScaleVisible
        {
            get { return (bool)GetValue(ScaleVisibleProperty); }
            set { SetValue(ScaleVisibleProperty, value); }
        }

        public static readonly DependencyProperty ScaleVisibleProperty =
            DependencyProperty.Register(nameof(ScaleVisible), typeof(bool), typeof(CameraDisplay), new PropertyMetadata(true));

        [Browsable(false)]
        public double ScaleLength { get; set; }

        [Browsable(false)]
        public string ScaleValue { get; set; }

        #endregion Scale Display

        public CameraDisplay()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            _logger = ClassLocator.Default.GetInstance<ILogger>();

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Points = new ObservableCollection<ICameraDisplayPoint>();
            }
            CurrentPixelInformation = new PixelInformation();
            CenterPixelInformation = new PixelInformation();
            MeanPixelValue = double.NaN;

            MeasurePointFrom = new Point(30, 30);
            MeasurePointTo = new Point(100, 100);
            MeasureValueDisplay = string.Empty;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            // We create the binding to the Axes supervisor for the current position
            Binding bindingX = new Binding("X");
            bindingX.Source = ServiceLocator.AxesSupervisor.AxesVM.Position;
            bindingX.Mode = BindingMode.OneWay;
            SetBinding(CameraPositionXProperty, bindingX);

            Binding bindingY = new Binding("Y");
            bindingY.Source = ServiceLocator.AxesSupervisor.AxesVM.Position;
            bindingY.Mode = BindingMode.OneWay;
            SetBinding(CameraPositionYProperty, bindingY);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            UpdateScale();
            if (!(Camera is null))
                Camera.PropertyChanged += Camera_PropertyChanged;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged += Axes_PropertyChanged;
            UpdateCursor();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Camera.PropertyChanged -= Camera_PropertyChanged;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= Axes_PropertyChanged;
        }

        private void Camera_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Camera.CameraBitmapSource))
            {
                UpdateScale();
                UpdateCenterPixelInformation();
                UpdateMeanPixelInformation();

                if (!(_currentMousePosition is null))
                    UpdatePixelInformation((Point)_currentMousePosition);
            }
        }

        private void Axes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxesVM.IsLocked) || e.PropertyName == nameof(AxesVM.IsReadyToStartMove))
            {
                UpdateCursor();
            }
        }

        private void UpdateCursor()
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                Cursor newCursor = null;
                if (ServiceLocator.AxesSupervisor.AxesVM.IsLocked)
                    newCursor = (Cursor)FindResource("NoCursor");
                else
                {
                    if ((!MoveIsEnabled) || IsZoomed)
                        newCursor = Cursors.Arrow;
                    else
                    {
                        if (ServiceLocator.AxesSupervisor.AxesVM.IsReadyToStartMove)
                            newCursor = (Cursor)FindResource("CrossCursor");
                        else
                            newCursor = Cursors.Wait;
                    }
                }

                CameraImage.Cursor = newCursor;
            }));
        }

        public CameraVM Camera
        {
            get { return (CameraVM)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register(nameof(Camera), typeof(CameraVM), typeof(CameraDisplay), new PropertyMetadata(null));

        // Points are used only in design mode
        public ObservableCollection<ICameraDisplayPoint> Points
        {
            get { return (ObservableCollection<ICameraDisplayPoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(nameof(Points), typeof(ObservableCollection<ICameraDisplayPoint>), typeof(CameraDisplay), new PropertyMetadata(null));

        public DataTemplate PointTemplate
        {
            get { return (DataTemplate)GetValue(PointTemplateProperty); }
            set { SetValue(PointTemplateProperty, value); }
        }

        public static readonly DependencyProperty PointTemplateProperty =
            DependencyProperty.Register(nameof(PointTemplate), typeof(DataTemplate), typeof(CameraDisplay), new PropertyMetadata(null));

        private double GetMeasuremm(Point measurePointFrom, Point measurePointTo)
        {
            return GetMeasurePixel(measurePointFrom, measurePointTo) * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
        }

        private double GetMeasurePixel(Point measurePointFrom, Point measurePointTo)
        {
            var measurePointFromAxes = ConvertActualPixelCoordToCameraPixel(measurePointFrom);

            var measurePointToAxes = ConvertActualPixelCoordToCameraPixel(measurePointTo);
            if ((measurePointFromAxes == null) || (measurePointTo == null))
                return double.NaN;

            var measure = Math.Sqrt(Math.Pow((double)measurePointToAxes?.X - (double)measurePointFromAxes?.X, 2) + Math.Pow((double)measurePointToAxes?.Y - (double)measurePointFromAxes?.Y, 2));
            return measure;
        }

        public static Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            Color color;
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel];
            var rect = new Int32Rect(x, y, 1, 1);

            bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

            if (bitmap.Format == PixelFormats.Bgra32)
            {
                color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else if (bitmap.Format == PixelFormats.Bgr32)
            {
                color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
            }
            else if (bitmap.Format == PixelFormats.Gray8)
            {
                color = Color.FromRgb(bytes[0], bytes[0], bytes[0]);
            }
            // handle other required formats
            else
            {
                color = Colors.Black;
            }

            return color;
        }

        private void CameraImage_MouseLeave(object sender, MouseEventArgs e)
        {
            CurrentPixelInformation.Value = double.NaN;
            _currentMousePosition = null;
        }

        private Size GetImageSize()
        {
            ImageSource imageSource = CameraImage.Source;
            if (imageSource == null)
                return Size.Empty;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            return new Size(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
        }

        private void CameraImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _newMeasurePointFrom = e.GetPosition(CameraImage);
        }

        private void CameraImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_measureInProgress)
            {
                if (MoveIsEnabled && !IsZoomed)
                {
                    // We try to move
                    var targetPositionPixels = e.GetPosition(CameraImage);

                    if (!(ServiceLocator.AxesSupervisor.AxesVM.IsLocked) && (ServiceLocator.AxesSupervisor.AxesVM.IsReadyToStartMove))
                    {
                        var moveOfset = GetOfsetMmFromPositionPixels(targetPositionPixels);

                        ServiceLocator.AxesSupervisor.MoveIncremental(new XYZTopZBottomMove(moveOfset.X, moveOfset.Y, 0, 0), AxisSpeed.Fast);
                    }
                }
            }
            else
            {
                CameraImage.ReleaseMouseCapture();
                _measureInProgress = false;
                UpdateCursor();
            }
            _newMeasurePointFrom = null;
        }

        private Point GetOfsetMmFromPositionPixels(Point pixelCoord)
        {
            Point newOfset = new Point(0, 0);
            var imageSize = GetImageSize();

            if (imageSize == Size.Empty)
                return newOfset;

            var realTargetPositionPixels = new Point(pixelCoord.X * imageSize.Width / CameraImage.ActualWidth, pixelCoord.Y * imageSize.Height / CameraImage.ActualHeight);

            // We calculate the offset to the center of the image
            var targetOffsetPixels = realTargetPositionPixels - new Point(imageSize.Width / 2, imageSize.Height / 2);

            newOfset.X = targetOffsetPixels.X * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;

            newOfset.Y = -targetOffsetPixels.Y * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters;
            return newOfset;
        }

        // Converts coordinates in pixels from the display to pixel camera
        private Point? ConvertActualPixelCoordToCameraPixel(Point pixelCoord)
        {
            var imageSize = GetImageSize();

            if (imageSize == Size.Empty)
                return null;

            return new Point(pixelCoord.X * imageSize.Width / CameraImage.ActualWidth, pixelCoord.Y * imageSize.Height / CameraImage.ActualHeight);
        }

        private void CameraImage_MouseMove(object sender, MouseEventArgs e)
        {
            var positionOverImage = e.GetPosition(CameraImage);
            // if we are outside of the cameraimage
            if ((positionOverImage.X > CameraImage.ActualWidth) || (positionOverImage.Y > CameraImage.ActualHeight) || (positionOverImage.X < 0) || (positionOverImage.Y < 0))
            {
                _currentMousePosition = null;
                return;
            }
            else
            {
                _currentMousePosition = new Point(positionOverImage.X, positionOverImage.Y);
            }

            if (!IsMeasureToolAvailable)
                return;

            if (_newMeasurePointFrom is null)
                return;

            // Square of the distance between MeasurePointFrom and the current mouse position
            var moveDistance2 = Math.Pow(positionOverImage.X - ((Point)_newMeasurePointFrom).X, 2) + Math.Pow(positionOverImage.Y - ((Point)_newMeasurePointFrom).Y, 2);

            if (((e.LeftButton == MouseButtonState.Pressed) &&
                 (moveDistance2 > MinMoveDistanceMeasure2)) || _measureInProgress)

            {
                if (!_measureInProgress)
                {
                    CameraImage.Cursor = (Cursor)FindResource("MeasureCursor");
                    MeasureDisplayVisible = true;
                    _measureInProgress = true;
                    CameraImage.CaptureMouse();
                }
                MeasurePointFrom = ((Point)_newMeasurePointFrom);

                MeasurePointTo = positionOverImage;

                UpdateMeasureValueDisplay();
            }
        }

        private void UpdateMeasureValueDisplay()
        {
            if (UsePixelUnit)
                MeasureValueDisplay = GetMeasurePixel(MeasurePointFrom, MeasurePointTo).ToString("0 pixels");
            else
                MeasureValueDisplay = GetMeasuremm(MeasurePointFrom, MeasurePointTo).ToString("0.000 mm");
        }

        private void UpdatePixelInformation(Point mousePosition)
        {
            if ((CameraImage == null) || (CameraImage.ActualWidth == 0) || (CameraImage.ActualHeight == 0))
                return;

            // We must retrieve the value of the pixel under the mouse.
            ImageSource imageSource = CameraImage.Source;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            if (bitmapImage == null)
                return;
            var imagePosX = mousePosition.X * bitmapImage.PixelWidth / CameraImage.ActualWidth;
            var imagePosY = mousePosition.Y * bitmapImage.PixelHeight / CameraImage.ActualHeight;
            CurrentPixelInformation.Position = new Point(imagePosX, imagePosY);

            try
            {
                // if we are outside the image
                if ((imagePosX + 1 > bitmapImage.Width) || (imagePosY + 1 > bitmapImage.Height) || imagePosX < 0 || imagePosY < 0)
                    CurrentPixelInformation.Value = double.NaN;
                else
                {
                    CurrentPixelInformation.Value = GetPixelColor(bitmapImage, (int)imagePosX, (int)imagePosY).R;
                }
            }
            catch (Exception)
            {
                CurrentPixelInformation.Value = double.NaN;
            }
        }

        private void UpdateCenterPixelInformation()
        {
            // We must retrieve the value of the pixel under the center target.
            ImageSource imageSource = CameraImage.Source;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            if (bitmapImage == null)
            {
                CenterPixelInformation.Value = double.NaN;
                return;
            }
            CenterPixelInformation.Position = new Point(bitmapImage.PixelWidth / 2, bitmapImage.PixelHeight / 2);
            try
            {
                CenterPixelInformation.Value = GetPixelColor(bitmapImage, (int)CenterPixelInformation.Position.X, (int)CenterPixelInformation.Position.Y).R;
            }
            catch (Exception)
            {
                CenterPixelInformation.Value = double.NaN;
            }
        }

        private void UpdateMeanPixelInformation()
        {
            // We must get the average pixel value of teh whole image displayed
            ImageSource imageSource = CameraImage.Source;
            BitmapSource bitmapImage = (BitmapSource)imageSource;
            if (bitmapImage == null)
            {
                MeanPixelValue = double.NaN;
                return;
            }

            var width = bitmapImage.PixelWidth;
            var height = bitmapImage.PixelHeight;
            var bytesPerPixel = (bitmapImage.Format.BitsPerPixel + 7) / 8;
            var bufstride = width * bytesPerPixel;
            var bytes = new byte[height * bufstride];
            bitmapImage.CopyPixels(bytes, bufstride, 0);

            long sumtotal = 0;
            unsafe
            {
                var totalPixels = width * height;
                fixed (byte* pixels = bytes)
                {
                    byte* p = pixels;
                    if (bytesPerPixel == 1)
                    {
                        for (int k = 0; k < totalPixels; k++)
                        {
                            sumtotal += *p;
                            ++p;
                        }
                    }
                    else
                    {
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                sumtotal += p[(y * bufstride) + x * bytesPerPixel];
                            }
                        }
                    }
                }
                MeanPixelValue = (double)sumtotal / (double)totalPixels;
            }
        }

        private void UpdateScale()
        {
            try
            {
                ImageSource imageSource = CameraImage.Source;

                if (imageSource == null)
                    return;

                BitmapSource bitmapImage = (BitmapSource)imageSource;

                double scaleLength;
                string scaleValueString;

                if (UsePixelUnit)
                    GetScaleValuePixel(out scaleLength, out scaleValueString);
                else
                {
                    if (ServiceLocator.CamerasSupervisor.Objective is null)
                        return;
                    GetScaleValuemm(bitmapImage.PixelWidth * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters, out scaleLength, out scaleValueString);
                }

                if (ScaleValue != scaleValueString)
                {
                    ScaleValue = scaleValueString;
                    (CameraScale).GetBindingExpression(CameraScaleDisplay.FullScaleValueProperty).UpdateTarget();
                }

                double newScaleLength;
                if (UsePixelUnit)
                    newScaleLength = (CameraImage.ActualWidth / bitmapImage.PixelWidth) * scaleLength;
                else
                    newScaleLength = scaleLength / ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters * CameraImage.ActualWidth / bitmapImage.PixelWidth;

                if (!ScaleLength.Near(newScaleLength, 0.01))
                {
                    ScaleLength = newScaleLength;
                    (CameraScale).GetBindingExpression(CameraScaleDisplay.ScaleLengthProperty).UpdateTarget();
                }
            }
            catch (NullObjectiveCalibrationException ex)
            {
                _logger.Warning(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex.Message);
            }
        }


        private void UpdateZoomPosition()
        {
            if (IsZoomedOnCenter)
            {
                ZoomOnPosition(new Point(CameraImage.ActualWidth / 2, CameraImage.ActualHeight / 2));
            }
        }

        // Gets the scale information to display from the width of the image in mm
        private void GetScaleValuemm(double imageWidth, out double scaleValue, out string scaleValueString)
        {
            if ((imageWidth > 10) && (imageWidth <= 50))
            {
                scaleValue = 10;
                scaleValueString = "10 mm";
                return;
            }

            if ((imageWidth > 5) && (imageWidth <= 10))
            {
                scaleValue = 5;
                scaleValueString = "5 mm";
                return;
            }

            if ((imageWidth > 1) && (imageWidth <= 5))
            {
                scaleValue = 1;
                scaleValueString = "1 mm";
                return;
            }
            if ((imageWidth > 0.5) && (imageWidth <= 1))
            {
                scaleValue = 0.500;
                scaleValueString = "500 µm";
                return;
            }

            if ((imageWidth > 0.1) && (imageWidth <= 0.5))
            {
                scaleValue = 0.1;
                scaleValueString = "100 µm";
                return;
            }

            if ((imageWidth > 0.05) && (imageWidth <= 0.1))
            {
                scaleValue = 0.05;
                scaleValueString = "50 µm";
                return;
            }

            if ((imageWidth > 0.01) && (imageWidth <= 0.05))
            {
                scaleValue = 0.01;
                scaleValueString = "10 µm";
                return;
            }

            if ((imageWidth > 0.005) && (imageWidth <= 0.01))
            {
                scaleValue = 0.005;
                scaleValueString = "5 µm";
                return;
            }

            if ((imageWidth > 0.001) && (imageWidth <= 0.005))
            {
                scaleValue = 0.001;
                scaleValueString = "1 µm";
                return;
            }

            if ((imageWidth > 0.0005) && (imageWidth <= 0.001))
            {
                scaleValue = 0.0005;
                scaleValueString = "500 nm";
                return;
            }

            if ((imageWidth > 0.0001) && (imageWidth <= 0.0005))
            {
                scaleValue = 0.0001;
                scaleValueString = "100 nm";
                return;
            }

            scaleValue = 0;
            scaleValueString = null;

            new ArgumentOutOfRangeException("Image size is out of range");
        }

        private void GetScaleValuePixel(out double scaleValue, out string scaleValueString)
        {
            scaleValue = 200;
            scaleValueString = "200 pixels";
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScale();
            UpdateZoomPosition();

        }

      

    }
}
