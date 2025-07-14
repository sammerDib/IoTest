using System.Text;

namespace UnitySC.Rorze.Emulator.Common
{
    internal class UtilitiesFunctions
    {
        private UtilitiesFunctions()
        {
        }

        public static string FormatString(string stringToFormat)
        {
            string rezString = stringToFormat.Replace("\a", "\\a");
            rezString = rezString.Replace("\b", "\\b");
            rezString = rezString.Replace("\f", "\\f");
            rezString = rezString.Replace("\n", "\\n");
            rezString = rezString.Replace("\r", "\\r");
            rezString = rezString.Replace("\t", "\\t");
            rezString = rezString.Replace("\v", "\\v");
            rezString = rezString.Replace("\0", "\\0");
            rezString = rezString.Replace(Encoding.ASCII.GetString(new byte[] { 01 }), "^a");
            
            return rezString;
        }

        public static string ReFormatString(string stringToFormat)
        {
            string rezString = stringToFormat.Replace("\\a","\a");
            rezString = rezString.Replace("\\b","\b");
            rezString = rezString.Replace("\\f","\f");
            rezString = rezString.Replace("\\n","\n");
            rezString = rezString.Replace("\\r","\r");
            rezString = rezString.Replace("\\t","\t");
            rezString = rezString.Replace("\\v","\v");
            rezString = rezString.Replace("^a", Encoding.ASCII.GetString(new byte[] { 01 }));
            
            return rezString;
        }
    }
}
