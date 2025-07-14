using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Agileo.SemiDefinitions;

namespace UnitySC.GUI.Common.Converters
{
    public class CassettePresenceToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CassettePresence cassettePresence)
            {
                if (cassettePresence == CassettePresence.Correctly)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
