using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.DMTConverter
{
    public class BooleanOrToWhiteBlackBrushConverter: MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.OfType<bool>().Count() != value.Length)
            {
                throw new ArgumentException("All values must be of type Boolean.");
            }
            return value.OfType<bool>().Any(b => b) ? Brushes.White : Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
