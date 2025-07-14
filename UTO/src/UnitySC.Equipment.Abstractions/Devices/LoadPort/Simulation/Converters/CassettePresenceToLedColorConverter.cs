using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    /// <summary>
    /// Convert from CassettePresence to color for leds
    /// </summary>
    public class CassettePresenceToLedColorConverter : IValueConverter
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
            if (value != null)
            {
                CassettePresence presence = (CassettePresence)value;
                switch (presence)
                {
                    case CassettePresence.Absent:
                        return Colors.Black;
                    case CassettePresence.Correctly:
                        return Colors.LimeGreen;
                    case CassettePresence.NoPresentPlacement:
                    case CassettePresence.PresentNoPlacement:
                    case CassettePresence.Unknown:
                        return Colors.Orange;

                }
            }

            return Colors.Orange;
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
