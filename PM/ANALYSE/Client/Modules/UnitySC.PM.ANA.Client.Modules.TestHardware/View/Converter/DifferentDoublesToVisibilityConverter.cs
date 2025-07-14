using System;
using System.Globalization;
using System.Windows.Data;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Converter
{
    public class DifferentDoublesToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool finalResult1 = double.TryParse(values[0].ToString(), out double result1);
            bool finalResult2 = double.TryParse(values[1].ToString(), out double result2); 
            if (!finalResult1 || !finalResult2)
            {
                return System.Windows.Visibility.Collapsed;
            }
            return !result1.Near(result2, 0.001) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
