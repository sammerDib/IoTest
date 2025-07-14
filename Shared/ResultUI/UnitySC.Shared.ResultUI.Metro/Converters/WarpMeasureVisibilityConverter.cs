using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Tools.Units;


namespace UnitySC.Shared.ResultUI.Metro.Converters
{
    public class WarpMeasureVisibilityConverter : IMultiValueConverter
    {
        public Visibility VisibilityEqual { get; set; } = Visibility.Visible;
        public Visibility VisibilityNonEqual { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityNull { get; set; } = Visibility.Collapsed;


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = VisibilityNonEqual;

            if (values.Count() != 2) return result;

            if (values[0] is bool && values[1] is Length)
            {
                if ((bool)values[0] && (Length)values[1] != null)
                    result = VisibilityEqual;

            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
