using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.Tools.Units
{
    /// <summary>
    /// Convert Unit to double
    /// </summary>
    /// <param name="value">Angle or Length</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">AngleUnit or LengthUnit</param>
    /// <param name="culture"></param>
    /// <returns>Double</returns>
    public class UnitToDoubleConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double res = double.NaN;
            switch (value)
            {
                case Angle angle:
                    var angleUnit = parameter as AngleUnit?;
                    if (!angleUnit.HasValue)
                        throw new InvalidCastException("Bad Unit Type for Angle");
                    res = angle.ToUnit(angleUnit.Value).Value;
                    break;

                case Length length:
                    var lengthUnit = parameter as LengthUnit?;
                    if (!lengthUnit.HasValue)
                        throw new InvalidCastException("Bad Unit Type for Length");
                    res = length.ToUnit(lengthUnit.Value).Value;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Convert Double to Unit
        /// </summary>
        /// <param name="value">Double</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">AngleUnit or LengthUnit</param>
        /// <param name="culture"></param>
        /// <returns>Angle or Length</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (!double.TryParse((string)value, out double doubleValue))
                throw new InvalidCastException("Value can't be parse to double in UnitConverter");

            switch (parameter)
            {
                case AngleUnit angleUnit:
                    res = new Angle(doubleValue, angleUnit);
                    break;

                case LengthUnit lengthUnit:
                    res = new Length(doubleValue, lengthUnit);
                    break;
            }

            return res;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
