using System.Windows;
using System.Windows.Media;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    public class CameraCenter : FrameworkElement
    {
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(CameraCenter), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red),FrameworkPropertyMetadataOptions.AffectsRender));

        public int BorderThickness
        {
            get { return (int)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(nameof(BorderThickness), typeof(int), typeof(CameraCenter), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender));

        public double CircleDiameter
        {
            get { return (double)GetValue(CircleDiameterProperty); }
            set { SetValue(CircleDiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircleDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircleDiameterProperty =
            DependencyProperty.Register(nameof(CircleDiameter), typeof(double), typeof(CameraCenter), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(null, new Pen(BorderBrush, BorderThickness), new Point(this.Width / 2, this.Height / 2), CircleDiameter/2, CircleDiameter/2);

            drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point(0, this.Height / 2), new Point((this.Width-CircleDiameter)/2, this.Height / 2));
            drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point((this.Width + CircleDiameter) / 2, this.Height / 2), new Point(this.Width, this.Height / 2));
            drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point(this.Width / 2, 0), new Point(this.Width / 2, (this.Height-CircleDiameter)/2));
            drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point(this.Width / 2, (this.Height + CircleDiameter) / 2), new Point(this.Width / 2, this.Height));

            var fadedBrush = BorderBrush.Clone();
            fadedBrush.Opacity = 0.5;

            drawingContext.DrawLine(new Pen(fadedBrush, BorderThickness), new Point((this.Width - CircleDiameter) / 2, this.Height / 2), new Point(this.Width/2 -1, this.Height / 2));
            drawingContext.DrawLine(new Pen(fadedBrush, BorderThickness), new Point(this.Width / 2 + 1, this.Height / 2), new Point((this.Width + CircleDiameter)/2, this.Height / 2));
            drawingContext.DrawLine(new Pen(fadedBrush, BorderThickness), new Point(this.Width / 2, (this.Height - CircleDiameter) / 2), new Point(this.Width / 2, this.Height  / 2-1));
            drawingContext.DrawLine(new Pen(fadedBrush, BorderThickness), new Point(this.Width / 2, this.Height / 2 + 1), new Point(this.Width / 2, (this.Height + CircleDiameter) / 2));



            //drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point(0, this.Height / 2), new Point(this.Width, this.Height / 2));
            //drawingContext.DrawLine(new Pen(BorderBrush, BorderThickness), new Point(this.Width / 2, 0), new Point(this.Width / 2, this.Height));
            base.OnRender(drawingContext);
        }
    }
}
