using System;
using System.Globalization;
using System.Windows.Data;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class TextToHumanizedAndUpperCaseTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString())) return "---";
            return value.ToString().Humanize().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
