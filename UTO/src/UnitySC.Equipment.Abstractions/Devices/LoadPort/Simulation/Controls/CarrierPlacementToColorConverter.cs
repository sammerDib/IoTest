using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Controls
{
    public class CarrierPlacementToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value) ? Colors.LimeGreen : Colors.Orange;
            }

            return Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                return ((Color)value) == Colors.LimeGreen;
            }

            return false;
        }
    }
}
