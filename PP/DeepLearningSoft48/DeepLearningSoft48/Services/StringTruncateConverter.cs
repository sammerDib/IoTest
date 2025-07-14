using System;
using System.Globalization;
using System.Windows.Data;

namespace DeepLearningSoft48.Services
{
    public class StringTruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string lengthString && int.TryParse(lengthString, out int maxLength))
            {
                if (stringValue.Length > maxLength)
                {
                    return "..." + stringValue.Substring(stringValue.Length - maxLength);
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
