using System;
using System.Globalization;

using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.Tools.Tolerances;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class ToleranceToStringConverter : MarkupConvert
    {
        public int Digits { get; set; }

        public bool ShowUnit { get; set; } = true;

        public string NullValue { get; set; } = string.Empty;

        #region Overrides of MarkupConvert

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ConvertToString(value as Tolerance, Digits, ShowUnit, NullValue);

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static string ConvertToString(Tolerance tolerance, int digits, bool showUnit, string nullValue)
        {
            if (tolerance == null) return nullValue;

            string format = StringFormatHelper.GetFormat(digits);

            switch (tolerance.Unit)
            {
                case ToleranceUnit.Percentage:
                    string percentageString = string.Format(format, tolerance.Value * 100);
                    return showUnit ? $"{percentageString} %" : $"{percentageString}";
                case ToleranceUnit.AbsoluteValue:
                    return string.Format(format, tolerance.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
