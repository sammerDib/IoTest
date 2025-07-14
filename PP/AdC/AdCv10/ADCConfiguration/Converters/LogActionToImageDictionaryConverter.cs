using System;
using System.Windows;
using System.Windows.Data;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.Converters
{
    public class LogActionToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                Dto.Log.ActionTypeEnum actionType = (Dto.Log.ActionTypeEnum)value;
                switch (actionType)
                {
                    case Dto.Log.ActionTypeEnum.Add:
                        res = Application.Current.FindResource("AddADCImage");
                        break;
                    case Dto.Log.ActionTypeEnum.Edit:
                        res = Application.Current.FindResource("EditADCImage");
                        break;
                    case Dto.Log.ActionTypeEnum.Remove:
                        res = Application.Current.FindResource("DeleteADCImage");
                        break;
                    case Dto.Log.ActionTypeEnum.Connect:
                        res = Application.Current.FindResource("LogOnADCImage");
                        break;
                    case Dto.Log.ActionTypeEnum.Disconnect:
                        res = Application.Current.FindResource("LogOffADCImage");
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
