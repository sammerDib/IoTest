using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.Semi.Gem300.Abstractions.E94;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    class StartMethodToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StartMethod startMethod)
            {
                return startMethod == StartMethod.Auto;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return val ? StartMethod.Auto : StartMethod.UserStart;
            }

            return StartMethod.UserStart;
        }
    }
}
