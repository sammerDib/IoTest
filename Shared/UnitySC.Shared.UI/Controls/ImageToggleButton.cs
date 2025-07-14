using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public class ImageToggleButton : ToggleButton
    {
        public Brush ForegroundCheckedBrush
        {
            get { return (Brush)GetValue(ForegroundCheckedBrushProperty); }
            set { SetValue(ForegroundCheckedBrushProperty, value); }
        }

        public static readonly DependencyProperty ForegroundCheckedBrushProperty =
            DependencyProperty.Register(nameof(ForegroundCheckedBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Brush ForegroundUncheckedBrush
        {
            get { return (Brush)GetValue(ForegroundUncheckedBrushProperty); }
            set { SetValue(ForegroundUncheckedBrushProperty, value); }
        }

        public static readonly DependencyProperty ForegroundUncheckedBrushProperty =
            DependencyProperty.Register(nameof(ForegroundUncheckedBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public ImageSource ImageRight
        {
            get { return (ImageSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        public static readonly DependencyProperty ImageRightProperty =
            DependencyProperty.Register(nameof(ImageRight), typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public Geometry ImageGeometry
        {
            get { return (Geometry)GetValue(ImageGeometryProperty); }
            set { SetValue(ImageGeometryProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryProperty =
            DependencyProperty.Register(nameof(ImageGeometry), typeof(Geometry), typeof(ImageToggleButton), new PropertyMetadata(ImageGeometryChanged));

        private static void ImageGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as ImageToggleButton)?.ImageCheckedGeometry is null)
                (d as ImageToggleButton).SetCurrentValue(ImageCheckedGeometryProperty, (d as ImageToggleButton)?.ImageGeometry);
        }

        public Geometry ImageCheckedGeometry
        {
            get { return (Geometry)GetValue(ImageCheckedGeometryProperty); }
            set { SetValue(ImageCheckedGeometryProperty, value); }
        }

        public static readonly DependencyProperty ImageCheckedGeometryProperty =
            DependencyProperty.Register(nameof(ImageCheckedGeometry), typeof(Geometry), typeof(ImageToggleButton), new PropertyMetadata(null));

        public Brush ImageGeometryBrush
        {
            get { return (Brush)GetValue(ImageGeometryBrushProperty); }
            set { SetValue(ImageGeometryBrushProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush ImageGeometryUncheckedBrush
        {
            get { return (Brush)GetValue(ImageGeometryUncheckedBrushProperty); }
            set { SetValue(ImageGeometryUncheckedBrushProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryUncheckedBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryUncheckedBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush ImageGeometryCheckedBrush
        {
            get { return (Brush)GetValue(ImageGeometryCheckedBrushProperty); }
            set { SetValue(ImageGeometryCheckedBrushProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryCheckedBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryCheckedBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Geometry ImageGeometryRight
        {
            get { return (Geometry)GetValue(ImageGeometryRightProperty); }
            set { SetValue(ImageGeometryRightProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryRightProperty =
            DependencyProperty.Register(nameof(ImageGeometryRight), typeof(Geometry), typeof(ImageToggleButton), new PropertyMetadata(null));

        public Brush ImageGeometryRightBrush
        {
            get { return (Brush)GetValue(ImageGeometryRightBrushProperty); }
            set { SetValue(ImageGeometryRightBrushProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryRightBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryRightBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush ImageGeometryRightCheckedBrush
        {
            get { return (Brush)GetValue(ImageGeometryRightCheckedBrushProperty); }
            set { SetValue(ImageGeometryRightCheckedBrushProperty, value); }
        }

        public static readonly DependencyProperty ImageGeometryRightCheckedBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryRightCheckedBrush), typeof(Brush), typeof(ImageToggleButton), new PropertyMetadata(new SolidColorBrush(Colors.Green)));
    }
}
