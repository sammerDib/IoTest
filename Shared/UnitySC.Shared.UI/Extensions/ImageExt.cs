using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Extensions
{

    // Usage example :  <Image sharedExtensions:ImageExt.SourceGeometry="{StaticResource CloudToolsGeometry}" sharedExtensions:ImageExt.GeometryBrush="{StaticResource IconsColor}" Height="250" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="50" />

    public class ImageExt
    {
        public static Geometry GetSourceGeometry(DependencyObject obj)
        {
            return (Geometry)obj.GetValue(SourceGeometryProperty);
        }

        public static void SetSourceGeometry(DependencyObject obj, Geometry value)
        {
            obj.SetValue(SourceGeometryProperty, value);
        }

        // Using a DependencyProperty as the backing store for Geometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceGeometryProperty =
            DependencyProperty.RegisterAttached("SourceGeometry", typeof(Geometry), typeof(ImageExt), new PropertyMetadata(null, OnSourceGeometryChanged));

        private static void OnSourceGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Image))
                return;

            if (!(e.NewValue is Geometry))
                return;
            var image = d as Image;
            var geometry = (Geometry)e.NewValue;
            var brush = GetGeometryBrush(image);

            if (brush is null)
                brush = new SolidColorBrush(Colors.Black);

            SetImageSourceFromGeometry(image, geometry, brush);
        }

        private static void SetImageSourceFromGeometry(Image image, Geometry geometry, Brush brush)
        {
            var drawingImage = new DrawingImage();

            var geometryDrawings = new DrawingGroup();

            using (var dc = geometryDrawings.Open())
            {
                dc.DrawGeometry(brush, null, geometry);
            }

            drawingImage.Drawing = geometryDrawings;

            image.Source = drawingImage;
        }

        public static Brush GetGeometryBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(GeometryBrushProperty);
        }

        public static void SetGeometryBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(GeometryBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for GeometryBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryBrushProperty =
            DependencyProperty.RegisterAttached("GeometryBrush", typeof(Brush), typeof(ImageExt), new PropertyMetadata(null, OnGeometryBrushChanged));

        private static void OnGeometryBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Image))
                return;

            if (!(e.NewValue is Brush))
                return;

            var image = d as Image;
            var geometry = GetSourceGeometry(image);
            if (geometry is null)
                return;

            var brush = e.NewValue as Brush;

            if (brush is null)
                brush = new SolidColorBrush(Colors.Black);

            SetImageSourceFromGeometry(image, geometry, brush);
        }
    }
}
