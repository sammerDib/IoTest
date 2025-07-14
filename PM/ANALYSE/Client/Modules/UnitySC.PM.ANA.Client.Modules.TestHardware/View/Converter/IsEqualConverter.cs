using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Converter
{
    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case int i:
                    return i == System.Convert.ToInt32(parameter);
                case double d:
                    return d == System.Convert.ToDouble(parameter);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                    return 0;

            return 1;
        }
    }
}
