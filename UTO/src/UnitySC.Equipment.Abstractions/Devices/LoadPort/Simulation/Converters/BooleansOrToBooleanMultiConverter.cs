using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    public class BooleansOrToBooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (values == null || values.Length == 0)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (value is bool)
                {
                    result = result || (bool)value;
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
