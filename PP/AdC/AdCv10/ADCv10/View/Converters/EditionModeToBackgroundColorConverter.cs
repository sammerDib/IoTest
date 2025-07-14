using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using ADC.AdcEnum;

namespace ADC.View.Converters
{
    ///////////////////////////////////////////////////////////////////////
    // Conversion Module => Help
    ///////////////////////////////////////////////////////////////////////
    [ValueConversion(typeof(RecipeEditionMode), typeof(Brush))]
    public class EditionModeToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush brush = Brushes.BlanchedAlmond;
            if ((value is RecipeEditionMode))
            {
                RecipeEditionMode editionMode = (RecipeEditionMode)value;

                switch (editionMode)
                {
                    case RecipeEditionMode.ExpertRecipeEdition:
                        brush = (Brush)Application.Current.FindResource("ExpertViewBackground");
                        break;
                    case RecipeEditionMode.SimplifiedRecipeEdition:
                        brush = (Brush)Application.Current.FindResource("SimplifyViewBackground");
                        break;
                    case RecipeEditionMode.RecipeProcessing:
                        brush = (Brush)Application.Current.FindResource("RunViewBackground");
                        break;
                }
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
