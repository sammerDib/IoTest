using System;
using System.Globalization;
using System.Windows.Data;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class TextToHumanizedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString())) return string.Empty;
            return value.ToString().Humanize();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
