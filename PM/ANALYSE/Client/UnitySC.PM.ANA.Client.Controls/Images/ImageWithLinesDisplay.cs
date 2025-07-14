using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Client.Controls
{
    public class ImageWithLinesDisplay : FrameworkElement
    {
        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(nameof(LineColor), typeof(Color), typeof(ImageWithLinesDisplay), new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public int LineThickness
        {
            get { return (int)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(int), typeof(ImageWithLinesDisplay), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender));


        public List<(ServicePoint pt1, ServicePoint pt2)> Lines
        {
            get { return (List<(ServicePoint pt1, ServicePoint pt2)>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register(nameof(Lines), typeof(List<(ServicePoint pt1, ServicePoint pt2)>), typeof(ImageWithLinesDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ImageWithLinesDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));



        protected override void OnRender(DrawingContext drawingContext)
        {
            var linePen = new Pen(new SolidColorBrush(LineColor), LineThickness);

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            drawingContext.DrawImage(Image, new Rect(0, 0, ActualWidth, ActualHeight));

            if (Lines is null)
                return;

            foreach (var line in Lines) 
            {
                drawingContext.DrawLine(linePen, new Point(line.pt1.X * ActualWidth / Image.Width, line.pt1.Y * ActualHeight / Image.Height), new Point(line.pt2.X * ActualWidth / Image.Width, line.pt2.Y * ActualHeight / Image.Height));
            }

            base.OnRender(drawingContext);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();

            if (Image is null)
                return availableSize;

            if ((availableSize.Width / Image.Width) * Image.Height < availableSize.Height)
            {
                desiredSize.Width = availableSize.Width;
                desiredSize.Height = (availableSize.Width / Image.Width) * Image.Height;
            }
            else
            {
                desiredSize.Height = availableSize.Height;
                desiredSize.Width = (availableSize.Height / Image.Height) * Image.Width;
            }


            return desiredSize;
        }

    }
}
