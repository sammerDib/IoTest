using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    /// <summary>
    /// Returns <see langword="true"/> when the string representation of <paramref name="value"/> matches the string representation of <paramref name="parameter"/>.
    /// Returns <see langword="false"/> in any other cases.
    /// </summary>
    /// <remarks>Strings are compared using <see cref="System.StringComparison.Ordinal"/></remarks>
    public class StringToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) { return false; }

            return string.Equals(value.ToString(), parameter.ToString(), StringComparison.Ordinal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
