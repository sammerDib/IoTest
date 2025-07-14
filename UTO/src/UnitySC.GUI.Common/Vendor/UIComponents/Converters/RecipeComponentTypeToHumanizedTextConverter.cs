using System;
using System.Globalization;
using System.Windows.Data;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public class RecipeComponentTypeToHumanizedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString())) return string.Empty;

            var humanizedType = value.ToString().Humanize();
            if (humanizedType.Contains("instruction"))
            {
                humanizedType = humanizedType.Replace("instruction", "");
            }
            else if (humanizedType.Contains("step"))
            {
                humanizedType = humanizedType.Replace("step", "");
            }

            return humanizedType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
