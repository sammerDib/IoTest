using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Agileo.UserControls;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    public class LedStateToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color colorOn = Colors.Black;
            Color colorOff = Colors.White;
            Color colorError = Colors.Red;

            LedState state;
            if (value == null || !Enum.TryParse(value.ToString(), out state))
            {
                return new SolidColorBrush(colorError);
            }

            switch (state)
            {
                case LedState.Off:
                    return new SolidColorBrush(colorOff);
                case LedState.On:
                    return new SolidColorBrush(colorOn);
                default:
                    return new SolidColorBrush(colorError);
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
