using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Converter
{
    public class ProgressRateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)values[0] == 0 || (int)values[1] == 0)
            {
                return 0.0;
            }
            
            return ((double)(int)values[2] / ((int)(values[0]) * (int)(values[1])) * 100.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
