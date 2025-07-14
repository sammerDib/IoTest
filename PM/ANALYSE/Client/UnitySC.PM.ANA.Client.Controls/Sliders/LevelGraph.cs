using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.PM.ANA.Client.Controls
{
    public class LevelGraph : Control
    {
        // public Brush BorderBrush
        // {
        //     get { return (Brush)GetValue(BorderBrushProperty); }
        //     set { SetValue(BorderBrushProperty, value); }
        // }

        //public static readonly DependencyProperty BorderBrushProperty =
        //     DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(LevelGraph), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red),FrameworkPropertyMetadataOptions.AffectsRender));

        // public int BorderThickness
        // {
        //     get { return (int)GetValue(BorderThicknessProperty); }
        //     set { SetValue(BorderThicknessProperty, value); }
        // }

        // public static readonly DependencyProperty BorderThicknessProperty =
        //     DependencyProperty.Register(nameof(BorderThickness), typeof(int), typeof(LevelGraph), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush GraphBackgroundBrush
        {
            get { return (Brush)GetValue(GraphBackgroundBrushProperty); }
            set { SetValue(GraphBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty GraphBackgroundBrushProperty =
             DependencyProperty.Register(nameof(GraphBackgroundBrush), typeof(Brush), typeof(LevelGraph), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.LightGray), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush GraphForegroundBrush
        {
            get { return (Brush)GetValue(GraphForegroundProperty); }
            set { SetValue(GraphForegroundProperty, value); }
        }

        public static readonly DependencyProperty GraphForegroundProperty =
             DependencyProperty.Register(nameof(GraphForegroundBrush), typeof(Brush), typeof(LevelGraph), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));

        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register(nameof(Level), typeof(double), typeof(LevelGraph), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set { SetValue(LeftTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register(nameof(LeftText), typeof(string), typeof(LevelGraph), new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.AffectsRender));


        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set { SetValue(RightTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register(nameof(RightText), typeof(string), typeof(LevelGraph), new FrameworkPropertyMetadata("100 %", FrameworkPropertyMetadataOptions.AffectsRender));



        protected override void OnRender(DrawingContext drawingContext)
        {
            FormattedText ftLeft = new FormattedText(LeftText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(this.FontFamily.Source), this.FontSize, Foreground, 1.25);
            FormattedText ftRight = new FormattedText(RightText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(this.FontFamily.Source), this.FontSize, Foreground, 1.25);

            var textHeight = ftLeft.Height;

            var graphHeight = this.ActualHeight - textHeight;

            // Draw back Triangle
            Point point1 = new Point(0, graphHeight);
            Point point2 = new Point(this.ActualWidth, graphHeight);
            Point point3 = new Point(this.ActualWidth, 0);
            StreamGeometry backTriangleGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = backTriangleGeometry.Open())
            {
                geometryContext.BeginFigure(point1, true, true);
                PointCollection points = new PointCollection
                                             {
                                                 point2,
                                                 point3
                                             };
                geometryContext.PolyLineTo(points, true, true);
            }
            backTriangleGeometry.Freeze();
            drawingContext.DrawGeometry(GraphBackgroundBrush, null, backTriangleGeometry);

            // Draw front Triangle
            point2 = new Point(this.ActualWidth * Level / 100, graphHeight);
            point3 = new Point(this.ActualWidth * Level / 100, graphHeight - (graphHeight) * Level / 100);
            StreamGeometry frontTriangleGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = frontTriangleGeometry.Open())
            {
                geometryContext.BeginFigure(point1, true, true);
                PointCollection points = new PointCollection
                                             {
                                                 point2,
                                                 point3
                                             };
                geometryContext.PolyLineTo(points, true, true);
            }
            frontTriangleGeometry.Freeze();
            drawingContext.DrawGeometry(GraphForegroundBrush, null, frontTriangleGeometry);

            drawingContext.DrawText(ftLeft, new Point(0, graphHeight));

            drawingContext.DrawText(ftRight, new Point(this.ActualWidth - ftRight.Width, graphHeight));

            base.OnRender(drawingContext);
        }
    }
}
