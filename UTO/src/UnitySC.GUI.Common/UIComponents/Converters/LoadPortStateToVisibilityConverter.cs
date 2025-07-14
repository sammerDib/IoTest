using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Agileo.SemiDefinitions;

namespace UnitySC.GUI.Common.UIComponents.Converters
{
    public class LoadPortStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LoadPortState physicalState)
            {
                if (physicalState is LoadPortState.Open or LoadPortState.Closed or LoadPortState.Docked or LoadPortState.Undocked)
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
