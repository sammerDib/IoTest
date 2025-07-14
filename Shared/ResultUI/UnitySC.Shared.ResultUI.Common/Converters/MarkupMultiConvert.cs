using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.Shared.ResultUI.Common.Converters
{
    public abstract class MarkupMultiConvert : MarkupExtension, IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);

        #endregion

        #region Overrides of MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}
