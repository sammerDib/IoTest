using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class QuantityUnitToAbbreviationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IQuantity quantity)) return null;

            var quantityUnit = quantity.QuantityInfo.UnitType;
            var defaultUnitIndex = quantity.QuantityInfo.UnitInfos.Select(x => x.Name).ToList().IndexOf(quantity.Unit.ToString()) + 1;
            var selectedAbbreviation = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(quantityUnit, defaultUnitIndex);
            return selectedAbbreviation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
