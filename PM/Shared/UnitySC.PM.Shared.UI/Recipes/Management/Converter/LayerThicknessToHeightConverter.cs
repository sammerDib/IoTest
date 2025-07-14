using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.UI.Recipes.Management.Converter
{
    public class LayerThicknessToHeightConverter : MarkupExtension, IValueConverter
    {
        private const float Max = 300;
        private const float Min = 35;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var thickness = (Length)value;
            if (thickness is  null)
                return Min;
            var height = thickness.Micrometers / 2.0;
            height = Math.Max(height, Min);
            height = Math.Min(height, Max);
            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
