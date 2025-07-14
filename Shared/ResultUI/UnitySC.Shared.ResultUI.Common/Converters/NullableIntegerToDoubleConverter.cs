using System;
using System.Globalization;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class NullableIntegerToDoubleConverter : MarkupConvert
    {
        #region Implementation of IValueConverter

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int nullable)
            {
                return nullable;
            }

            return 0.0;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                if (double.IsNaN(doubleValue))
                {
                    return null;
                }

                return (int)doubleValue;
            }

            return null;
        }

        #endregion
    }
}
