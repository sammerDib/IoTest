using System;

namespace ADCEngine.Parameters.View.Converters
{


    public class ColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            // la couleur selectionné dans l'enum
            EnumColorList enumColorList = (EnumColorList)value;

            // on recupêre le type Colors
            Type t = typeof(System.Windows.Media.Colors);

            // on recherche la propriété static qui correspond la couleur selectionné dans l'enum
            System.Reflection.PropertyInfo pi = t.GetProperty(enumColorList.ToString(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            if (pi == null)
            {
                // pas de correspondance trouvé.
                return null;
            }

            // on converti l'obj en couleur et on renvois
            System.Windows.Media.Color c = (System.Windows.Media.Color)pi.GetValue(null);

            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
