using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class QuantityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return LocalizationHelper.ObjectToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
