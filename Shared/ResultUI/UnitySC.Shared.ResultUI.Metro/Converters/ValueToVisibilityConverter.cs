using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp;

namespace UnitySC.Shared.ResultUI.Metro.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class ValueToVisibilityConverter : IValueConverter
    {
        public Visibility VisibilityEqual { get; set; } = Visibility.Visible;
        public Visibility VisibilityNonEqual { get; set; } = Visibility.Collapsed;
        public Visibility VisibilityNull { get; set; } = Visibility.Collapsed;

        public WarpViewerType ViewerTypeVisibilityValue { get; set; } = WarpViewerType.WARP;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string desc = value as string;

                if (!string.IsNullOrEmpty(desc))
                {
                    WarpViewerType viewerType = EnumUtils.GetEnumFromDescription<WarpViewerType>(value as string);

                    return viewerType == ViewerTypeVisibilityValue ? VisibilityEqual : VisibilityNonEqual;
                }
                else
                    return VisibilityNull;
            }
            else if (value is double)
            {
                return (double)value < 0 ? VisibilityEqual : VisibilityNonEqual;
            }
            else
                return VisibilityNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
