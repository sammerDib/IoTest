using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using ADC.ViewModel;

namespace ADCConfiguration.Converters
{
    /// <summary>
    /// Conversion de l'index du stage vers une couleur d'un noeud pour la vue
    /// </summary>
    public class ColorIndexToNodeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            eModuleVisualState moduleVisualState = (eModuleVisualState)value;
            Brush brush;

            switch (moduleVisualState)
            {
                case eModuleVisualState.Different:
                    brush = (Brush)Application.Current.FindResource("NodeBackgroundDifferent");
                    break;
                case eModuleVisualState.Same:
                    brush = (Brush)Application.Current.FindResource("NodeBackgroundSame");
                    break;
                case eModuleVisualState.Added:
                    brush = (Brush)Application.Current.FindResource("NodeBackgroundAdded");
                    break;
                case eModuleVisualState.Removed:
                    brush = (Brush)Application.Current.FindResource("NodeBackgroundRemoved");
                    break;
                default:
                    brush = (Brush)Application.Current.FindResource("NodeBackgroundError");
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
