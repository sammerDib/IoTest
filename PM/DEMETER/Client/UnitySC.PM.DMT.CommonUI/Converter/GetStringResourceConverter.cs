using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.PM.DMT.CommonUI.Converter
{
    public class GetStringResourceConverter : MarkupExtension, IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                res = Properties.Resources.ResourceManager.GetString(value as string);
                if (res == null)
                    res = value;
            }

            return res;
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
