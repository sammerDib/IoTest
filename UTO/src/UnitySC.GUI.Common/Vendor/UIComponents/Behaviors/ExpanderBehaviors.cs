using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class ExpanderBehaviors
    {
        public static readonly DependencyProperty CollapseHeaderProperty = DependencyProperty.RegisterAttached(
            "CollapseHeader",
            typeof(bool),
            typeof(ExpanderBehaviors),
            new PropertyMetadata(default(bool)));

        public static void SetCollapseHeader(DependencyObject element, bool value)
        {
            element.SetValue(CollapseHeaderProperty, value);
        }

        public static bool GetCollapseHeader(DependencyObject element)
        {
            return (bool)element.GetValue(CollapseHeaderProperty);
        }

        public static readonly DependencyProperty CollapseContentProperty = DependencyProperty.RegisterAttached(
            "CollapseContent",
            typeof(bool),
            typeof(ExpanderBehaviors),
            new PropertyMetadata(default(bool)));

        public static void SetCollapseContent(DependencyObject element, bool value)
        {
            element.SetValue(CollapseContentProperty, value);
        }

        public static bool GetCollapseContent(DependencyObject element)
        {
            return (bool)element.GetValue(CollapseContentProperty);
        }
    }
}
