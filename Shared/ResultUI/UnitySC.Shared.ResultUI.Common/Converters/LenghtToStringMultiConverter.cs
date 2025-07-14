using System;
using System.Globalization;
using System.Windows;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class LenghtToStringMultiConverter : MarkupMultiConvert
    {
        public bool ShowUnit { get; set; } = true;

        public string NullValue { get; set; } = string.Empty;

        public LengthUnit Unit { get; set; } = LengthUnit.Micrometer;

        #region Overrides of MarkupMultiConvert

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                object value = values[0];
                int digits = values[1] as int? ?? 0;
                return LengthToStringConverter.ConvertToString(value, digits, ShowUnit, NullValue, Unit);
            }

            return DependencyProperty.UnsetValue;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
