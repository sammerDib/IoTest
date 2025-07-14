using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation.Converters
{
    [ValueConversion(typeof(float), typeof(string))]
    public class FloatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is float)
            {
                return ((float)value).ToString("F3", CultureInfo.InvariantCulture);
            }
            return ("NaN");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is string))
                    return float.NaN;
                float val;
                if (float.TryParse((string)value, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                {
                    return val;
                }
                return float.NaN;
            }
            catch (Exception)
            {
                return float.NaN;
            }
        }
    }
}
