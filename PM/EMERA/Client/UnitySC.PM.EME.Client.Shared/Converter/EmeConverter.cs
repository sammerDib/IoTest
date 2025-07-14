using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

using UnitySC.PM.EME.Client.Shared.Image;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Client.Shared.Converter
{
    public class FlowStateToImageDictionaryConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            switch ((FlowState)value)
            {
                case FlowState.Error:
                    return Application.Current.FindResource("Error");
                case FlowState.Partial:
                    return Application.Current.FindResource("WarningWithCircle");
                case FlowState.InProgress:
                    return Application.Current.FindResource("RunningWithAnimation");
                case FlowState.Waiting:
                    return Application.Current.FindResource("Intializing");
                case FlowState.Success:
                    return Application.Current.FindResource("Valid");
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

        [ValueConversion(typeof(ServiceImage), typeof(BitmapSource))]
    public class ServiceImageToCachedBitmapSourceConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ServiceImage image))
            {
                return null;
            }

            return image.ToCachedBitmapSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
