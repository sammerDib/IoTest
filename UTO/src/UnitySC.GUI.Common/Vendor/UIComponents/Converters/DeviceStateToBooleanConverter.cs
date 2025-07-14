using System;
using System.Globalization;
using System.Windows.Data;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class DeviceStateToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not OperatingModes operatingModes)
            {
                return false;
            }

            switch (operatingModes)
            {
                case OperatingModes.Executing:
                case OperatingModes.Initialization:
                    return false;
                case OperatingModes.Idle:
                case OperatingModes.Maintenance:
                    return true;
                default:
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
