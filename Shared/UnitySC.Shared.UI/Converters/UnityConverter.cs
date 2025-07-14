using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.UI.Converters
{
    public class MessageTypeToImageDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                var messageType = (MessageLevel)value;
                switch (messageType)
                {
                    case MessageLevel.None:
                        res = null;
                        break;

                    case MessageLevel.Information:
                        res = Application.Current.FindResource("Info");
                        break;

                    case MessageLevel.Question:
                        res = Application.Current.FindResource("Help");
                        break;

                    case MessageLevel.Success:
                        res = Application.Current.FindResource("Valid");
                        break;

                    case MessageLevel.Warning:
                        res = Application.Current.FindResource("Warning");
                        break;

                    case MessageLevel.Error:
                        res = Application.Current.FindResource("Error");
                        break;

                    case MessageLevel.Fatal:
                        res = Application.Current.FindResource("Error");
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

    public class LayerIndexToLayerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int colorIndex = (int)value;
            Brush brush;

            brush = (Brush)Application.Current.FindResource("LayerBrushIndex" + colorIndex % 6);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
