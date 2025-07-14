using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.PM.DMT.Shared.UI.Converter
{
    public class RemoveFileExtensionConverter : MarkupExtension, IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fileName = null;
            if (value != null)
            {
                fileName= value as string;
                fileName=Path.GetFileNameWithoutExtension(fileName);
           
            }

            return fileName;
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
