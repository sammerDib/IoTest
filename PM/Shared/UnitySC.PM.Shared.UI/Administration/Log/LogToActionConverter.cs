using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Dto = UnitySC.DataAccess.Dto;

namespace UnitySC.PM.Shared.UI.Administration.Log
{

    public class LogToActionConverter : IValueConverter
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
                        res = Application.Current.FindResource("Add");
                        break;
                    case Dto.Log.ActionTypeEnum.Edit:
                        res = Application.Current.FindResource("Edit");
                        break;
                    case Dto.Log.ActionTypeEnum.Remove:
                        res = Application.Current.FindResource("Delete");
                        break;
                    case Dto.Log.ActionTypeEnum.Connect:
                        res = Application.Current.FindResource("LogOn");
                        break;
                    case Dto.Log.ActionTypeEnum.Disconnect:
                        res = Application.Current.FindResource("LogOff");
                        break;
                    case Dto.Log.ActionTypeEnum.Restore:
                        res = Application.Current.FindResource("Undo");
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
