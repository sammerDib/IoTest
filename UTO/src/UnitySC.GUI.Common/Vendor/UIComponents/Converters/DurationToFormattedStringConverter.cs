using System;
using System.Globalization;
using System.Windows.Data;

using UnitsNet;
using UnitsNet.Units;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class DurationToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Duration duration)) return string.Empty;
            if (double.IsNaN(duration.Value)) return "NaN";

            var dr = duration.ToTimeSpan();
            return dr.Hours >= 24 ? "--:--:--" : $"{dr.Hours:00}:{dr.Minutes:00}:{dr.Seconds:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Duration.Zero;
            if (TimeSpan.TryParseExact((string)value, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out var t))
            {
                result = Duration.From(t.TotalMilliseconds, DurationUnit.Millisecond);
            }
            return result;
        }
    }
}
