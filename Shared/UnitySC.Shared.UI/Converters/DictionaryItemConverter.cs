using System;
using System.Collections;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.UI.Converters
{
    public class DictionaryItemConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                var myDict = values[0] as IDictionary;
                var myKey = values[1] as object;
                if (myDict != null && myKey != null)
                {
                    //the automatic conversion from Uri to string doesn't work
                    //return myDict[myKey];
                    return myDict[myKey];
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
