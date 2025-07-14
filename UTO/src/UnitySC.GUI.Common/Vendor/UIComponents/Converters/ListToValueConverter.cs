using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

using Agileo.Semi.Gem300.Abstractions.E40;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class ListToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> collection)
            {
                if (collection.Count == 0)
                    return "";
                else if (collection.Count <= 1)
                    return collection[0];
                else
                    return collection.Count.ToString();
            }
            else if (value is ReadOnlyCollection<MaterialNameListElement> collection2)
            {
                if (collection2.Count == 0)
                    return "";
                else if (collection2.Count <= 1)
                    return collection2[0].CarrierID;
                else
                    return collection2.Count.ToString();
            }

            return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
    }
}
