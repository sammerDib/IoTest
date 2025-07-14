using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Controls.NavigationControls
{
    public class DieNavigationControl : FrameworkElement
    {
        private DrawingVisual _dieVisual;

        private DrawingVisual _currentPosVisual;

        private PointsVisual _pointsVisual;

        private readonly VisualCollection _visualCollection;

        // Drawing Information
        private double fullWidth => (DieDimentionalCharac is null) ? 100 : DieDimentionalCharac.DieWidth.Millimeters + DieDimentionalCharac.StreetWidth.Millimeters;   // Die + street in mm

        private double fullHeight => (DieDimentionalCharac is null) ? 100 : DieDimentionalCharac.DieHeight.Millimeters + DieDimentionalCharac.StreetHeight.Millimeters;  // Die + street in mm

        private double _drawingRatio; // from mm to drawing pixels

        private double _drawingOffsetX;  // pixels

        private double _drawingOffsetY;  // pixels

        static DieNavigationControl()
        {
        }

        public DieNavigationControl()
        {
            _visualCollection = new VisualCollection(this);
            _dieVisual = new DrawingVisual();
            _visualCollection.Add(_dieVisual);
            _currentPosVisual = new DrawingVisual();
            _visualCollection.Add(_currentPosVisual);
            _pointsVisual = new PointsVisual(this);
            _visualCollection.Add(_pointsVisual);

            Cursor = CustomCursors.Cross;

            //Id = Guid.NewGuid();

            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(this);
            if (designTime)
            {
                return;
            }

            Loaded += DieNavigationControl_Loaded;
            Unloaded += DieNavigationControl_Unloaded;
        }

        private void DieNavigationControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("DieNavigationControl Unloaded " + Id);
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= Axes_PropertyChanged;
            Unloaded -= DieNavigationControl_Unloaded;
            this.MouseDown -= DieNavigationControl_MouseDown;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged -= Axes_PropertyChanged;
            this.ClearValue(AxesPositionXProperty);
            this.ClearValue(AxesPositionYProperty);
            this.ClearValue(CurrentDieRowProperty);
            this.ClearValue(CurrentDieColProperty);
        }

        private void DieNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("DieNavigationControl Loaded " + Id);

            Loaded -= DieNavigationControl_Loaded;
            ServiceLocator.AxesSupervisor.AxesVM.PropertyChanged += Axes_PropertyChanged;
            this.MouseDown += DieNavigationControl_MouseDown;
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

        private void DieNavigationControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);

            if (!ServiceLocator.AxesSupervisor.AxesVM.IsReadyToStartMove)
                return;

            // We convert the pixels coordinates into axes coordinates

            Point pointDie = new Point();

            pointDie.X = ((pos.X - _drawingOffsetX) / _drawingRatio - DieDimentionalCharac.StreetWidth.Millimeters / 2);

            pointDie.Y = DieDimentionalCharac.DieHeight.Millimeters - (pos.Y - _drawingOffsetY) / _drawingRatio + DieDimentionalCharac.StreetHeight.Millimeters / 2;

            XYZTopZBottomPosition waferPosition;

            try
            {
                var currentDieIndexServer = ServiceLocator.AxesSupervisor.AxesVM.CurrentDieIndex;

                waferPosition = ServiceLocator.ReferentialSupervisor.ConvertTo(new XYZTopZBottomPosition(new DieReferential(currentDieIndexServer.Column, currentDieIndexServer.Row), pointDie.X, pointDie.Y, double.NaN, double.NaN), ReferentialTag.Wafer).Result.ToXYZTopZBottomPosition();
            }
            catch (Exception)
            {
                return;
            }

            ServiceLocator.AxesSupervisor.GotoPosition(waferPosition, AxisSpeed.Fast);
        }

        public double AxesPositionX
        {
            get { return (double)GetValue(AxesPositionXProperty); }
            set { SetValue(AxesPositionXProperty, value); }
        }

        public static readonly DependencyProperty AxesPositionXProperty =
            DependencyProperty.Register(nameof(AxesPositionX), typeof(double), typeof(DieNavigationControl), new PropertyMetadata(0D, OnCurrentPositionChanged));

        public double AxesPositionY
        {
            get { return (double)GetValue(AxesPositionYProperty); }
            set { SetValue(AxesPositionYProperty, value); }
        }

        public static readonly DependencyProperty AxesPositionYProperty =
            DependencyProperty.Register(nameof(AxesPositionY), typeof(double), typeof(DieNavigationControl), new PropertyMetadata(0D, OnCurrentPositionChanged));

        private static void OnCurrentPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dieNavigationControl = (DieNavigationControl)obj;
            dieNavigationControl.DrawCurrentPos();
        }

        public int CurrentDieRow
        {
            get { return (int)GetValue(CurrentDieRowProperty); }
            set { SetValue(CurrentDieRowProperty, value); }
        }

        public static readonly DependencyProperty CurrentDieRowProperty =
            DependencyProperty.Register(nameof(CurrentDieRow), typeof(int), typeof(DieNavigationControl), new PropertyMetadata(0, OnCurrentPositionChanged));

        public int CurrentDieCol
        {
            get { return (int)GetValue(CurrentDieColProperty); }
            set { SetValue(CurrentDieColProperty, value); }
        }

        public static readonly DependencyProperty CurrentDieColProperty =
            DependencyProperty.Register(nameof(CurrentDieCol), typeof(int), typeof(DieNavigationControl), new PropertyMetadata(0, OnCurrentPositionChanged));

        public DieDimensionalCharacteristic DieDimentionalCharac
        {
            get { return (DieDimensionalCharacteristic)GetValue(DieDimentionalCharacProperty); }
            set { SetValue(DieDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty DieDimentionalCharacProperty = DependencyProperty.Register(nameof(DieDimentionalCharac), typeof(DieDimensionalCharacteristic), typeof(DieNavigationControl), new PropertyMetadata(null, OnDieDimensionChanged));

        private static void OnDieDimensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = e;
        }

        public double EdgeExclusionThickness
        {
            get { return (double)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(double), typeof(DieNavigationControl), new FrameworkPropertyMetadata(5D, FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DieBrush
        {
            get { return (Brush)GetValue(DieBrushProperty); }
            set { SetValue(DieBrushProperty, value); }
        }

        public static readonly DependencyProperty DieBrushProperty =
            DependencyProperty.Register(nameof(DieBrush), typeof(Brush), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen DieBorderPen
        {
            get { return (Pen)GetValue(DieBorderPenProperty); }
            set { SetValue(DieBorderPenProperty, value); }
        }

        public static readonly DependencyProperty DieBorderPenProperty =
            DependencyProperty.Register(nameof(DieBorderPen), typeof(Pen), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush StreetBrush
        {
            get { return (Brush)GetValue(StreetBrushProperty); }
            set { SetValue(StreetBrushProperty, value); }
        }

        public static readonly DependencyProperty StreetBrushProperty =
            DependencyProperty.Register(nameof(StreetBrush), typeof(Brush), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        public Pen StreetBorderPen
        {
            get { return (Pen)GetValue(StreetBorderPenProperty); }
            set { SetValue(StreetBorderPenProperty, value); }
        }

        public static readonly DependencyProperty StreetBorderPenProperty =
            DependencyProperty.Register(nameof(StreetBorderPen), typeof(Pen), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen CurrentPosBorderPen
        {
            get { return (Pen)GetValue(CurrentPosBorderPenProperty); }
            set { SetValue(CurrentPosBorderPenProperty, value); }
        }

        public static readonly DependencyProperty CurrentPosBorderPenProperty =
            DependencyProperty.Register(nameof(CurrentPosBorderPen), typeof(Pen), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Yellow), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush CurrentPosBrush
        {
            get { return (Brush)GetValue(CurrentPosBrushProperty); }
            set { SetValue(CurrentPosBrushProperty, value); }
        }

        public static readonly DependencyProperty CurrentPosBrushProperty =
            DependencyProperty.Register(nameof(CurrentPosBrush), typeof(Brush), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public List<Point> Points
        {
            get { return (List<Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(List<Point>), typeof(DieNavigationControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush PointsBrush
        {
            get { return (Brush)GetValue(PointsBrushProperty); }
            set { SetValue(PointsBrushProperty, value); }
        }

        public static readonly DependencyProperty PointsBrushProperty =
            DependencyProperty.Register(nameof(PointsBrush), typeof(Brush), typeof(DieNavigationControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = Math.Min(availableSize.Width, 1000);
            var height = Math.Min(availableSize.Height, 1000);

            height = width * fullHeight / fullWidth;

            return new Size(width, height);
        }

        private void UpdateDrawingInfos()
        {
            _drawingRatio = Math.Min(ActualWidth / fullWidth, ActualHeight / fullHeight);

            _drawingOffsetX = 0;
            _drawingOffsetY = 0;

            if (ActualWidth / fullWidth > ActualHeight / fullHeight)
                _drawingOffsetX = (ActualWidth - fullWidth * _drawingRatio) / 2;
            else
                _drawingOffsetY = (ActualHeight - fullHeight * _drawingRatio) / 2;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            UpdateDrawingInfos();
            DrawDie();

            if (!(_pointsVisual is null))
            {
                _pointsVisual.DrawPoints(Points, PointsBrush, DieDimentionalCharac);
            }

            DrawCurrentPos();
        }

        private void DrawDie()
        {
            if (DieDimentionalCharac == null)
                return;

            using (var drawingContext = _dieVisual.RenderOpen())
            {
                // Draw streets

                var streetsRect = new Rect(_drawingOffsetX,
                                           _drawingOffsetY,
                                        fullWidth * _drawingRatio,
                                        fullHeight * _drawingRatio);
                drawingContext.DrawRectangle(StreetBrush, StreetBorderPen, streetsRect);

                var dieRect = new Rect(DieDimentionalCharac.StreetWidth.Millimeters / 2 * _drawingRatio + _drawingOffsetX,
                                        DieDimentionalCharac.StreetHeight.Millimeters / 2 * _drawingRatio + _drawingOffsetY,
                                        DieDimentionalCharac.DieWidth.Millimeters * _drawingRatio,
                                        DieDimentionalCharac.DieHeight.Millimeters * _drawingRatio);

                drawingContext.DrawRectangle(DieBrush, DieBorderPen, dieRect);
            }
        }

        private void DrawCurrentPos()
        {
            if (DieDimentionalCharac == null)
                return;

            if (!IsVisible)
                return;

            using (var drawingContext = _currentPosVisual.RenderOpen())
            {
                XYPosition diePosition;
                try
                {
                    diePosition = ServiceLocator.ReferentialSupervisor.ConvertTo(ServiceLocator.AxesSupervisor.AxesVM.Position.ToAxesPosition(), ReferentialTag.Die).Result.ToXYPosition();
                }
                catch (Exception)
                {
                    return;
                }
                var controlPosX = (diePosition.X) * _drawingRatio + DieDimentionalCharac.StreetWidth.Millimeters / 2 * _drawingRatio + _drawingOffsetX;
                var controlPosY = (-diePosition.Y) * _drawingRatio - DieDimentionalCharac.StreetHeight.Millimeters / 2 * _drawingRatio + ActualHeight + _drawingOffsetY;

                int crossSize = 16;
                // if we are outside, don't draw, it is just to optimize
                if (controlPosX < -crossSize || controlPosY < -crossSize || controlPosX > ActualWidth + crossSize || controlPosY > ActualHeight + crossSize)
                    return;

                drawingContext.PushClip(new RectangleGeometry(new Rect(_drawingOffsetX, _drawingOffsetY, fullWidth * _drawingRatio, fullHeight * _drawingRatio)));
                // Draw a cross

                drawingContext.DrawLine(CurrentPosBorderPen, new Point(controlPosX, controlPosY - crossSize / 2), new Point(controlPosX, controlPosY + crossSize / 2));
                drawingContext.DrawLine(CurrentPosBorderPen, new Point(controlPosX - crossSize / 2, controlPosY), new Point(controlPosX + crossSize / 2, controlPosY));
                drawingContext.Pop();
            }
        }
    }
}
