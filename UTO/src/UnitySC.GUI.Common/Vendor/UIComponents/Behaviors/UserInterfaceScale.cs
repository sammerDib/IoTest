using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class UserInterfaceScale
    {
        public static readonly DependencyProperty FontScaleProperty = DependencyProperty.RegisterAttached(
            "FontScale", typeof(Transform), typeof(UserInterfaceScale), new PropertyMetadata(default(Transform)));

        public static void SetFontScale(DependencyObject element, Transform value)
        {
            element.SetValue(FontScaleProperty, value);
        }

        public static Transform GetFontScale(DependencyObject element)
        {
            return (Transform)element.GetValue(FontScaleProperty);
        }
    }
}
