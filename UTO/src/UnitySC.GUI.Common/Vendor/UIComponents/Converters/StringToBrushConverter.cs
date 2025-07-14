using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Allows to return SolidColorbrush object from string hewadecimal color name
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var solidColorBrush = new SolidColorBrush();
            if (value != null) solidColorBrush.Color = (Color) ColorConverter.ConvertFromString(value.ToString());
            return solidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
