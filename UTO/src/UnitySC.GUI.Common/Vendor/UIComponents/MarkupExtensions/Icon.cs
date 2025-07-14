using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    /// <summary>
    /// Icon DependecyObject allows to add Geometry DependencyProperty on any object.
    /// This additional Geometry DependencyProperty is usually used to add an icon on any existing WPF object.
    /// </summary>
    public class Icon : DependencyObject
    {
        #region GeometryProperty

        /// <summary>
        /// Geometry is useful to add <see cref="DependencyProperty" /> of type <see cref="Geometry" />
        /// Note : Object of type <see cref="Geometry" /> is usually used
        /// </summary>
        public static readonly DependencyProperty GeometryProperty
            = DependencyProperty.RegisterAttached(
                nameof(Geometry),
                typeof(Geometry),
                typeof(Icon));

        /// <summary>
        /// Gets the geometry.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <returns>Geometry</returns>
        public static Geometry GetGeometry(DependencyObject d)
        {
            return (Geometry) d.GetValue(GeometryProperty);
        }

        /// <summary>
        /// Sets the geometry.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="value">Geometry</param>
        public static void SetGeometry(DependencyObject d, Geometry value)
        {
            d.SetValue(GeometryProperty, value);
        }

        #endregion GeometryProperty

        #region MarginProperty

        /// <summary>
        /// Margin is useful to add <see cref="DependencyProperty" /> of type <see cref="FrameworkElement.Margin" /> to
        /// Geometry object
        /// </summary>
        public static readonly DependencyProperty MarginProperty
            = DependencyProperty.RegisterAttached(
                nameof(FrameworkElement.Margin),
                typeof(Thickness),
                typeof(Icon),
                new PropertyMetadata(new Thickness(5)));

        /// <summary>
        /// Gets the margin.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <returns>Thickness</returns>
        public static Thickness GetMargin(DependencyObject d)
        {
            return (Thickness) d.GetValue(MarginProperty);
        }

        /// <summary>
        /// Sets the margin.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="value">Thickness</param>
        public static void SetMargin(DependencyObject d, Thickness value)
        {
            d.SetValue(MarginProperty, value);
        }

        #endregion MarginProperty

        #region TransformProperty

        /// <summary>
        /// Transform is useful to add <see cref="DependencyProperty" /> of type <see cref="Transform" /> to Geometry object
        /// </summary>
        public static readonly DependencyProperty TransformProperty
            = DependencyProperty.RegisterAttached(
                nameof(Transform),
                typeof(Transform),
                typeof(Icon));

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <returns>Transform</returns>
        public static Transform GetTransform(DependencyObject d)
        {
            return (Transform) d.GetValue(TransformProperty);
        }

        /// <summary>
        /// Sets the transform.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="value">Transform</param>
        public static void SetTransform(DependencyObject d, Transform value)
        {
            d.SetValue(TransformProperty, value);
        }

        #endregion TransformProperty
    }
}
