using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.PM.EME.Client.Controls.Camera
{
    public class Scale : Control
    {
        // size of the small lines at the beginning and the end of the line this size is added to the line thickness
        private const double SmallLinesHeight = 6d;

        public Scale()
        {
            IsHitTestVisible = false;
        }

        public string ScaleLengthValue
        {
            get => (string)GetValue(ScaleLengthValueProperty);
            set => SetValue(ScaleLengthValueProperty, value);
        }

        public static readonly DependencyProperty ScaleLengthValueProperty =
            DependencyProperty.Register(nameof(ScaleLengthValue), typeof(string), typeof(Scale),
                new FrameworkPropertyMetadata("1mm",
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(Scale), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Red), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public int LineThickness
        {
            get { return (int)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(int), typeof(Scale), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (ScaleLengthValue?.Length == 0)
            {
                return;
            }

            var pen = new Pen(LineBrush, LineThickness);

            // Draw scale line 
            double lineY = ActualHeight - ((LineThickness + SmallLinesHeight) / 2);
            var pointLeft = new Point(0, lineY);
            var pointRight = new Point(ActualWidth, lineY);
            drawingContext.DrawLine(pen, pointLeft, pointRight);

            // Draw edge lines on the left and right
            double edgeTopY = ActualHeight;
            double edgeBottomY = ActualHeight - LineThickness - SmallLinesHeight;
            drawingContext.DrawLine(pen, new Point(0, edgeTopY), new Point(0, edgeBottomY));
            drawingContext.DrawLine(pen, new Point(ActualWidth, edgeTopY), new Point(ActualWidth, edgeBottomY));

            // Draw text
            var scaleFormattedText = new FormattedText(ScaleLengthValue, CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize,
                LineBrush, 1.25);
            double textX = ActualWidth / 2 - scaleFormattedText.Width / 2;
            double textY = ActualHeight - LineThickness - SmallLinesHeight / 2 - scaleFormattedText.Height;
            var textPosition = new Point(textX, textY);
            drawingContext.DrawText(scaleFormattedText, textPosition);

            base.OnRender(drawingContext);
        }
    }
}
