using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class ColorToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }

            return Colors.Magenta;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
