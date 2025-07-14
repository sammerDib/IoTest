using System;
using System.Globalization;

namespace Common.WinForms
{
    public class ParserInt32 : Parser<Int32>
    {
        public ParserInt32(CultureInfo culture)
        {
            _culture = culture;
        }
        readonly CultureInfo _culture;

        public Int32 Parse(string text)
        {
            try
            {
                //xArgumentNullException s is null.
                //>FormatException s does not represent a number in a valid format.
                //.OverflowException s represents a number that is less than MinValue or greater than MaxValue.
                return System.Int32.Parse(text, _culture);
            }
            catch (OverflowException ex)
            {
                throw new FormatException(string.Empty, ex);
            }
        }
    }
}
