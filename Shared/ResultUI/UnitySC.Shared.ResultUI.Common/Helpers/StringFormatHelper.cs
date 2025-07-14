namespace UnitySC.Shared.ResultUI.Common.Helpers
{
    public static class StringFormatHelper
    {
        public static string GetFormat(int digits)
        {
            string format = "{0:0";
            for (int i = 0; i < digits; i++)
            {
                if (i == 0) format += ".";
                format += "0";
            }

            format += "}";
            return format;
        }
    }
}
