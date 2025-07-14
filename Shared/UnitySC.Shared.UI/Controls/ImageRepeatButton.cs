using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public class ImageRepeatButton : RepeatButton
    {
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ImageRepeatButton), new PropertyMetadata(null));

        public ImageSource ImageRight
        {
            get { return (ImageSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageRightProperty =
            DependencyProperty.Register(nameof(ImageRight), typeof(ImageSource), typeof(ImageRepeatButton), new PropertyMetadata(null));

        public Geometry ImageGeometry
        {
            get { return (Geometry)GetValue(ImageGeometryProperty); }
            set { SetValue(ImageGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryProperty =
            DependencyProperty.Register(nameof(ImageGeometry), typeof(Geometry), typeof(ImageRepeatButton), new PropertyMetadata(null));

        public Brush ImageGeometryBrush
        {
            get { return (Brush)GetValue(ImageGeometryBrushProperty); }
            set { SetValue(ImageGeometryBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometryBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryBrush), typeof(Brush), typeof(ImageRepeatButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Geometry ImageGeometryRight
        {
            get { return (Geometry)GetValue(ImageGeometryRightProperty); }
            set { SetValue(ImageGeometryRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometryRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryRightProperty =
            DependencyProperty.Register(nameof(ImageGeometryRight), typeof(Geometry), typeof(ImageRepeatButton), new PropertyMetadata(null));

        public Brush ImageGeometryRightBrush
        {
            get { return (Brush)GetValue(ImageGeometryRightBrushProperty); }
            set { SetValue(ImageGeometryRightBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageGeometryBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryRightBrushProperty =
            DependencyProperty.Register(nameof(ImageGeometryRightBrush), typeof(Brush), typeof(ImageRepeatButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
    }
}