using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UnitySC.PM.DMT.Shared.UI.Converter
{
    [ValueConversion(typeof(UnitySC.Shared.Data.Enum.RoiType), typeof(UnitySC.Shared.UI.Controls.ZoomboxImage.RoiType))]
    public class RoiTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (UnitySC.Shared.UI.Controls.ZoomboxImage.RoiType)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (UnitySC.Shared.Data.Enum.RoiType)value;
        }
    }
}
