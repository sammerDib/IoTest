using System;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Global
{
    public class DeviceStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var deviceStatus = (DeviceStatus)value;
                switch (deviceStatus)
                {
                    case DeviceStatus.Ready:
                        res = Application.Current.FindResource("ImageValidColor");
                        break;

                    case DeviceStatus.Error:
                        res = Application.Current.FindResource("ImageErrorColor");
                        break;

                    case DeviceStatus.Warning:
                        res = Application.Current.FindResource("ImageWarningColor");
                        break;

                    case DeviceStatus.Unknown:
                        res = Application.Current.FindResource("ImageDisabledColor");
                        break;

                    default:
                        res = null;
                        break;
                }
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}