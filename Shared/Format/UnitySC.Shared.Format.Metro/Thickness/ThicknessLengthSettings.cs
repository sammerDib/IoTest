using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Colors;

namespace UnitySC.Shared.Format.Metro.Thickness
{
    [DataContract]
    public class ThicknessLengthSettings : NamedLengthSettings
    {
        [DataMember]
        public bool IsMeasured { get; set; }

        [DataMember]
        [XmlElement(Type = typeof(XmlColor))]
        public Color LayerColor { get; set; }
    }
}
