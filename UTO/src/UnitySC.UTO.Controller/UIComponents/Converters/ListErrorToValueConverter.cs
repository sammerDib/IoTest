using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

using UnitySC.UTO.Controller.Views.Panels.Gem;

namespace UnitySC.UTO.Controller.UIComponents.Converters
{
    public class ListErrorToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Error> collection)
            {
                if (collection.Count == 0)
                    return "";
                else if (collection.Count <= 1)
                    return collection[0].Id + " " + collection[0].Description;
                else
                    return collection.Count.ToString();
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
