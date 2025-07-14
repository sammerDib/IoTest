using System;
using System.Globalization;
using System.Windows.Data;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.GUI.Common.Resources;

namespace UnitySC.GUI.Common.UIComponents.Converters
{
    class TextToAlignTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AlignType alignType)
            {
                switch (alignType)
                {
                    case AlignType.AlignWaferWithoutCheckingSubO_FlatLocation:
                        return EquipmentResources.ALIGNTYPE_WITHOUTSUBCHECK;

                    case AlignType.AlignWaferForMainO_FlatCheckingSubO_FlatLocation:
                        return EquipmentResources.ALIGNTYPE_MAIN_FLATCHECK;

                    case AlignType.AlignWaferForSubO_FlatCheckingSubO_FlatLocation:
                        return EquipmentResources.ALIGNTYPE_SUB_FLATCHECK;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return AlignType.AlignWaferWithoutCheckingSubO_FlatLocation;
            }

            var stringValue = value.ToString();
            if (stringValue == EquipmentResources.ALIGNTYPE_WITHOUTSUBCHECK)
            {
                return AlignType.AlignWaferWithoutCheckingSubO_FlatLocation;
            }
            else if (stringValue == EquipmentResources.ALIGNTYPE_MAIN_FLATCHECK)
            {
                return AlignType.AlignWaferForMainO_FlatCheckingSubO_FlatLocation;
            }
            else if (stringValue == EquipmentResources.ALIGNTYPE_SUB_FLATCHECK)
            {
                return AlignType.AlignWaferForSubO_FlatCheckingSubO_FlatLocation;
            }

            return AlignType.AlignWaferWithoutCheckingSubO_FlatLocation;
        }
    }
}
