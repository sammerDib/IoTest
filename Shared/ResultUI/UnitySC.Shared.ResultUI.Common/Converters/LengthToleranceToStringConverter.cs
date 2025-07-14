using System;
using System.Globalization;

using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class LengthToleranceToStringConverter : MarkupConvert
    {
        public int Digits { get; set; }

        public bool ShowUnit { get; set; } = true;

        public string NullValue { get; set; } = string.Empty;

        public LengthToleranceUnit Unit { get; set; } = LengthToleranceUnit.Nanometer;

        #region Overrides of MarkupConvert

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ConvertToString(value as LengthTolerance, Digits, ShowUnit, NullValue, Unit);

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static string ConvertToString(LengthTolerance tolerance, int digits, bool showUnit, string nullValue, LengthToleranceUnit unit)
        {
            if (tolerance == null) return nullValue;

            switch (tolerance.Unit)
            {
                case LengthToleranceUnit.Percentage:
                    string format = StringFormatHelper.GetFormat(digits);
                    string percentageString = string.Format(format, tolerance.Value);
                    return showUnit ? $"{percentageString} %" : $"{percentageString}";
                default:
                    var length = new Length(tolerance.Value, tolerance.Unit.ToLengthUnit());
                    return LengthToStringConverter.ConvertToString(length, digits, showUnit, nullValue, unit.ToLengthUnit());
            }
        }

        public static string ConvertToString(LengthTolerance tolerance, int digits, bool showUnit, string nullValue, LengthUnit unit)
        {
            if (tolerance == null) return nullValue;

            switch (tolerance.Unit)
            {
                case LengthToleranceUnit.Percentage:
                    string format = StringFormatHelper.GetFormat(digits);
                    string percentageString = string.Format(format, tolerance.Value * 100);
                    return showUnit ? $"{percentageString} %" : $"{percentageString}";
                default:
                    var length = new Length(tolerance.Value, tolerance.Unit.ToLengthUnit());
                    return LengthToStringConverter.ConvertToString(length, digits, showUnit, nullValue, unit);
            }
        }
    }
}
