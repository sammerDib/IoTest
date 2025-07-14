using System;
using System.Globalization;

using UnitySC.Shared.ResultUI.Common.Helpers;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class DoubleToPercentConverter : MarkupConvert
    {
        public int Digits { get; set; }

        public bool ShowPercentSymbol { get; set; } = true;

        public string NullValue { get; set; } = string.Empty;

        #region Overrides of MarkupConvert

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = StringFormatHelper.GetFormat(Digits);

            if (!(value is double doubleValue) || double.IsNaN(doubleValue)) return NullValue;

            doubleValue *= 100;
            
            return ShowPercentSymbol ?
                $"{string.Format(format, doubleValue)} %" :
                $"{string.Format(format, doubleValue)}";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
