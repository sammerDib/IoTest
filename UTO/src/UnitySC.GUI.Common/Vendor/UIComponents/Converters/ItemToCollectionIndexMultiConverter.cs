using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class ItemToCollectionIndexMultiConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;

            var item = values[0];
            var collection = values[1] as IEnumerable;
            if (item == null || collection == null) return false;

            var index = collection.Cast<object>().ToList().FindIndex(i => ReferenceEquals(i, item)) + 1;
            if (targetType == typeof(string))
            {
                return index.ToString();
            }
            return index;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
