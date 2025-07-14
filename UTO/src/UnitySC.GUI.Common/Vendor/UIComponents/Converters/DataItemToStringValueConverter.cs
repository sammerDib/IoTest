using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.Semi.Communication.Abstractions.E5;

using UnitySC.GUI.Common.Vendor.Extensions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class DataItemToStringValueConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataItem = value as DataItem;
            return dataItem == null ? string.Empty : dataItem.GetValueAsString("-");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
