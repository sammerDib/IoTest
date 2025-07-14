using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class FalseToCollapsedConverter : IValueConverter
    {
        /// <summary>Converts a Boolean value to a <see cref="T:System.Windows.Visibility" /> enumeration value.</summary>
        /// <returns>
        /// <see cref="F:System.Windows.Visibility.Visible" /> if <paramref name="value" /> is true; otherwise, <see cref="F:System.Windows.Visibility.Collapsed" />.</returns>
        /// <param name="value">The Boolean value to convert. This value can be a standard Boolean value or a nullable Boolean value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Notifier.IsInDesignModeStatic) return Visibility.Visible;

            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>Converts a <see cref="T:System.Windows.Visibility" /> enumeration value to a Boolean value.</summary>
        /// <returns>true if <paramref name="value" /> is <see cref="F:System.Windows.Visibility.Visible" />; otherwise, false.</returns>
        /// <param name="value">A <see cref="T:System.Windows.Visibility" /> enumeration value. </param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Visibility? == Visibility.Visible;
        }
    }
}
