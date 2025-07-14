using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E90;

using Castle.Core.Internal;

namespace UnitySC.UTO.Controller.UIComponents.Converters
{
     public class E90SubstrateToLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Substrate substrate)
            {
                if (Notifier.IsInDesignModeStatic) return "SubstrateLocation";

                if (!substrate.SubstLocID.IsNullOrEmpty())
                {
                    return substrate.SubstLocID;
                }

                if (!substrate.BatchLocID.IsNullOrEmpty())
                {
                    return substrate.BatchLocID;
                }

                return "";
            }

            throw new ArgumentException(
                $"Given value for {nameof(E90SubstrateToLocationConverter)} is not type of {nameof(Substrate)}.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
