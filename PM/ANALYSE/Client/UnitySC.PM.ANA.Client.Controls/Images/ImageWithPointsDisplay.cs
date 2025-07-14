using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Controls
{
    public class ImageWithPointsDisplay : FrameworkElement
    {
        public Color CrossColor
        {
            get { return (Color)GetValue(CrossColorProperty); }
            set { SetValue(CrossColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossColorProperty =
            DependencyProperty.Register(nameof(CrossColor), typeof(Color), typeof(ImageWithPointsDisplay), new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));

        public int CrossThickness
        {
            get { return (int)GetValue(CrossThicknessProperty); }
            set { SetValue(CrossThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossThicknessProperty =
            DependencyProperty.Register(nameof(CrossThickness), typeof(int), typeof(ImageWithPointsDisplay), new FrameworkPropertyMetadata(2, FrameworkPropertyMetadataOptions.AffectsRender));

        public double CrossSize
        {
            get { return (double)GetValue(CrossSizeProperty); }
            set { SetValue(CrossSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CircleDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossSizeProperty =
            DependencyProperty.Register(nameof(CrossSize), typeof(double), typeof(ImageWithPointsDisplay), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        public List<ServicePoint> Points
        {
            get { return (List<ServicePoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(List<ServicePoint>), typeof(ImageWithPointsDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ImageWithPointsDisplay), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender|FrameworkPropertyMetadataOptions.AffectsMeasure));


        //public int ImageWidth
        //{
        //    get { return (int)GetValue(ImageWidthProperty); }
        //    set { SetValue(ImageWidthProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ImageWidthProperty =
        //    DependencyProperty.Register(nameof(ImageWidth), typeof(int), typeof(PointsDisplay), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));



        //public int ImageHeight
        //{
        //    get { return (int)GetValue(ImageHeightProperty); }
        //    set { SetValue(ImageHeightProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ImageHeightProperty =
        //    DependencyProperty.Register(nameof(ImageHeight), typeof(int), typeof(PointsDisplay), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));



        protected override void OnRender(DrawingContext drawingContext)
        {
            var crossPen = new Pen(new SolidColorBrush(CrossColor), CrossThickness);

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            drawingContext.DrawImage(Image, new Rect(0, 0, ActualWidth, ActualHeight));

            if (Points is null)
                return;

            foreach (var point in Points)
            {
                // Draw a cross

                DrawCross(drawingContext, new Point(point.X * ActualWidth / Image.Width, point.Y * ActualHeight / Image.Height), crossPen, CrossSize);
            }

            base.OnRender(drawingContext);
        }

        private static void DrawCross(DrawingContext drawingContext, Point point, Pen crossPen, double crossSize)
        {
            drawingContext.DrawLine(crossPen, new Point(point.X, point.Y - crossSize / 2), new Point(point.X, point.Y + crossSize / 2));
            drawingContext.DrawLine(crossPen, new Point(point.X - crossSize / 2, point.Y), new Point(point.X + crossSize / 2, point.Y));
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
