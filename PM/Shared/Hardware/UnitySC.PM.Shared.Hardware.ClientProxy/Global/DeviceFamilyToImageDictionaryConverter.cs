using System;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Global
{
    public class DeviceFamilyToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var deviceFamily = (DeviceFamily)value;
                switch (deviceFamily)
                {
                    case DeviceFamily.Plc:
                        res = Application.Current.FindResource("Connection");
                        break;

                    case DeviceFamily.Camera:
                        res = Application.Current.FindResource("Camera");
                        break;

                    case DeviceFamily.Screen:
                        res = Application.Current.FindResource("Screen");
                        break;

                    case DeviceFamily.Axes:
                        res = Application.Current.FindResource("Wafer");
                        break;

                    case DeviceFamily.Other:
                        res = Application.Current.FindResource("Hardware");
                        break;

                    default:
                        res = Application.Current.FindResource("Hardware");
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
