using System.Globalization;
using System.Text;

namespace UnitySC.Shared.Format.Helper
{
    public class DelimitedStringBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();

        private readonly string _separator;

        public DelimitedStringBuilder(string delimiter)
        {
            _separator = delimiter ?? CultureInfo.InstalledUICulture.TextInfo.ListSeparator;
        }

        public void Append(params string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Add(fields[i]);
            }
        }

        public void AppendLine(params string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Add(fields[i]);
            }

            _sb.AppendLine();
        }

        public void AppendLineWithoutFinalDelim(params string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                Add(fields[i]);
            }

            if (fields.Length > 0)
            {
                RemoveEndDelim();
            }

            _sb.AppendLine();
        }

        public void AppendLine_NoDelim(string field)
        {
            _sb.AppendLine(field);
        }

        public void Add(string field)
        {
            _sb.Append(field);
            _sb.Append(_separator); // add Delimiter separator
        }

        public void Add_NoDelim(string field) { _sb.Append(field); }

        public void RemoveEndDelim() { _sb.Remove(_sb.Length - 1, _separator.Length); }

        public void Clear() { _sb.Clear(); }

        public override string ToString() { return _sb.ToString(); }
    }
}
