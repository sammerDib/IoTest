using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using UnitySC.PM.ANA.Client.Proxy;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    /// <summary>
    /// Interaction logic for MeasureTool.xaml
    /// </summary>
    public partial class MeasureTool : UserControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                    Canvas.SetZIndex(this, 100);
                else
                    Canvas.SetZIndex(this, 0);
            }
        }

        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MeasureTool), new FrameworkPropertyMetadata(false));

        private Point? _fromPointInCamera;
        private Point? _toPointInCamera;

        public Point FromPoint
        {
            get { return (Point)GetValue(FromPointProperty); }
            set { SetValue(FromPointProperty, value); }
        }

        public static readonly DependencyProperty FromPointProperty =
            DependencyProperty.Register(nameof(FromPoint), typeof(Point), typeof(MeasureTool), new PropertyMetadata(new Point(0, 0), OnFromPointChanged));

        private static void OnFromPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MeasureTool).UpdateFromPointInCamera();
            (d as MeasureTool).UpdateAllDisplay();
        }

        private void UpdateFromPointInCamera()
        {
            _fromPointInCamera = ConvertActualPixelCoordToCameraPixel(FromPoint);
        }

        private void UpdateActualPoints()
        {
            if (!(_fromPointInCamera is null))
                FromPoint = (Point)(ConvertCameraPixelCoordToActualPixel((Point)_fromPointInCamera)?? new Point(10,10));

            if (!(_toPointInCamera is null))
                ToPoint = (Point)(ConvertCameraPixelCoordToActualPixel((Point)_toPointInCamera)??new Point(100,100));
        }

        public Point ToPoint
        {
            get { return (Point)GetValue(ToPointProperty); }
            set { SetValue(ToPointProperty, value); }
        }

        public static readonly DependencyProperty ToPointProperty =
            DependencyProperty.Register(nameof(ToPoint), typeof(Point), typeof(MeasureTool), new PropertyMetadata(new Point(0, 0), OnToPointChanged));

        private static void OnToPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MeasureTool).UpdateToPointInCamera();
            (d as MeasureTool).UpdateAllDisplay();
        }

        private void UpdateToPointInCamera()
        {
            _toPointInCamera = ConvertActualPixelCoordToCameraPixel(ToPoint);
        }

        public double ContainerWidth
        {
            get { return (double)GetValue(MaxSelectorWidthProperty); }
            set { SetValue(MaxSelectorWidthProperty, value); }
        }

        public static readonly DependencyProperty MaxSelectorWidthProperty =
            DependencyProperty.Register(nameof(ContainerWidth), typeof(double), typeof(MeasureTool), new PropertyMetadata(0d, OnContainerWidthChanged));

        private static void OnContainerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MeasureTool).UpdateActualPoints();
        }

        public double ContainerHeight
        {
            get { return (double)GetValue(MaxSelectorHeightProperty); }
            set { SetValue(MaxSelectorHeightProperty, value); }
        }

        public static readonly DependencyProperty MaxSelectorHeightProperty =
            DependencyProperty.Register(nameof(ContainerHeight), typeof(double), typeof(MeasureTool), new PropertyMetadata(0d, OnContainerHeightChanged));

        private static void OnContainerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MeasureTool).UpdateActualPoints();
        }

        public string ValueDisplay
        {
            get { return (string)GetValue(ValueDisplayProperty); }
            set { SetValue(ValueDisplayProperty, value); }
        }

        public static readonly DependencyProperty ValueDisplayProperty =
            DependencyProperty.Register(nameof(ValueDisplay), typeof(string), typeof(MeasureTool), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(MeasureTool), new PropertyMetadata(0d));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(MeasureTool), new PropertyMetadata(0d));

        public bool UsePixelUnit
        {
            get { return (bool)GetValue(UsePixelUnitProperty); }
            set { SetValue(UsePixelUnitProperty, value); }
        }

        public static readonly DependencyProperty UsePixelUnitProperty =
            DependencyProperty.Register(nameof(UsePixelUnit), typeof(bool), typeof(MeasureTool), new PropertyMetadata(false));

        public bool IsZoomed
        {
            get { return (bool)GetValue(IsZoomedProperty); }
            set { SetValue(IsZoomedProperty, value); }
        }

        public static readonly DependencyProperty IsZoomedProperty =
            DependencyProperty.Register(nameof(IsZoomed), typeof(bool), typeof(MeasureTool), new PropertyMetadata(false));

        public bool CanZoom
        {
            get { return (bool)GetValue(CanZoomProperty); }
            set { SetValue(CanZoomProperty, value); }
        }

        public static readonly DependencyProperty CanZoomProperty =
            DependencyProperty.Register(nameof(CanZoom), typeof(bool), typeof(MeasureTool), new PropertyMetadata(true));

        public Point ZoomPosition
        {
            get { return (Point)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register(nameof(ZoomPosition), typeof(Point), typeof(MeasureTool), new PropertyMetadata(new Point()));


        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(MeasureTool), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int LineThickness
        {
            get { return (int)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(int), typeof(MeasureTool), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush PointsBrush
        {
            get { return (Brush)GetValue(PointsBrushProperty); }
            set { SetValue(PointsBrushProperty, value); }
        }

        public static readonly DependencyProperty PointsBrushProperty =
            DependencyProperty.Register(nameof(PointsBrush), typeof(Brush), typeof(MeasureTool), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(MeasureTool), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Yellow), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));



        public MeasureTool()
        {
            InitializeComponent();
            ContentLine = new Path();
            UpdateThumbsPositions();
            UpdateLine();
        }

        private void DesignerComponent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
        }

        private void Thumb_DragDeltaFrom(object sender, DragDeltaEventArgs e)
        {
            if ((ContainerWidth == 0) || (ContainerHeight == 0))
                return;
            var newFromPoint = new Point(FromPoint.X + e.HorizontalChange, FromPoint.Y + e.VerticalChange);
            FromPoint = CoerceThumbPosition(newFromPoint); ;
            UpdateAllDisplay();
        }

        private Point CoerceThumbPosition(Point point)
        {
            if (point.X < 0)
                point.X = 0;
            if (point.Y < 0)
                point.Y = 0;
            if (point.X > ContainerWidth)
                point.X = ContainerWidth;
            if (point.Y > ContainerHeight)
                point.Y = ContainerHeight;
            return point;
        }

        private void UpdateAllDisplay()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
            UpdateThumbsPositions();
            UpdateMeasureDisplay();
            UpdateLine();
            UpdateButtons();
        }

        private void Thumb_DragDeltaTo(object sender, DragDeltaEventArgs e)
        {
            if ((ContainerWidth == 0) || (ContainerHeight == 0))
                return;
            var newToPoint = new Point(ToPoint.X + e.HorizontalChange, ToPoint.Y + e.VerticalChange);
            ToPoint = CoerceThumbPosition(newToPoint); ;
            UpdateAllDisplay();
        }

        private void UpdateThumbsPositions()
        {
            FromThumb.SetValue(Canvas.LeftProperty, FromPoint.X);
            FromThumb.SetValue(Canvas.TopProperty, FromPoint.Y);
            ToThumb.SetValue(Canvas.LeftProperty, ToPoint.X);
            ToThumb.SetValue(Canvas.TopProperty, ToPoint.Y);
        }

        private void UpdateButtons()
        {
            ButtonZoomFrom.SetValue(Canvas.LeftProperty, FromPoint.X - ButtonZoomFrom.ActualWidth / 2);
            if (FromPoint.Y + ButtonZoomFrom.ActualHeight * 1.5 > ContainerHeight)
                ButtonZoomFrom.SetValue(Canvas.TopProperty, FromPoint.Y - ButtonZoomFrom.ActualHeight * 1.5);
            else
                ButtonZoomFrom.SetValue(Canvas.TopProperty, FromPoint.Y + ButtonZoomFrom.ActualHeight / 2);
            ButtonZoomTo.SetValue(Canvas.LeftProperty, ToPoint.X - ButtonZoomTo.ActualWidth / 2);
            if (ToPoint.Y + ButtonZoomFrom.ActualHeight * 1.5 > ContainerHeight)
                ButtonZoomTo.SetValue(Canvas.TopProperty, ToPoint.Y - ButtonZoomTo.ActualHeight * 1.5);
            else
                ButtonZoomTo.SetValue(Canvas.TopProperty, ToPoint.Y + ButtonZoomTo.ActualHeight / 2);
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
        }

        private void UpdateLine()
        {
            LineGeometry line = new LineGeometry();

            line.StartPoint = FromPoint;
            line.EndPoint = ToPoint;

            (ContentLine as Path).Stroke = LineBrush;
            (ContentLine as Path).StrokeThickness = LineThickness;

            (ContentLine as Path).Data = line;
        }

        private void UpdateMeasureDisplay()
        {
            double lengthPixels = GetMeasurePixel(FromPoint, ToPoint);
            if (UsePixelUnit)
                ValueDisplay = lengthPixels.ToString("0 pixels");
            else
                ValueDisplay = GetMeasuremm(FromPoint, ToPoint).ToString("0.000 mm");

            double verticalOffset = -30;
            if ((Math.Abs(ToPoint.Y-FromPoint.Y) > 30)|| (Math.Abs(ToPoint.X - FromPoint.X) > 100))
            {
                verticalOffset = 0;
            }
        
            DisplayValueText.SetValue(Canvas.LeftProperty, (FromPoint.X + ToPoint.X) / 2 - DisplayValueText.ActualWidth / 2);
            DisplayValueText.SetValue(Canvas.TopProperty, (FromPoint.Y + ToPoint.Y) / 2 - DisplayValueText.ActualHeight / 2+ verticalOffset);
        }

        public object ContentLine
        {
            get
            {
                return ContentComponent.Content;
            }
            protected set
            {
                ContentComponent.Content = value;
            }
        }

        private double GetMeasuremm(Point measurePointFrom, Point measurePointTo)
        {
            double measuremm = 0;
            try
            {
                measuremm = GetMeasurePixel(measurePointFrom, measurePointTo) * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;

            }
            catch 
            {
                
            }

            return measuremm;
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

        private Point? ConvertActualPixelCoordToCameraPixel(Point pixelCoord)
        {
            if ((ImageWidth == 0) || (ImageHeight == 0) || (ContainerWidth == 0) || (ContainerHeight == 0))
                return null;
            return new Point(pixelCoord.X * ImageWidth / ContainerWidth, pixelCoord.Y * ImageHeight / ContainerHeight);
        }

        private Point? ConvertCameraPixelCoordToActualPixel(Point pixelCoord)
        {
            if ((ImageWidth == 0) || (ImageHeight == 0) || (ContainerWidth == 0) || (ContainerHeight == 0))
                return null;
            return new Point(pixelCoord.X * ContainerWidth / ImageWidth, pixelCoord.Y * ContainerHeight / ImageHeight);
        }

        private void ButtonZoomFrom_Click(object sender, RoutedEventArgs e)
        {
            ZoomPosition = FromPoint;
            IsZoomed = true;
        }

        private void ButtonZoomTo_Click(object sender, RoutedEventArgs e)
        {
            ZoomPosition = ToPoint;
            IsZoomed = true;
        }

        private void MeasureToolControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) 
                UpdateAllDisplay();
        }
    }
}
