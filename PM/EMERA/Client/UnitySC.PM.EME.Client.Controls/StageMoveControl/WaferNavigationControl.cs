using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.EME.Client.Controls.StageMoveControl
{
    public class WaferNavigationControl : FrameworkElement
    {
        private WaferVisual _waferVisual;
        private DrawingVisual _currentPosVisual;

        private readonly VisualCollection _visualCollection;

        private Size _fieldOfView = new Size(10, 10);
        static WaferNavigationControl()
        {
        }

        public WaferNavigationControl()
        {
            _visualCollection = new VisualCollection(this);
            _waferVisual = new WaferVisual();
            _visualCollection.Add(_waferVisual);
            _currentPosVisual = new DrawingVisual();
            _visualCollection.Add(_currentPosVisual);

            Cursor = CustomCursors.Cross;

            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (designTime)
            {
                return;
            }
            // We create the binding to the axes supervisor for the current position
            Binding bindingX = new Binding("X");
            bindingX.Source = ClassLocator.Default.GetInstance<AxesVM>().Position;
            bindingX.Mode = BindingMode.OneWay;
            this.SetBinding(AxesPositionXProperty, bindingX);

            Binding bindingY = new Binding("Y");
            bindingY.Source = ClassLocator.Default.GetInstance<AxesVM>().Position;
            bindingY.Mode = BindingMode.OneWay;
            this.SetBinding(AxesPositionYProperty, bindingY);
            // Default Field View
            _fieldOfView.Width = 0.2;
            _fieldOfView.Height = 0.2;

            Loaded += WaferNavigationControl_Loaded;
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferNavigationControl), new FrameworkPropertyMetadata(new WaferDimensionalCharacteristic() { Diameter = 200.Millimeters() }, FrameworkPropertyMetadataOptions.AffectsRender));

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

        private void WaferNavigationControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClassLocator.Default.GetInstance<AxesVM>().PropertyChanged -= Axes_PropertyChanged;
            Unloaded -= WaferNavigationControl_Unloaded;
            this.MouseDown -= WaferNavigationControl_MouseDown;
            Loaded += WaferNavigationControl_Loaded;
        }
        private void WaferNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WaferNavigationControl_Loaded;
            ClassLocator.Default.GetInstance<AxesVM>().PropertyChanged += Axes_PropertyChanged;
            this.MouseDown += WaferNavigationControl_MouseDown;
            Unloaded += WaferNavigationControl_Unloaded;
        }
        private void Axes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AxesVM.IsReadyToStartMove))
            {
                var isAxesReadyToStartMove = ClassLocator.Default.GetInstance<AxesVM>().IsReadyToStartMove;

                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (isAxesReadyToStartMove)
                        Cursor = CustomCursors.Cross;
                    else
                        Cursor = System.Windows.Input.Cursors.Wait;
                }));
            }
        }
        private static void OnAxesPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wafer = (WaferNavigationControl)obj;
            wafer.DrawCurrentPos();
        }
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

        private void WaferNavigationControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            if (!ClassLocator.Default.GetInstance<AxesVM>().IsReadyToStartMove)
                return;

            // We convert the pixels coordinates into axes coordinates
            var waferDiameter = WaferDimentionalCharac.Diameter; //mm

            Point pointWaferRef = new Point();

            pointWaferRef.X = (pos.X - ActualWidth / 2) * waferDiameter.Millimeters / ActualWidth;

            pointWaferRef.Y = -(pos.Y - ActualHeight / 2) * waferDiameter.Millimeters / ActualHeight;

            var destination = new XYZPosition(new WaferReferential(), pointWaferRef.X, pointWaferRef.Y, double.NaN);

            ClassLocator.Default.GetInstance<AxesVM>().DoMoveAxes(destination);
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
        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawWaferVisual();

            DrawCurrentPos();
            base.OnRender(drawingContext);
        }
    }
}
