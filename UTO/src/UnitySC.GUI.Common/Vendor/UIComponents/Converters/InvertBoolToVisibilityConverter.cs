using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    /// <summary>
    /// Allows to return Visible when boolean input value is false
    /// Collapsed when boolean input value is true
    /// </summary>
    public class TrueToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notVisible = parameter as Visibility? ?? Visibility.Collapsed;

            if (value is bool) return (bool) value ? notVisible : Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
