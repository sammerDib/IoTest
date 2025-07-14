using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class GreaterThanDoubleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Notifier.IsInDesignModeStatic) return Visibility.Visible;

            var valueAsConvertible = value as IConvertible;
            var referenceValue = valueAsConvertible?.ToDouble(null) ?? 0;

            var parameterAsConvertible = parameter as IConvertible;
            var parameterValue = parameterAsConvertible?.ToDouble(null) ?? 0;

            return referenceValue > parameterValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
