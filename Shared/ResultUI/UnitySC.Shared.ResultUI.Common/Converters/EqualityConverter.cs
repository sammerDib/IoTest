using System;
using System.Globalization;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class EqualityConverter : MarkupConvert
    {
        #region Implementation of IValueConverter

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && parameter == null) return true;
            return Equals(value, parameter);
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                {
                    return parameter;
                }
            }

            return null;
        }

        #endregion
    }
}
