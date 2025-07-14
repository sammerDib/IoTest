using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.Semi.Gem300.Abstractions.E90;

namespace UnitySC.UTO.Controller.UIComponents.Converters
{
    public class LocationStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is LocationState state)
                {
                    return state == LocationState.Occupied;
                }
                return new ArgumentException(
                    "LocationStateToBoolConverter given value is not type of LocationState.");
            }
            return new ArgumentNullException(nameof(value),
                @"LocationStateToBoolConverter given value is null.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
