using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.Tools.Units
{
    public class UnitToStringConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convert Unit to string
        /// </summary>
        /// <param name="value">Angle or Length</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">AngleUnit or LengthUnit</param>
        /// <param name="culture"></param>
        /// <returns>String</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res;
            switch (value)
            {
                case Angle angle:
                    var angleUnit = parameter as AngleUnit?;
                    if (!angleUnit.HasValue)
                        throw new InvalidCastException("Bad Unit Type for Angle");
                    res = angle.ToUnit(angleUnit.Value).ToString("F3");
                    break;

                case Length length:
                    var lengthUnit = parameter as LengthUnit?;
                    if (!lengthUnit.HasValue)
                        throw new InvalidCastException("Bad Unit Type for Length");
                    res = length.ToUnit(lengthUnit.Value).ToString("F3");
                    break;

                default:
                    res = "NaN";
                    break;
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
