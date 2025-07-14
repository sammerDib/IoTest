using System;
using System.Globalization;

using UnitySC.Shared.ResultUI.Common.Helpers;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class LengthToStringConverter : MarkupConvert
    {
        public int Digits { get; set; }

        public bool ShowUnit { get; set; } = true;

        public string NullValue { get; set; } = string.Empty;

        public LengthUnit Unit { get; set; } = LengthUnit.Micrometer;

        #region Implementation of IValueConverter

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ConvertToString(value, Digits, ShowUnit, NullValue, Unit);

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        public static string ConvertToString(object value, int digits, bool showUnit = false, string nullValue = "-", LengthUnit unit = LengthUnit.Undefined)
        {
            string format = StringFormatHelper.GetFormat(digits);

            if (value is string stringValue) return stringValue;

            if (value is IConvertible doubleValue)
            {
                return showUnit ?
                    $"{string.Format(format, doubleValue)} {Length.GetUnitSymbol(unit)}" :
                    $"{string.Format(format, doubleValue)}";
            }

            if (!(value is Length length)) return nullValue;
            
            switch (unit)
            {
                case LengthUnit.Undefined:
                    return $"{string.Format(format, length.Value)}";
                case LengthUnit.Centimeter:
                    doubleValue = length.Centimeters;
                    break;
                case LengthUnit.Meter:
                    doubleValue = length.Meters;
                    break;
                case LengthUnit.Millimeter:
                    doubleValue = length.Millimeters;
                    break;
                case LengthUnit.Micrometer:
                    doubleValue = length.Micrometers;
                    break;
                case LengthUnit.Nanometer:
                    doubleValue = length.Nanometers;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }

            return showUnit ?
                $"{string.Format(format, doubleValue)} {Length.GetUnitSymbol(unit)}" :
                $"{string.Format(format, doubleValue)}";
        }


        /// <summary>
        /// Convert value (<see cref="string"/>, <see cref="IConvertible"/> or <see cref="Length"/>) to a formatted string.
        /// </summary>
        public static string ConvertToString(object value, int digits, string unitSymbol, string nullValue = "-")
        {
            string format = StringFormatHelper.GetFormat(digits);

            if (value is string stringValue) return stringValue;

            if (value is IConvertible doubleValue)
            {
                return string.IsNullOrWhiteSpace(unitSymbol) ? $"{string.Format(format, doubleValue)}" : $"{string.Format(format, doubleValue)} {unitSymbol}";
            }

            if (!(value is Length length)) return nullValue;
            
            return string.IsNullOrWhiteSpace(unitSymbol) ? $"{string.Format(format, length.Value)}" : $"{string.Format(format, length.Value)} {unitSymbol}";
        }

        #endregion
    }
}
