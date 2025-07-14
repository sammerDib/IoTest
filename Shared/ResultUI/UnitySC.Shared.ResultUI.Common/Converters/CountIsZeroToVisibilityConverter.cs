using System;
using System.Globalization;
using System.Windows;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class CountIsZeroToVisibilityConverter : MarkupConvert
    {
        #region Overrides of MarkupConvert

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
