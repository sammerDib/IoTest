using System;
using System.Globalization;
using System.Windows.Data;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class UnitToAbbreviationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "---";

            var unit = value;
            var unitAb = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(unit.GetType(), (int)unit);

            return unitAb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new InvalidOperationException("The target must be a boolean");
            return !(bool)value;
        }
    }
}
