using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.Semi.Gem300.Abstractions.E40;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class ProcessStartToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProcessStart process)
            {
                return process == ProcessStart.AutomaticStart;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return val ? ProcessStart.AutomaticStart : ProcessStart.ManualStart;
            }
            return ProcessStart.ManualStart;
        }
    }
}
