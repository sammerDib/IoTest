using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.EME.Service.Interface.Recipe.Execution;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class ExecutionStatusToImageConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object res = null;
            if (value != null)
            {
                switch ((ExecutionStatus)value)
                {
                    case ExecutionStatus.Finished:
                        res = Application.Current.FindResource("ValidWithCircle");
                        break;
                    case ExecutionStatus.Canceled:
                        res = Application.Current.FindResource("WarningWithCircle");
                        break;
                    case ExecutionStatus.Failed:
                        res = Application.Current.FindResource("ErrorGeometry");
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
