using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Converters
{
    public class DataTreeItemContainerStyleConverter : IValueConverter
    {
        /// <summary>
        /// Item container style to use when <see cref="Controls.DataTree.GridView" /> is <c>null</c>.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Item container style to use when <see cref="Controls.DataTree.GridView" /> is not <c>null</c>, typically when a
        /// <see cref="GridView" /> is applied.
        /// </summary>
        public object GridViewValue { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Controls.DataTree dataTree)
            {
                return dataTree.GridView != null ? GridViewValue : DefaultValue;
            }

            return value is GridView ? GridViewValue : DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        #endregion
    }
}
