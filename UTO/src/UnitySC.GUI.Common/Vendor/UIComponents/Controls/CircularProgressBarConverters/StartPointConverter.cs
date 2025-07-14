using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.CircularProgressBarConverters
{
    public class StartPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue and > 0.0)
            {
                return new Point(doubleValue / 2, 0);
            }

            return new Point();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
