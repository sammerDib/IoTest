using System;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Converters
{
    public class RecipeStateToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var recipeType = (RecipeState)value;
                switch (recipeType)
                {
                    case RecipeState.Local:
                    case RecipeState.New:
                        res = Application.Current.FindResource("File");
                        break;
                    case RecipeState.Shared:
                        res = Application.Current.FindResource("ShareEnabled");
                        break;
                    case RecipeState.Template:
                        res = Application.Current.FindResource("TemplateRecipe");
                        break;
                    default:
                        throw new InvalidOperationException("Unknow Recipe Type");
                }
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
