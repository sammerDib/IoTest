using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.PM.DMT.Shared.UI.Converter
{
    public class IfNan : MarkupExtension, IValueConverter
    {

        public object FallbackValue { get; set; } 

        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double? doubleValue=null;

            if (value is double)
                doubleValue = (double)value;

            if (doubleValue == null || double.IsNaN((double)doubleValue))
            {
                return this.FallbackValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion Public Methods
    }
}
