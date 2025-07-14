using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.GUI.Common.UIComponents.Converters
{
    public class UnityIntModuloToVisibilityConverter : IValueConverter
    {
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        public int Modulo { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                if (intValue == 1)
                {
                    return Visibility.Visible;
                }
                return intValue % Modulo == 0 ? Visibility.Visible : FalseVisibility;
            }

            return FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
