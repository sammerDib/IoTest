using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.PM.ANA.Client.CommonUI.Converters
{
    public class MathInvertConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse(value.ToString(), out double valueToInvert))
                return DependencyProperty.UnsetValue;

            if (valueToInvert == 0.0)
                return DependencyProperty.UnsetValue;
            return 1 / valueToInvert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse(value.ToString(), out double valueToInvert))
                return DependencyProperty.UnsetValue;

            if (valueToInvert == 0.0)
                return DependencyProperty.UnsetValue;
            return 1 / valueToInvert;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
