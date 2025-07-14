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
    public class LogAreaToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                UnitySC.DataAccess.Dto.Log.TableTypeEnum tableType = (Dto.Log.TableTypeEnum)value;
                switch (tableType)
                {
                    case Dto.Log.TableTypeEnum.User:
                        res = Application.Current.FindResource("User");
                        break;
                    case Dto.Log.TableTypeEnum.Chamber:
                        res = Application.Current.FindResource("Chamber");
                        break;
                    case Dto.Log.TableTypeEnum.Tool:
                        res = Application.Current.FindResource("Tool");
                        break;
                    case Dto.Log.TableTypeEnum.Recipe:
                        res = Application.Current.FindResource("File");
                        break;
                    case Dto.Log.TableTypeEnum.Configuration:
                        res = Application.Current.FindResource("Settings");
                        break;
                    case Dto.Log.TableTypeEnum.WaferType:
                        res = Application.Current.FindResource("Wafer");
                        break;
                    case Dto.Log.TableTypeEnum.Step:
                        res = Application.Current.FindResource("Step");
                        break;
                    case Dto.Log.TableTypeEnum.Dataflow:
                        res = Application.Current.FindResource("Dataflow");
                        break;
                    case Dto.Log.TableTypeEnum.Product:
                        res = Application.Current.FindResource("Wafer");
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
