using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class LevelToLeftMarginConverter : IValueConverter
    {
        public double LeftMarginByLevel { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int defaultLevel = 0;
            if (!(value is int)) return new Thickness(0);
            if (parameter != null) defaultLevel = int.Parse(parameter.ToString());
            var level = (int) value > 0
                ? (int) value - defaultLevel
                : 0; //skip root menu (level 0) and first menu (level 1)
            return new Thickness(level == 0 ? 10 : LeftMarginByLevel * level, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
