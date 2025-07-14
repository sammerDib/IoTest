using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    [ValueConversion(typeof(BuzzerState), typeof(PathGeometry))]
    public class BuzzerStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Notifier.IsInDesignModeStatic) return PathIcon.AudioOff;

            if (value != null && Enum.TryParse(value.ToString(), false, out BuzzerState buzzerState))
            {
                switch (buzzerState)
                {
                    case BuzzerState.Undetermined:
                    case BuzzerState.Off:
                        return PathIcon.AudioOff;
                    case BuzzerState.On:
                    case BuzzerState.Slow:
                    case BuzzerState.Fast:
                        return PathIcon.AudioOn;
                }
            }

            return PathIcon.AudioOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
