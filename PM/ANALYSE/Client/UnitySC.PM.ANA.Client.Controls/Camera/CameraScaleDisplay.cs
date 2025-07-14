using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    // Draws a line and displays a value
    public class CameraScaleDisplay : Control
    {
        // size of the small lines at the begining and the en of the line this size is added to the line thickness
        private const double smallLinesHeight = 6d;

        public CameraScaleDisplay()
        {
            IsHitTestVisible = false;
        }

        public double ScaleLength
        {
            get { return (double)GetValue(ScaleLengthProperty); }
            set { SetValue(ScaleLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleLengthProperty =
            DependencyProperty.Register(nameof(ScaleLength), typeof(double), typeof(CameraScaleDisplay), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public string FullScaleValue
        {
            get { return (string)GetValue(FullScaleValueProperty); }
            set { SetValue(FullScaleValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullScaleValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullScaleValueProperty =
            DependencyProperty.Register(nameof(FullScaleValue), typeof(string), typeof(CameraScaleDisplay), new FrameworkPropertyMetadata("1mm", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(CameraScaleDisplay), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int LineThickness
        {
            get { return (int)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(int), typeof(CameraScaleDisplay), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double SpaceBetweenTextAndLine
        {
            get { return (double)GetValue(SpaceBetweenTextAndLineProperty); }
            set { SetValue(SpaceBetweenTextAndLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpaceBetweenTextAndLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpaceBetweenTextAndLineProperty =
            DependencyProperty.Register(nameof(SpaceBetweenTextAndLine), typeof(double), typeof(CameraScaleDisplay), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pointLeft = new Point(0, this.ActualHeight - (double)LineThickness / 2 - smallLinesHeight / 2);
            var pointRight = new Point(ScaleLength, this.ActualHeight - (double)LineThickness / 2 - smallLinesHeight / 2);
            drawingContext.DrawLine(new Pen(LineBrush, LineThickness), pointLeft, pointRight);

            // Draw the small line on the left
            drawingContext.DrawLine(new Pen(LineBrush, LineThickness), new Point(0, this.ActualHeight), new Point(0, this.ActualHeight - LineThickness - smallLinesHeight));

            // Draw the small line on the right
            drawingContext.DrawLine(new Pen(LineBrush, LineThickness), new Point(ScaleLength, this.ActualHeight), new Point(ScaleLength, this.ActualHeight - LineThickness - smallLinesHeight));

            // We find the middle of the line
            var centerPosition = new Point(pointLeft.X + (pointRight.X - pointLeft.X) / 2, pointLeft.Y + (pointRight.Y - pointLeft.Y) / 2);

            var scaleFormatedText = GetScaleFormatedText();

            if (scaleFormatedText != null)
            {
                var textPosition = new Point(centerPosition.X - scaleFormatedText.Width / 2, this.ActualHeight - LineThickness - smallLinesHeight / 2 - scaleFormatedText.Height - SpaceBetweenTextAndLine);

                drawingContext.DrawText(scaleFormatedText, textPosition);
            }
            base.OnRender(drawingContext);
        }

        private FormattedText GetScaleFormatedText()
        {
            if (FullScaleValue == null)
                return null;

            return new FormattedText(FullScaleValue, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, LineBrush, 1.25);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var scaleFormatedText = GetScaleFormatedText();

            if (scaleFormatedText == null)
                return base.MeasureOverride(constraint);

            var desiredSize = new Size(Math.Max(scaleFormatedText.Width, ScaleLength), SpaceBetweenTextAndLine + LineThickness + scaleFormatedText.Height);

            //Console.WriteLine("CameraScaleDisplay Desired Size :" + desiredSize.ToString());

            return desiredSize;
        }
    }
}
