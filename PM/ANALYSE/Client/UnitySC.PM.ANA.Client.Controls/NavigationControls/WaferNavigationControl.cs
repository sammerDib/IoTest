using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.Controls.NavigationControls
{
    public class WaferNavigationControl : FrameworkElement
    {
        private WaferVisual _waferVisual;
        private DrawingVisual _currentPosVisual;
        private PointsVisual _pointsVisual;

        private readonly VisualCollection _visualCollection;

        //   private static WaferDimensionalCharacteristic _designWaferCharac;

        //Field of view in mm

        private Size _fieldOfView = new Size(10, 10);

        static WaferNavigationControl()
        {
            //_designWaferCharac = new WaferDimensionalCharacteristic();
            //_designWaferCharac.Diameter = 300;
        }

        public WaferNavigationControl()
        {
            _visualCollection = new VisualCollection(this);
            _waferVisual = new WaferVisual();
            _visualCollection.Add(_waferVisual);
            _currentPosVisual = new DrawingVisual();
            _visualCollection.Add(_currentPosVisual);
            _pointsVisual = new PointsVisual(this);
            _visualCollection.Add(_pointsVisual);

            Cursor = CustomCursors.Cross;

            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (designTime)
            {
                return;
            }

            // We create the binding to the axes supervisor for the current position
            Binding bindingX = new Binding("X");
            bindingX.Source = ServiceLocator.AxesSupervisor.AxesVM.Position;
            bindingX.Mode = BindingMode.OneWay;
            this.SetBinding(AxesPositionXProperty, bindingX);

            Binding bindingY = new Binding("Y");
            bindingY.Source = ServiceLocator.AxesSupervisor.AxesVM.Position;
            bindingY.Mode = BindingMode.OneWay;
            this.SetBinding(AxesPositionYProperty, bindingY);

            // We retrieve information about the current objective in order to calculate the field of view
            if (!(ServiceLocator.CamerasSupervisor.Camera?.CameraInfo is null))
            {
                string currentCameraId = ServiceLocator.CamerasSupervisor.Objective.DeviceID;
                if (ServiceLocator.CalibrationSupervisor.ObjectiveIsCalibrated(currentCameraId))
                {
                    _fieldOfView.Width = ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(currentCameraId).Image.PixelSizeX.Millimeters;
                    _fieldOfView.Height = ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(currentCameraId).Image.PixelSizeY.Millimeters;
                }
                else
                {
                    // Default Field View
                    _fieldOfView.Width = 0.2;
                    _fieldOfView.Height = 0.2;
                }
            }

            Loaded += WaferNavigationControl_Loaded;
        }

        private void WaferNavigationControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= Axes_PropertyChanged;
            Unloaded -= WaferNavigationControl_Unloaded;
            this.MouseDown -= WaferNavigationControl_MouseDown;
            Loaded += WaferNavigationControl_Loaded;
        }

        private void WaferNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WaferNavigationControl_Loaded;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged += Axes_PropertyChanged;
            this.MouseDown += WaferNavigationControl_MouseDown;
            Unloaded += WaferNavigationControl_Unloaded;
        }

        private void Axes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxesVM.IsReadyToStartMove))
            {
                var isAxesReadyToStartMove = ServiceLocator.AxesSupervisor.AxesVM.IsReadyToStartMove;

                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (isAxesReadyToStartMove)
                        Cursor = CustomCursors.Cross;
                    else
                        Cursor = Cursors.Wait;
                }));
            }
        }

        private void WaferNavigationControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            if (!ServiceLocator.AxesSupervisor.AxesVM.IsReadyToStartMove)
                return;

            // We convert the pixels coordinates into axes coordinates
            var waferDiameter = WaferDimentionalCharac.Diameter; //mm

            Point pointWaferRef = new Point();

            pointWaferRef.X = (pos.X - ActualWidth / 2) * waferDiameter.Millimeters / ActualWidth;

            pointWaferRef.Y = -(pos.Y - ActualHeight / 2) * waferDiameter.Millimeters / ActualHeight;

            var destination = new XYZTopZBottomPosition(new WaferReferential(), pointWaferRef.X, pointWaferRef.Y, double.NaN, double.NaN);
            ServiceLocator.AxesSupervisor.GotoPosition(destination, AxisSpeed.Fast);
        }

        public double AxesPositionX
        {
            get { return (double)GetValue(AxesPositionXProperty); }
            set { SetValue(AxesPositionXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxesPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxesPositionXProperty =
            DependencyProperty.Register(nameof(AxesPositionX), typeof(double), typeof(WaferNavigationControl), new PropertyMetadata(0D, OnAxesPositionChanged));

        public double AxesPositionY
        {
            get { return (double)GetValue(AxesPositionYProperty); }
            set { SetValue(AxesPositionYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxesPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxesPositionYProperty =
            DependencyProperty.Register(nameof(AxesPositionY), typeof(double), typeof(WaferNavigationControl), new PropertyMetadata(0D, OnAxesPositionChanged));

        private static void OnAxesPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wafer = (WaferNavigationControl)obj;
            wafer.DrawCurrentPos();
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new WaferDimensionalCharacteristic() { Diameter = 300.Millimeters() }, FrameworkPropertyMetadataOptions.AffectsRender));

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(5.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EdgeExclusionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen CurrentPosBorderPen
        {
            get { return (Pen)GetValue(CurrentPosBorderPenProperty); }
            set { SetValue(CurrentPosBorderPenProperty, value); }
        }

        public static readonly DependencyProperty CurrentPosBorderPenProperty =
            DependencyProperty.Register(nameof(CurrentPosBorderPen), typeof(Pen), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Yellow), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush CurrentPosBrush
        {
            get { return (Brush)GetValue(CurrentPosBrushProperty); }
            set { SetValue(CurrentPosBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPosBrushProperty =
            DependencyProperty.Register(nameof(CurrentPosBrush), typeof(Brush), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public List<Point> Points
        {
            get { return (List<Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(List<Point>), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush PointsBrush
        {
            get { return (Brush)GetValue(PointsBrushProperty); }
            set { SetValue(PointsBrushProperty, value); }
        }

        public static readonly DependencyProperty PointsBrushProperty =
            DependencyProperty.Register(nameof(PointsBrush), typeof(Brush), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkRed), FrameworkPropertyMetadataOptions.AffectsRender));

        private void DrawWaferVisual()
        {
            _waferVisual.Width = ActualWidth;
            _waferVisual.Height = ActualHeight;
            _waferVisual.WaferInfos = WaferDimentionalCharac;
            _waferVisual.WaferBrush = WaferBrush;
            _waferVisual.EdgeExclusionBrush = EdgeExclusionBrush;
            _waferVisual.WaferBorderPen = WaferBorderPen;
            _waferVisual.EdgeExclusionThickness = EdgeExclusionThickness;

            _waferVisual.Draw();
        }

        private void DrawCurrentPos()
        {
            if (WaferDimentionalCharac == null)
                return;

            using (var drawingContext = _currentPosVisual.RenderOpen())
            {
                var controlPosX = AxesPositionX * ActualWidth / WaferDimentionalCharac.Diameter.Millimeters + ActualWidth / 2;
                var controlPosY = -AxesPositionY * ActualHeight / WaferDimentionalCharac.Diameter.Millimeters + ActualHeight / 2;
                var minCurPosWidth = 5;
                var minCurPosHeight = 5;
                drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));

                // We calculate the field of view in pixels
                var fieldOfViewWidth = Math.Max(_fieldOfView.Width * ActualWidth / WaferDimentionalCharac.Diameter.Millimeters, minCurPosWidth);
                var fieldOfViewHeight = Math.Max(_fieldOfView.Height * ActualHeight / WaferDimentionalCharac.Diameter.Millimeters, minCurPosHeight);

                drawingContext.DrawRectangle(CurrentPosBrush, CurrentPosBorderPen, new Rect(controlPosX - fieldOfViewWidth / 2, controlPosY - fieldOfViewHeight / 2, fieldOfViewWidth, fieldOfViewHeight));
                drawingContext.Pop();
            }
        }

        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = Math.Min(availableSize.Width, 1000);
            var height = Math.Min(availableSize.Height, 1000);

            width = Math.Min(width, height);
            height = width;

            return new Size(width, height);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawWaferVisual();

            if (!(_pointsVisual is null))
            {
                _pointsVisual.DrawPoints(Points, PointsBrush, WaferDimentionalCharac);
            }

            DrawCurrentPos();
            base.OnRender(drawingContext);
        }
    }
}
