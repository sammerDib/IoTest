using System;
using System.Windows;
using System.Windows.Data;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.Converters
{
    public class LogTableToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                Dto.Log.TableTypeEnum tableType = (Dto.Log.TableTypeEnum)value;
                switch (tableType)
                {
                    case Dto.Log.TableTypeEnum.User:
                        res = Application.Current.FindResource("UserADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.Chamber:
                        res = Application.Current.FindResource("ChamberADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.Tool:
                        res = Application.Current.FindResource("ToolADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.Recipe:
                        res = Application.Current.FindResource("RecipeFileADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.Configuration:
                        res = Application.Current.FindResource("ToolADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.WaferType:
                        res = Application.Current.FindResource("WaferADCImage");
                        break;
                    case Dto.Log.TableTypeEnum.Vid:
                        res = Application.Current.FindResource("VidADCImage");
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
