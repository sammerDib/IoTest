using System;
using System.Windows;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Extensions
{
    #region Documentation Tags

    /// <summary>
    ///     WPF rounded corners. It is used to display an image with rounded corners
    /// </summary>

    #endregion Documentation Tags

    public class RoundedCornersExt
    {
        #region Mask Property

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(RoundedCornersExt), new FrameworkPropertyMetadata(CornerRadiusChangedCallback));

        private static void CornerRadiusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as FrameworkElement;
            if (_this == null)
                return;

            if (e.OldValue is CornerRadius)
            {
                _this.SizeChanged -= Framework_SizeChanged;
            }

            SetRoundClip(_this);

            _this.SizeChanged += Framework_SizeChanged;
        }

        private static void Framework_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            if (fe is null)
                return;

            SetRoundClip(fe);
        }

        private static void SetRoundClip(FrameworkElement fe)
        {
            var clipGeometry = new RectangleGeometry
            {
                Rect = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight),
                RadiusX = GetCornerRadius(fe).TopLeft,
                RadiusY = GetCornerRadius(fe).TopLeft
            };

            fe.Clip = clipGeometry;
        }

        public static Geometry GetRoundRectangleGeometry(Rect baseRect, CornerRadius cornerRadius)
        {
            if (cornerRadius.TopLeft < double.Epsilon)
                cornerRadius.TopLeft = 0.0;
            if (cornerRadius.TopRight < double.Epsilon)
                cornerRadius.TopRight = 0.0;
            if (cornerRadius.BottomLeft < double.Epsilon)
                cornerRadius.BottomLeft = 0.0;
            if (cornerRadius.BottomRight < double.Epsilon)
                cornerRadius.BottomRight = 0.0;

            // Create the rectangles for the corners that needs to be curved in the base rectangle
            // TopLeft Rectangle
            var topLeftRect = new Rect(baseRect.Location.X,
                                        baseRect.Location.Y,
                                        cornerRadius.TopLeft,
                                        cornerRadius.TopLeft);
            // TopRight Rectangle
            var topRightRect = new Rect(baseRect.Location.X + baseRect.Width - cornerRadius.TopRight,
                                         baseRect.Location.Y,
                                         cornerRadius.TopRight,
                                         cornerRadius.TopRight);
            // BottomRight Rectangle
            var bottomRightRect = new Rect(baseRect.Location.X + baseRect.Width - cornerRadius.BottomRight,
                                            baseRect.Location.Y + baseRect.Height - cornerRadius.BottomRight,
                                            cornerRadius.BottomRight,
                                            cornerRadius.BottomRight);
            // BottomLeft Rectangle
            var bottomLeftRect = new Rect(baseRect.Location.X,
                                           baseRect.Location.Y + baseRect.Height - cornerRadius.BottomLeft,
                                           cornerRadius.BottomLeft,
                                           cornerRadius.BottomLeft);

            // Adjust the width of the TopLeft and TopRight rectangles so that they are proportional to the width of the baseRect
            if (topLeftRect.Right > topRightRect.Left)
            {
                double newWidth = topLeftRect.Width / (topLeftRect.Width + topRightRect.Width) * baseRect.Width;
                topLeftRect = new Rect(topLeftRect.Location.X, topLeftRect.Location.Y, newWidth, topLeftRect.Height);
                topRightRect = new Rect(baseRect.Left + newWidth, topRightRect.Location.Y, Math.Max(0.0, baseRect.Width - newWidth), topRightRect.Height);
            }

            // Adjust the height of the TopRight and BottomRight rectangles so that they are proportional to the height of the baseRect
            if (topRightRect.Bottom > bottomRightRect.Top)
            {
                double newHeight = topRightRect.Height / (topRightRect.Height + bottomRightRect.Height) * baseRect.Height;
                topRightRect = new Rect(topRightRect.Location.X, topRightRect.Location.Y, topRightRect.Width, newHeight);
                bottomRightRect = new Rect(bottomRightRect.Location.X, baseRect.Top + newHeight, bottomRightRect.Width, Math.Max(0.0, baseRect.Height - newHeight));
            }

            // Adjust the width of the BottomLeft and BottomRight rectangles so that they are proportional to the width of the baseRect
            if (bottomRightRect.Left < bottomLeftRect.Right)
            {
                double newWidth = bottomLeftRect.Width / (bottomLeftRect.Width + bottomRightRect.Width) * baseRect.Width;
                bottomLeftRect = new Rect(bottomLeftRect.Location.X, bottomLeftRect.Location.Y, newWidth, bottomLeftRect.Height);
                bottomRightRect = new Rect(baseRect.Left + newWidth, bottomRightRect.Location.Y, Math.Max(0.0, baseRect.Width - newWidth), bottomRightRect.Height);
            }

            // Adjust the height of the TopLeft and BottomLeft rectangles so that they are proportional to the height of the baseRect
            if (bottomLeftRect.Top < topLeftRect.Bottom)
            {
                double newHeight = topLeftRect.Height / (topLeftRect.Height + bottomLeftRect.Height) * baseRect.Height;
                topLeftRect = new Rect(topLeftRect.Location.X, topLeftRect.Location.Y, topLeftRect.Width, newHeight);
                bottomLeftRect = new Rect(bottomLeftRect.Location.X, baseRect.Top + newHeight, bottomLeftRect.Width, Math.Max(0.0, baseRect.Height - newHeight));
            }

            var roundedRectGeometry = new StreamGeometry();

            using (var context = roundedRectGeometry.Open())
            {
                // Begin from the Bottom of the TopLeft Arc and proceed clockwise
                context.BeginFigure(topLeftRect.BottomLeft, true, true);
                // TopLeft Arc
                context.ArcTo(topLeftRect.TopRight, topLeftRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                // Top Line
                context.LineTo(topRightRect.TopLeft, true, true);
                // TopRight Arc
                context.ArcTo(topRightRect.BottomRight, topRightRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                // Right Line
                context.LineTo(bottomRightRect.TopRight, true, true);
                // BottomRight Arc
                context.ArcTo(bottomRightRect.BottomLeft, bottomRightRect.Size, 0, false, SweepDirection.Clockwise, true, true);
                // Bottom Line
                context.LineTo(bottomLeftRect.BottomRight, true, true);
                // BottomLeft Arc
                context.ArcTo(bottomLeftRect.TopLeft, bottomLeftRect.Size, 0, false, SweepDirection.Clockwise, true, true);

                context.Close();
            }

            return roundedRectGeometry;
        }

        #endregion Mask Property
    }
}