using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;


namespace UnitySC.Shared.ResultUI.Metro.Converters
{
    public class MultiValuesToVisibilityConverter : IMultiValueConverter
    {
        public Visibility VisibilityEqual { get; set; } = Visibility.Visible;
        public Visibility VisibilityNonEqual { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityNull { get; set; } = Visibility.Collapsed;
        public string VisibilityLimit { get; set; } = "Range";


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = VisibilityNonEqual;

            if (values.Count() != 2) return result;

            if (values[0] is double && values[1] is double)
            {
                if (VisibilityLimit == "Range")
                {
                    if ((double)values[0] >= 0 && (double)values[0] <= (double)values[1])
                    {
                        result = VisibilityEqual;
                    }
                }
                else
                {
                    if ((double)values[0] > (double)values[1])
                    {
                        result = VisibilityEqual;
                    }
                }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
