using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class ListViewItemToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ListViewItem item))
            {
                return string.Empty;
            }

            if (!(ItemsControl.ItemsControlFromItemContainer(item) is ListView listView))
            {
                return string.Empty;
            }

            var index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return index < 0 ? string.Empty : (index + 1).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert from index to ListViewItem");
        }
    }
}
