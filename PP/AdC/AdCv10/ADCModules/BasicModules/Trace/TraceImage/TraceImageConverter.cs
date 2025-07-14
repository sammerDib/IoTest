using System;
using System.Windows.Data;


namespace BasicModules.Trace
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class TraceImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            TraceImage trace = (TraceImage)value;
            if (targetType == Type.GetType("System.String"))
                return trace.ToString();

            return trace.WpfBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
