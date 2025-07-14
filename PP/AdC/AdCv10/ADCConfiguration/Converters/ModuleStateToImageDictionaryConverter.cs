using System;
using System.Windows;
using System.Windows.Data;

using ADC.ViewModel;

namespace ADCConfiguration.Converters
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
                    case eModuleVisualState.Same:
                        res = Application.Current.FindResource("SameADCImage");
                        break;
                    case eModuleVisualState.Different:
                        res = Application.Current.FindResource("DifferentADCImage");
                        break;
                    case eModuleVisualState.Added:
                        res = Application.Current.FindResource("AddedADCImage");
                        break;
                    case eModuleVisualState.Removed:
                        res = Application.Current.FindResource("RemovedADCImage");
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
