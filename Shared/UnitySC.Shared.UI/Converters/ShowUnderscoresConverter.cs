using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.UI.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ShowUnderscoreConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is string text ? text.Replace("_", "__") : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is string text ? text.Replace("__", "_") : null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
