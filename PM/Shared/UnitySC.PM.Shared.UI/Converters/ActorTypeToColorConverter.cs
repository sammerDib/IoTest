using System;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Converters
{
    public class ActorTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var actorType = (ActorType)value;
                switch (actorType)
                {
                    case ActorType.DEMETER:
                    case ActorType.BrightField2D:
                    case ActorType.Darkfield:
                    case ActorType.BrightFieldPattern:
                    case ActorType.Edge:
                    case ActorType.NanoTopography:
                    case ActorType.LIGHTSPEED:
                    case ActorType.BrightField3D:
                    case ActorType.HardwareControl:
                    case ActorType.EdgeInspect:
                        res = Application.Current.FindResource("NodeBackgroundColorIndex4");
                        break;
                    case ActorType.ADC:
                        res = Application.Current.FindResource("NodeBackgroundColorIndex2");
                        break;
                    case ActorType.DataflowManager:
                        res = Application.Current.FindResource("NodeBackgroundColorIndex0");
                        break;
                    default:
                        throw new InvalidOperationException("Unknow ModuleID");
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
