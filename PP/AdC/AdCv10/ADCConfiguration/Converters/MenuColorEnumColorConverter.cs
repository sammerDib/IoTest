using System;
using System.Windows.Data;

using ADCConfiguration.ViewModel;

namespace ADCConfiguration.Converters
{
    public class MenuColorEnumColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MenuColorEnum menuColorEnum = (MenuColorEnum)value;

            switch (menuColorEnum)
            {
                case MenuColorEnum.Blue:
                    return System.Windows.Media.Colors.CornflowerBlue;
                case MenuColorEnum.Green:
                    return System.Windows.Media.Colors.LightGreen;
                case MenuColorEnum.Orange:
                    return System.Windows.Media.Colors.Orange;
                case MenuColorEnum.Red:
                    return System.Windows.Media.Colors.Red;
                case MenuColorEnum.Yellow:
                    return System.Windows.Media.Colors.YellowGreen;

                case MenuColorEnum.Blue01:
                    return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFB5B5FF");
                case MenuColorEnum.Blue02:
                    return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFA5A5E8");
                case MenuColorEnum.Blue03:
                    return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF8D8DC7");

            }


            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
