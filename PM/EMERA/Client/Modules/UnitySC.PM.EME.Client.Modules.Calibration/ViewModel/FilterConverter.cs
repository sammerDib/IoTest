using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.EME.Service.Interface.Calibration;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class FilterInstallationStateToImageDictionaryConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var status = (FilterInstallationState)value;
                switch (status)
                {
                    case FilterInstallationState.Validated:
                        res = Application.Current.FindResource("ValidWithCircle");
                        break;
                    case FilterInstallationState.Missing:
                        res = Application.Current.FindResource("WarningWithCircle");
                        break;
                    default:
                        return null;
                }
            }

            return res;
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

    public class FilterCalibrationStateToImageDictionaryConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            switch ((FilterCalibrationState)value)
            {
                case FilterCalibrationState.CalibrationError:
                    return Application.Current.FindResource("Error");
                case FilterCalibrationState.Calibrated:
                    return Application.Current.FindResource("Valid");
                case FilterCalibrationState.Uncalibrated:
                    return Application.Current.FindResource("WarningWithCircle");
                default:
                    return null;
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
