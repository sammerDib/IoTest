using System;
using System.Windows;
using System.Windows.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Converters
{
    public class ActorTypeToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
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
                case ActorType.EdgeInspect:
                case ActorType.HardwareControl:
                case ActorType.ANALYSE:
                    res = Application.Current.FindResource("Chamber");
                    break;
                case ActorType.ADC:
                    res = Application.Current.FindResource("Running");
                    break;
                case ActorType.DataflowManager:
                    res = Application.Current.FindResource("Dataflow");
                    break;
                default:
                    res = Application.Current.FindResource("Folder");
                    break;
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
