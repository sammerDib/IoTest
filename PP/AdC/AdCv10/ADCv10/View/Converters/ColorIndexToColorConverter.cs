using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ADC.View.Converters
{
    /// <summary>
    /// Conversion de l'index d'une couleur vers une couleur du noeud pour la vue
    /// </summary>
    public class ColorIndexToNodeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int colorIndex = (int)value;
            Brush brush;

            if (colorIndex >= 0)
                brush = (Brush)Application.Current.FindResource("NodeBackgroundColorIndex" + colorIndex % 6);
            else
                brush = (Brush)Application.Current.FindResource("NodeBackgroundColorIndexError");

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
