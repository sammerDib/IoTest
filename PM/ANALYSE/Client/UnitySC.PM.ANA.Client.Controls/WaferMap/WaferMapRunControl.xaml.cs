using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Controls
{
    /// <summary>
    /// Interaction logic for WaferMapRunControl.xaml
    /// </summary>
    public partial class WaferMapRunControl : UserControl
    {
        // Width used to display the current point and the selected point
        private readonly int _pointsWidth = 6;

        private double _drawingRatio => ActualWidth / WaferDimentionalCharac.Diameter.Millimeters;

        public WaferMapRunControl()
        {
            InitializeComponent();
            PointCurrentDisplay.Visibility = Visibility.Collapsed;
            PointSelectedDisplay.Visibility = Visibility.Collapsed;
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register(nameof(WaferDimentionalCharac), typeof(WaferDimensionalCharacteristic), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Length EdgeExclusionThickness
        {
            get { return (Length)GetValue(EdgeExclusionThicknessProperty); }
            set { SetValue(EdgeExclusionThicknessProperty, value); }
        }

        public static readonly DependencyProperty EdgeExclusionThicknessProperty =
            DependencyProperty.Register(nameof(EdgeExclusionThickness), typeof(Length), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(0.Millimeters(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush EdgeExclusionBrush
        {
            get { return (Brush)GetValue(EdgeExclusionBrushProperty); }
            set { SetValue(EdgeExclusionBrushProperty, value); }
        }

        public static readonly DependencyProperty EdgeExclusionBrushProperty =
            DependencyProperty.Register(nameof(EdgeExclusionBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Pink), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush WaferBrush
        {
            get { return (Brush)GetValue(WaferBrushProperty); }
            set { SetValue(WaferBrushProperty, value); }
        }

        public static readonly DependencyProperty WaferBrushProperty =
            DependencyProperty.Register(nameof(WaferBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Pen WaferBorderPen
        {
            get { return (Pen)GetValue(WaferBorderPenProperty); }
            set { SetValue(WaferBorderPenProperty, value); }
        }

        public static readonly DependencyProperty WaferBorderPenProperty =
            DependencyProperty.Register(nameof(WaferBorderPen), typeof(Pen), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new Pen(new SolidColorBrush(Colors.Gray), 2), FrameworkPropertyMetadataOptions.AffectsRender));

        [Browsable(true)]
        public Brush DiesBrush
        {
            get { return (Brush)GetValue(DiesBrushProperty); }
            set { SetValue(DiesBrushProperty, value); }
        }

        public static readonly DependencyProperty DiesBrushProperty =
            DependencyProperty.Register(nameof(DiesBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightBlue), FrameworkPropertyMetadataOptions.AffectsRender));

        public List<Point> MeasurePoints
        {
            get { return (List<Point>)GetValue(MeasurePointsProperty); }
            set { SetValue(MeasurePointsProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointsProperty =
            DependencyProperty.Register(nameof(MeasurePoints), typeof(List<Point>), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(null));

        public Brush MeasurePointsBrush
        {
            get { return (Brush)GetValue(MeasurePointsBrushProperty); }
            set { SetValue(MeasurePointsBrushProperty, value); }
        }

        public static readonly DependencyProperty MeasurePointsBrushProperty =
            DependencyProperty.Register(nameof(MeasurePointsBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DarkRed), FrameworkPropertyMetadataOptions.AffectsRender));

        public Point? SelectedPoint
        {
            get { return (Point?)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        public static readonly DependencyProperty SelectedPointProperty =
            DependencyProperty.Register(nameof(SelectedPoint), typeof(Point?), typeof(WaferMapRunControl), new PropertyMetadata(null, OnSelectedPointChanged));

        private static void OnSelectedPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaferMapRunControl).UpdateSelectedPointDisplay();
        }

        private void UpdateSelectedPointDisplay()
        {
            UpdatePointDisplay(SelectedPoint, PointSelectedDisplay, _pointsWidth + 6);
        }

        public Brush SelectedPointBrush
        {
            get { return (Brush)GetValue(SelectedPointBrushProperty); }
            set { SetValue(SelectedPointBrushProperty, value); }
        }

        public static readonly DependencyProperty SelectedPointBrushProperty =
            DependencyProperty.Register(nameof(SelectedPointBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Blue), FrameworkPropertyMetadataOptions.AffectsRender));

        public Point? CurrentPoint
        {
            get { return (Point?)GetValue(CurrentPointProperty); }
            set { SetValue(CurrentPointProperty, value); }
        }

        public static readonly DependencyProperty CurrentPointProperty =
            DependencyProperty.Register(nameof(CurrentPoint), typeof(Point?), typeof(WaferMapRunControl), new PropertyMetadata(null, OnCurrentPointChanged));

        public Brush CurrentPointBrush
        {
            get { return (Brush)GetValue(CurrentPointBrushProperty); }
            set { SetValue(CurrentPointBrushProperty, value); }
        }

        public static readonly DependencyProperty CurrentPointBrushProperty =
            DependencyProperty.Register(nameof(CurrentPointBrush), typeof(Brush), typeof(WaferMapRunControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Blue), FrameworkPropertyMetadataOptions.AffectsRender));

        private static void OnCurrentPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WaferMapRunControl).UpdateCurrentPointDisplay();
        }

        private void UpdateCurrentPointDisplay()
        {
            UpdatePointDisplay(CurrentPoint, PointCurrentDisplay, _pointsWidth);
        }

        private void UpdatePointDisplay(Point? point, System.Windows.Shapes.Rectangle rectangleDisplay, int rectangleSize)
        {
            if ((point is null) || (WaferDimentionalCharac is null))
            {
                rectangleDisplay.Visibility = Visibility.Collapsed;
                return;
            }

            rectangleDisplay.Visibility = Visibility.Visible;
         

            var squarePos= PointsVisual.CalculateRectanglePosition((Point)point, _drawingRatio, rectangleSize, rectangleSize, WaferDimentionalCharac);


            Canvas.SetLeft(rectangleDisplay, squarePos.X);
            Canvas.SetTop(rectangleDisplay, squarePos.Y);
            rectangleDisplay.Width = rectangleSize;
            rectangleDisplay.Height = rectangleSize;
        }
        



        public WaferMapResult WaferMap
        {
            get { return (WaferMapResult)GetValue(WaferMapProperty); }
            set { SetValue(WaferMapProperty, value); }
        }

        public static readonly DependencyProperty WaferMapProperty =
            DependencyProperty.Register(nameof(WaferMap), typeof(WaferMapResult), typeof(WaferMapRunControl), new PropertyMetadata(null, OnWaferMapChanged));

        private static void OnWaferMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
