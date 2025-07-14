using System;
using System.Windows.Data;
using System.Windows.Media;

using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.Services
{
    /// <summary>
    ///  Within the UnitySC.Shared.UI, there's 'ColorToSolidBrushConverter' which converts from System.Drawing.Color to SolidBrush (System.Windows.Media.Color).
    ///  However, since we defined a custom class Color class (SerializableColor.cs) to make it compatible for serialisation through XMLSerialize:
    ///  we will also need to implement a custom converter that takes 'SerializableColor' and converts it to 'SolidBrush' (System.Windows.Media.Color). 
    ///  Because for colours to be displayed at the WPF side, they need to be of type 'System.Windows.Media.Color'.
    /// </summary>
    public class SerializableColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SerializableColor serializableColor)
            {
                var color = Color.FromArgb(serializableColor.A, serializableColor.R, serializableColor.G, serializableColor.B);
                return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                var color = brush.Color;
                return new SerializableColor(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            return null;
        }
    }

}
