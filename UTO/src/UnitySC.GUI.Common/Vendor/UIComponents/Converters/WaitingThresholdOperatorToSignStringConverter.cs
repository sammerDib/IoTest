using System;
using System.Globalization;
using System.Windows.Data;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class WaitingThresholdOperatorToSignStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && Enum.TryParse(value.ToString(), false, out WaitingOperator waitingThresholdOperator))
            {
                return waitingThresholdOperator.ToHumanizedString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return WaitingOperatorExtensions.FromHumanizedString(value.ToString());
        }
    }
}
