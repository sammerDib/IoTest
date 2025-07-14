
using System.Windows.Media;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools.Colors
{

    public class XmlColor
    {
        private Color _color;

        public XmlColor() { }
        public XmlColor(Color c) { _color = c; }

        public static implicit operator Color(XmlColor x)
        {
            return x._color;
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        [XmlText]
        public string Default
        {
            get { return _color.ToString(); }
            set { _color = (Color)ColorConverter.ConvertFromString(value); }
        }
    }
}

