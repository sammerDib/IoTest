using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Converter

{
    public class MultiplyIntegerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)values[0] * (int)values[1]).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
