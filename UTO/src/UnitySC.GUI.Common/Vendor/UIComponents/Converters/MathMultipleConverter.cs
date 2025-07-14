using System;
using System.Globalization;
using System.Windows.Data;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Converters
{
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public sealed class MathMultipleConverter : IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null || values[1] == null)
            {
                return Binding.DoNothing;
            }

            if (!double.TryParse(values[0].ToString(), out var value1)
                || !double.TryParse(values[1].ToString(), out var value2))
            {
                return 0;
            }

            switch (Operation)
            {
                default:
                case MathOperation.Add:
                    return value1 + value2;
                case MathOperation.Divide:
                    return value1 / value2;
                case MathOperation.Multiply:
                    return value1 * value2;
                case MathOperation.Subtract:
                    return value1 - value2;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
