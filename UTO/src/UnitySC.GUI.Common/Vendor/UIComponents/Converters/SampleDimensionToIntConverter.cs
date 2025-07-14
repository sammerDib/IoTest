using System;
using System.Globalization;
using System.Windows.Data;

using Agileo.SemiDefinitions;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class SampleDimensionToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SampleDimension dimension)
            {
                if (dimension == SampleDimension.S100mm)
                {
                    return 40;
                }
                if (dimension == SampleDimension.S150mm)
                {
                    return 43;
                }
                if (dimension == SampleDimension.S200mm)
                {
                    return 46;
                }
                if (dimension == SampleDimension.S300mm)
                {
                    return 52;
                }
                if (dimension == SampleDimension.S450mm)
                {
                    return 61;
                }
            }

            return 50;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
