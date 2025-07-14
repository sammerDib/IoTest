using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using AppLauncher.ViewModel;

namespace AppLauncher.Converters
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class StatusToColorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (ExecutionStatus)Enum.Parse(typeof(ExecutionStatus), value.ToString());
            switch (status)
            {
                case ExecutionStatus.Stopped:
                    return new SolidColorBrush(Colors.Red);
                case ExecutionStatus.Running:
                    return new SolidColorBrush(Colors.Green);
                case ExecutionStatus.Starting:
                case ExecutionStatus.Stopping:
                    return new SolidColorBrush(Colors.Orange);
                case ExecutionStatus.Unknown:
                    return new SolidColorBrush(Colors.Gray);
                case ExecutionStatus.Error:
                    return new SolidColorBrush(Colors.Red);

            }
            return new SolidColorBrush(Colors.Gray);
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
