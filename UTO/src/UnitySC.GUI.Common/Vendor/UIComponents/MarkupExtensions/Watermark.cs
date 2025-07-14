using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.MarkupExtensions
{
    /// <inheritdoc />
    /// <summary>
    /// Watermark DependencyObject allows to add Watermark DependencyProperty on any object.
    /// This additional Text DependencyProperty is usually used to add an Watermark on any existing WPF object.
    /// </summary>
    public class Watermark : DependencyObject
    {
        /// <summary>
        /// Text is useful to add <see cref="DependencyProperty" /> of type <see cref="string" />
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Watermark));

        /// <summary>
        /// Gets the watermark text.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <returns>String value</returns>
        public static string GetText(DependencyObject d)
        {
            return (string) d.GetValue(TextProperty);
        }

        /// <summary>
        /// Sets the watermark text.
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="value">String value</param>
        public static void SetText(DependencyObject d, string value)
        {
            d.SetValue(TextProperty, value);
        }
    }
}
