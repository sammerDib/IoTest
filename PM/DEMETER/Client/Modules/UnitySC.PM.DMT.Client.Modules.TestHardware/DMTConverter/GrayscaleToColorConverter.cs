using System;
using System.Windows.Data;
using System.Windows.Markup;

using MColor = System.Windows.Media.Color;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.DMTConverter
{
    [ValueConversion(typeof(int), typeof(System.Windows.Media.Color))]
    public class GrayscaleToColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int grayscale = ((int)value);
            MColor grayScaleColor = MColor.FromRgb((byte)grayscale, (byte)grayscale, (byte)grayscale);
            MColor mediaColor = MColor.FromArgb(grayScaleColor.A, grayScaleColor.R, grayScaleColor.G, grayScaleColor.B);
            System.Windows.Media.SolidColorBrush brushColor = new System.Windows.Media.SolidColorBrush(mediaColor);
            return brushColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
