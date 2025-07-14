using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public class ImageGroupBox : GroupBox
    {
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageGroupBox), new PropertyMetadata());

        public static Geometry GetImageGeometry(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(ImageGeometryProperty);
        }

        public static void SetImageGeometry(DependencyObject obj, Geometry value)
        {
            obj.SetValue(ImageGeometryProperty, value);
        }

        // Using a DependencyProperty as the backing store for Geometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryProperty =
            DependencyProperty.RegisterAttached("ImageGeometry", typeof(Geometry), typeof(ImageGroupBox), new PropertyMetadata(null));

        public static Brush GetImageGeometryBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ImageGeometryBrushProperty);
        }

        public static void SetImageGeometryBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ImageGeometryBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for GeometryBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageGeometryBrushProperty =
            DependencyProperty.RegisterAttached("ImageGeometryBrush", typeof(Brush), typeof(ImageGroupBox), new PropertyMetadata(null));
    }
}