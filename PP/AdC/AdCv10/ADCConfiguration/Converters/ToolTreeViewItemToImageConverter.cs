using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using ADCConfiguration.ViewModel.Tool.TreeView;

namespace ADCConfiguration.Converters
{
    internal class ToolTreeViewItemToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToolViewModel)
            {
                return Application.Current.FindResource("ToolADCImage");
            }
            /*  else if (value is ChamberViewModel)
              {
                  return Application.Current.FindResource("ChamberADCImage");
              }
              else if (value is WaferTypeViewModel)
              {
                  return Application.Current.FindResource("WaferADCImage");
              }*/
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
