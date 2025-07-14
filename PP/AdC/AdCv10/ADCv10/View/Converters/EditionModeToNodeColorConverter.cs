using System;
using System.Windows.Data;
using System.Windows.Media;

using ADC.AdcEnum;

namespace ADC.View.Converters
{
    ///////////////////////////////////////////////////////////////////////
    // Conversion Module => Help
    ///////////////////////////////////////////////////////////////////////
    [ValueConversion(typeof(RecipeEditionMode), typeof(SolidColorBrush))]
    public class EditionModeToNodeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush color = new SolidColorBrush(Colors.Blue);
            if ((value is RecipeEditionMode))
            {
                RecipeEditionMode editionMode = (RecipeEditionMode)value;

                switch (editionMode)
                {
                    case RecipeEditionMode.ExpertRecipeEdition:
                        color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF38B5B8"));
                        break;
                    case RecipeEditionMode.SimplifiedRecipeEdition:
                        color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2186AA"));
                        break;
                    case RecipeEditionMode.RecipeProcessing:
                        color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF79AB54"));
                        break;
                }
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
