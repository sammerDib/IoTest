using System;
using System.Globalization;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.Shared.ResultUI.Metro.Converters
{
    public class MeasureStateToToleranceDisplayerConverter : MarkupConvert
    {
        #region Implementation of IValueConverter

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MeasureState measureState)
            {
                return Convert(measureState);
            }

            return Tolerance.None;
        }

        public static Tolerance Convert(MeasureState? measureState)
        {
            if (!measureState.HasValue) return Tolerance.None;

            switch (measureState.Value)
            {
                case MeasureState.Success:
                    return Tolerance.Good;
                case MeasureState.Partial:
                    return Tolerance.Warning;
                case MeasureState.Error:
                    return Tolerance.Bad;
                case MeasureState.NotMeasured:
                    return Tolerance.NotMeasured;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
