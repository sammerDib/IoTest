using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            int val = System.Convert.ToInt32(value);
            return (val.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is string))
                    return 0;
                int val = int.Parse((string)value);
                return val;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
