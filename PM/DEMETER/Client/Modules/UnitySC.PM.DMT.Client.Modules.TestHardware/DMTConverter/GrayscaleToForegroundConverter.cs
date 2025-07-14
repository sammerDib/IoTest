using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MColor = System.Windows.Media.Color;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.DMTConverter
{
    [ValueConversion(typeof(int), typeof(MColor))]
    public class GrayscaleToForegroundConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int grayscale = ((int)value);

            MColor grayScaleColor = Colors.Black;
            if (grayscale < 127)
                grayScaleColor = Colors.White;

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
