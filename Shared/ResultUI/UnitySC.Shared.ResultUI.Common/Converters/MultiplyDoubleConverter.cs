using System;
using System.Globalization;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class MultiplyDoubleConverter : MarkupConvert
    {
        public double Factor { get; set; } = 1.0;

        #region Overrides of MarkupConvert

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue * Factor;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
