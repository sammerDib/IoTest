using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    [ValueConversion(typeof(DependencyObject), typeof(bool))]
    public class IsLastItemInContainerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DependencyObject dependencyObject)
            {
                return false;
            }

            var itemControl = ItemsControl.ItemsControlFromItemContainer(dependencyObject);
            return itemControl.ItemContainerGenerator.IndexFromContainer(dependencyObject)
                   == itemControl.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
