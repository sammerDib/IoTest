using System.Globalization;

namespace UnitySC.Shared.Format.Helper
{
    public class CSVStringBuilder : DelimitedStringBuilder
    {
        public CSVStringBuilder()
            : base(GetCSVSeparator())
        {
        }

        public static string GetCSVSeparator()
        {
            // Use InstalledUICulture since CurrentCulture or CurrentCulture could be changed in thread.culture
            return CultureInfo.InstalledUICulture.TextInfo.ListSeparator;
        }
    }
}
