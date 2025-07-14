using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Client.Shared.Converter
{
    public class FlowStateToImageDictionaryConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                FlowState state = (FlowState)value;
                switch (state)
                {
                    case FlowState.Error:
                        res = Application.Current.FindResource("Error");
                        break;
                    case FlowState.InProgress:
                        res = Application.Current.FindResource("RunningWithAnimation");
                        break;
                    case FlowState.Waiting:
                        res = Application.Current.FindResource("Intializing");
                        break;
                    case FlowState.Success:
                        res = Application.Current.FindResource("Valid");
                        break;
                    default:
                        return null;
                }
            }

            return res;
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
}
