using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Converter
{
    public class NotEqualEnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() != parameter.ToString() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
