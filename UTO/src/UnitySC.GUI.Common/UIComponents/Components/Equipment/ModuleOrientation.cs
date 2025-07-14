using System.Windows;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment
{
    public enum ModuleOrientationValue
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public static class ModuleOrientation
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached(
            "Orientation",
            typeof(ModuleOrientationValue),
            typeof(ModuleOrientation),
            new FrameworkPropertyMetadata(default(ModuleOrientationValue), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetOrientation(DependencyObject element, ModuleOrientationValue value)
        {
            element.SetValue(OrientationProperty, value);
        }

        public static ModuleOrientationValue GetOrientation(DependencyObject element)
        {
            return (ModuleOrientationValue)element.GetValue(OrientationProperty);
        }
    }
}
