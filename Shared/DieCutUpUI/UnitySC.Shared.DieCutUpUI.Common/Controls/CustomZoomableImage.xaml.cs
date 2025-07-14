using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using BitmapSource = System.Windows.Media.Imaging.BitmapSource;

namespace UnitySC.Shared.DieCutUpUI.Common.Controls
{
    /// <summary>
    /// Interaction logic for CustomZoomableImage.xaml
    /// </summary>
    public partial class CustomZoomableImage : UserControl
    {
        public CustomZoomableImage()
        {
            InitializeComponent();
            scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            scrollViewer.SizeChanged += ScrollViewer_SizeChanged;
            zoomableImage.MouseRightButtonDown += Image_MouseRightButtonDown;
            zoomableImage.MouseRightButtonUp += Image_MouseRightButtonUp;
            zoomableImage.MouseMove += Image_MouseMove;
        }

        private void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ImageSource == null)
            {
                return;
            }
            Scale = Math.Max(GetMinScaleForZoom(), Scale);
        }

        private Point _panStart;
        private Point _panOrigin;

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ImageTopLeftPosition = GetImageOrigin();
        }

        public BitmapSource ImageSource
        {
            get { return (BitmapSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(CustomZoomableImage), new PropertyMetadata(null, ImageSource_Changed));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(CustomZoomableImage), new PropertyMetadata(0.0));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(CustomZoomableImage), new PropertyMetadata(0.0));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(CustomZoomableImage), new PropertyMetadata(1.0, Scale_Changed));

        public Point ImageTopLeftPosition
        {
            get => (Point)GetValue(ImageTopLeftPositionProperty);
            set => SetValue(ImageTopLeftPositionProperty, value);
        }

        public static readonly DependencyProperty ImageTopLeftPositionProperty =
            DependencyProperty.Register("ImageTopLeftPosition", typeof(Point), typeof(CustomZoomableImage),
                new PropertyMetadata(default(Point)));

        private static void ImageSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (CustomZoomableImage)d;
            if (view.ImageSource == null)
            {
                return;
            }
            view.ImageWidth = view.ImageSource.Width;
            view.ImageHeight = view.ImageSource.Height;
            view.Scale = view.GetMinScaleForZoom();
        }

        private static void Scale_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (CustomZoomableImage)d;

            view.imageScale.ScaleX = view.Scale;
            view.imageScale.ScaleY = view.Scale;

            view.ImageTopLeftPosition = view.GetImageOrigin();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ImageSource == null)
            {
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double minScale = GetMinScaleForZoom();
                Point mousePosition = e.GetPosition(scrollViewer);
                Point mousePosOnFullImage = new Point((mousePosition.X - ImageTopLeftPosition.X) / Scale, (mousePosition.Y - ImageTopLeftPosition.Y) / Scale);

                const double zoomFactor = 1.05;

                if (e.Delta > 0)
                {
                    Scale *= zoomFactor;
                }
                else if (e.Delta < 0)
                {
                    Scale = Math.Max(minScale, Scale / zoomFactor);
                }

                Point mousePosAfterScale = new Point((mousePosOnFullImage.X * Scale) + ImageTopLeftPosition.X, (mousePosOnFullImage.Y * Scale) + ImageTopLeftPosition.Y);
                Point diffToOldPos = new Point(mousePosAfterScale.X - mousePosition.X, mousePosAfterScale.Y - mousePosition.Y);

                double offsetX = scrollViewer.HorizontalOffset + diffToOldPos.X;
                double offsetY = scrollViewer.VerticalOffset + diffToOldPos.Y;

                scrollViewer.ScrollToHorizontalOffset(offsetX);
                scrollViewer.ScrollToVerticalOffset(offsetY);

                ImageTopLeftPosition = GetImageOrigin();

                e.Handled = true;
            }
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            zoomableImage.CaptureMouse();
            _panStart = e.GetPosition(scrollViewer);
            _panOrigin = new Point(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
        }

        private void Image_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            zoomableImage.ReleaseMouseCapture();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (zoomableImage.IsMouseCaptured)
            {
                Vector v = _panStart - e.GetPosition(scrollViewer);
                scrollViewer.ScrollToHorizontalOffset(_panOrigin.X + v.X);
                scrollViewer.ScrollToVerticalOffset(_panOrigin.Y + v.Y);
            }
        }

        private double GetMinScaleForZoom()
        {
            return (scrollViewer.ActualHeight - GetVerticalScrollBarSize()) / ImageSource.Height;
        }

        private double GetVerticalScrollBarSize()
        {
            bool isVerticalScrollBarVisible = scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible;
            return isVerticalScrollBarVisible ? SystemParameters.VerticalScrollBarWidth : 0.0;
        }

        private Point GetImageOrigin()
        {
            var transform = zoomableImage.TransformToVisual(scrollViewer);
            var origin = transform.Transform(new Point(0, 0));
            return origin;
        }
    }
}
