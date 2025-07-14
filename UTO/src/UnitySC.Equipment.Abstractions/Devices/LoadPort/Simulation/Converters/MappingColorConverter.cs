using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    /// <summary>
    /// Converts WaferStates to the related color
    /// </summary>
    public class MappingColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert a value.
        /// </summary>
        /// <param name="value">Value produced by the binding source</param>
        /// <param name="targetType">Property type of  the binding source</param>
        /// <param name="parameter">Parameter to use in the conversion.</param>
        /// <param name="culture">Culture to use</param>
        /// <returns>
        /// The converted value
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter == null)
            {
                if (value is DisplayColor)
                {
                    var color = (DisplayColor)value;
                    switch (color)
                    {
                        case DisplayColor.Undefined:
                            return new SolidColorBrush(Colors.Transparent);
                        case DisplayColor.Blue:
                            return new SolidColorBrush(Colors.DodgerBlue);
                        case DisplayColor.Green:
                            return new SolidColorBrush(Colors.LimeGreen);
                        case DisplayColor.Pink:
                            return new SolidColorBrush(Colors.Pink);
                        case DisplayColor.Red:
                            return new SolidColorBrush(Colors.OrangeRed);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                LinearGradientBrush lgb = new LinearGradientBrush();
                lgb.EndPoint = new Point(0.902, 0.495);
                lgb.StartPoint = new Point(0.127, 0.49);
                lgb.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x4F, 0x83, 0xFF), 0.445));
                lgb.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x49, 0xFD), 1));
                lgb.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x4F, 0x83, 0xFF), 0.571));
                lgb.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0x49, 0xFD), 0));
                return lgb;
            }

            if (value != null && parameter.ToString() == "Stroke")
            {
                return new SolidColorBrush(Colors.Black);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
