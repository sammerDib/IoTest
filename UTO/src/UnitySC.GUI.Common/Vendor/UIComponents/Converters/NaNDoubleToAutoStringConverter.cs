using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class NaNDoubleToAutoStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return double.IsNaN(doubleValue) ? "Auto" : doubleValue.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var output) ? output : double.NaN;
            }

            return double.NaN;
        }
    }
}
