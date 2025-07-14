using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    /// <summary>
    /// Useful to return the parameter word with uppercase first letter
    /// </summary>
    public sealed class FirstLetterUppercaseConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string)) return string.Empty;

            var word = (string) value;
            return char.ToUpper(word[0]) + word.Substring(1);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
