using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.DMTConverter
{
    internal class DoubleToStringPositionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = (double)value;

            // Convert integer values to corresponding strings
            switch (doubleValue)
            {
                case 0:
                    return "Move";
                case 1:
                    return "Load";
                case 2:
                    return "Process";
                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
