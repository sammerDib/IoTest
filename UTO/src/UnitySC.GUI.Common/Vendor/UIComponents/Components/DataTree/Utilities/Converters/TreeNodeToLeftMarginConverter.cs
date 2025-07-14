using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Interfaces;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Utilities.Converters
{
    public class TreeNodeToLeftMarginConverter : IValueConverter
    {
        public double Length { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ITreeNode item)
            {
                return new Thickness(Length * item.Level, 0, 0, 0);
            }

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
