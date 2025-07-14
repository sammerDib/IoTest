using System;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.ResultUI.Common.Enums;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public class LotViewTypeToImageResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            
            var viewType = (LotView)value;
            switch (viewType)
            {
                case LotView.Stats:
                    return Application.Current.TryFindResource("Stats");

                case LotView.Wafers:
                    return Application.Current.TryFindResource("Wafer");

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
