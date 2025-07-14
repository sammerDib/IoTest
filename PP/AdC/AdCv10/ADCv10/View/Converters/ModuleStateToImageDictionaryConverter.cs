using System;
using System.Windows;
using System.Windows.Data;

using ADC.ViewModel;

namespace ADC.View.Converters
{
    public class ModuleStateToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                eModuleVisualState moduleState = (eModuleVisualState)value;
                switch (moduleState)
                {
                    case eModuleVisualState.Running:
                        res = Application.Current.FindResource("RunningWithAnimationADCImage");
                        break;
                    case eModuleVisualState.Stopped:
                        res = Application.Current.FindResource("ValidWithCircleADCImage");
                        break;
                    case eModuleVisualState.Error:
                        res = Application.Current.FindResource("ErrorADCImage");
                        break;
                    case eModuleVisualState.Warning:
                        res = Application.Current.FindResource("WarningWithCircleADCImage");
                        break;
                    case eModuleVisualState.Exports:
                        res = Application.Current.FindResource("FavoritesADCImage");
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
