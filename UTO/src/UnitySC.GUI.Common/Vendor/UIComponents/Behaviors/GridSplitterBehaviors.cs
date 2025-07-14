using System.Windows;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class GridSplitterBehaviors
    {
        public static readonly DependencyProperty GripThicknessProperty = DependencyProperty.RegisterAttached(
            "GripThickness",
            typeof(Thickness),
            typeof(GridSplitterBehaviors),
            new PropertyMetadata(default(Thickness)));

        public static void SetGripThickness(DependencyObject element, Thickness value)
        {
            element.SetValue(GripThicknessProperty, value);
        }

        public static Thickness GetGripThickness(DependencyObject element)
        {
            return (Thickness)element.GetValue(GripThicknessProperty);
        }
    }
}
