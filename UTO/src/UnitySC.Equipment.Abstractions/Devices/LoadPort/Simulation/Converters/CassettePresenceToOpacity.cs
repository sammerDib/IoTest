using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Converters
{
    public class CassettePresenceToOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "bool")
            {
                var mapping = (bool)value;
                if (mapping)
                    return 1;
                return 0.3;
            }

            var state = (CassettePresence)value;
            switch (state)
            {
                case CassettePresence.Absent:
                    return 0.3;
                default:
                    return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
