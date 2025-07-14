using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro
{
    /// <summary>
    /// ax +by + cz
    /// </summary>
    public class BestFitPlan
    {
        [XmlElement("A")]
        [DataMember]
        public Length CoeffA { get; set; }

        [XmlElement("B")]
        [DataMember]
        public Length CoeffB { get; set; }

        [XmlElement("C")]
        [DataMember]
        public Length CoeffC { get; set; }
    }
}
