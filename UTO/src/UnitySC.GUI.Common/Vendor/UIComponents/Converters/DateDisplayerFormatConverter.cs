using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    /// <summary>
    /// Help us format the content of a header button in a calendar.
    /// </summary>
    /// <remarks>
    /// Expected items, in the following order:
    ///     1) DateTime Calendar.DisplayDate
    ///     2) DateTime? Calendar.SelectedDate
    /// </remarks>
    public class DateDisplayerFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) throw new ArgumentException("Unexpected", nameof(values));
            if (!(values[0] is DateTime)) throw new ArgumentException("Unexpected", nameof(values));
            if (values[1] != null && !(values[1] is DateTime?)) throw new ArgumentException("Unexpected", nameof(values));

            var selectedDate = (DateTime?)values[1];

            return selectedDate ?? values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
