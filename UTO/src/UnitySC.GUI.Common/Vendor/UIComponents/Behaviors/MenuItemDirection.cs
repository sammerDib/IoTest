using System.Windows;
using System.Windows.Controls.Primitives;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    /// <summary>
    /// Determines on which direction the menu will be deployed.
    /// </summary>
    public static class MenuItemDirection
    {
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.RegisterAttached(
            "Direction", typeof(PlacementMode), typeof(MenuItemDirection), new PropertyMetadata(PlacementMode.Right));

        public static void SetDirection(DependencyObject element, PlacementMode value)
        {
            element.SetValue(DirectionProperty, value);
        }

        public static PlacementMode GetDirection(DependencyObject element)
        {
            return (PlacementMode)element.GetValue(DirectionProperty);
        }
    }
}
