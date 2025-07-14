using System.Windows;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models
{
    public class ColorRoutedEventArgs : RoutedEventArgs
    {
        public ColorRoutedEventArgs(RoutedEvent routedEvent, Color color) : base(routedEvent)
        {
            Color = color;
        }

        public Color Color { get; }
    }
}
