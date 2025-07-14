using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.GUI.Common.Resources;

namespace UnitySC.GUI.Common.UIComponents.Converters
{
    class AlignTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                List<string> resultValues = new List<string>();
                var enumValues = ((AlignType[])value).ToList();
                foreach (var enumValue in enumValues)
                {
                    if (Enum.TryParse(enumValue.ToString(), false, out AlignType alignType))
                    {
                        switch (alignType)
                        {
                            case AlignType.AlignWaferForMainO_FlatCheckingSubO_FlatLocation:
                                resultValues.Add(EquipmentResources.ALIGNTYPE_WITHOUTSUBCHECK);
                                break;
                            case AlignType.AlignWaferForSubO_FlatCheckingSubO_FlatLocation:
                                resultValues.Add(EquipmentResources.ALIGNTYPE_MAIN_FLATCHECK);
                                break;
                            case AlignType.AlignWaferWithoutCheckingSubO_FlatLocation:
                                resultValues.Add(EquipmentResources.ALIGNTYPE_SUB_FLATCHECK);
                                break;
                        }
                    }
                }
                return resultValues.ToArray();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
