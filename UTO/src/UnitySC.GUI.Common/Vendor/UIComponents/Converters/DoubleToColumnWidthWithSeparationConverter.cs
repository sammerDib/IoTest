using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    class DoubleToColumnWidthWithSeparationConverter : IValueConverter
    {
        public int Columns { get; set; }

        public int SeparationWidth { get; set; }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var separationSize = SeparationWidth * (Columns - 0);
            if (value is double)
            {
                return (((double)value) - separationSize) / Columns;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
