using System;
using System.Windows;
using System.Windows.Data;

namespace ADCConfiguration.Converters
{
    public class FindResourceFromString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string resourceName= (string)value;
                if (!resourceName.Contains("ADCImage"))
                    resourceName = resourceName + "ADCImage";

                return Application.Current.FindResource(resourceName);
            }
               
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
