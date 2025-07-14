using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.Shared.Tools.Converter
{
    public class Int32Multiply : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double value_f64;
            if (value is Int32)
            {
                value_f64 = (Int32)value;
            }
            else
            {
                value_f64 = (double)value;
            }

            return (Int32)Math.Round(((double)parameter) * value_f64);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}