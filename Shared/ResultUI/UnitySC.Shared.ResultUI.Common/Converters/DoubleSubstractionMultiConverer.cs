using System;
using System.Globalization;
using System.Linq;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class DoubleSubstractionMultiConverer : MarkupMultiConvert
    {
        #region Implementation of IMultiValueConverter

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var doubles = values.OfType<double>().ToList();
            if (doubles.Count == 2)
            {
                double a = doubles.ElementAt(0);
                double b = doubles.ElementAt(1);
                return a - b;
            }

            return 0;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
