using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity.Converter
{
    public class LayerThicknessToHeightConverter : IValueConverter
    {
        private const double Max = 200;
        private const double Min = 35;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thickness = (double)value;
            var height = thickness / 2.0;
            height =  Math.Max(height, Min);
            height = Math.Min(height, Max);
            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
