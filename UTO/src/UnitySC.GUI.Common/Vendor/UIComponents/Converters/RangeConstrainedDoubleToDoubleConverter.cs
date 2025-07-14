using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class RangeConstrainedDoubleToDoubleConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(double), typeof(RangeConstrainedDoubleToDoubleConverter),
                new PropertyMetadata(0.0));

        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(double), typeof(RangeConstrainedDoubleToDoubleConverter),
                new PropertyMetadata(1.0));
        
        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse(
                    ((string)value).Replace(',', '.'),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double result))
            {
                return DependencyProperty.UnsetValue;
            }

            return MathHelper.Clamp(result, Min, Max);
        }
    }
}
