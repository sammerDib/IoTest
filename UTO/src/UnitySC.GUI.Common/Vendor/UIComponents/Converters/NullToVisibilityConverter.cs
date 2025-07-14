using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public Visibility NullVisibility { get; set; } = Visibility.Visible;

        public Visibility NotNullVisibility { get; set; } = Visibility.Collapsed;

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null
            ? NullVisibility
            : NotNullVisibility;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        #endregion
    }
}
