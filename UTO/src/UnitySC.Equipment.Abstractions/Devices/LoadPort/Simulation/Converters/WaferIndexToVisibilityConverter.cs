using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    public class WaferIndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int)value + 1;
            var param = (string)parameter;
            if (!string.IsNullOrEmpty(param))
            {
                if ((number % 5) == 0 || number == 1)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }

            return number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
