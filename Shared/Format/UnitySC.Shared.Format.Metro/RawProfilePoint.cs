
using System;
using UnitySC.Shared.Tools.Units;

using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace UnitySC.Shared.Format.Metro
{

    [Serializable]
    [DataContract]
    public class RawProfile
    {
        [XmlAttribute("XUnit")]
        [DataMember]
        public LengthUnit XUnit { get; set; }

        [XmlAttribute("ZUnit")]
        [DataMember]
        public LengthUnit ZUnit { get; set; }

        [XmlArrayItem("pt")]
        [DataMember]
        public List<RawProfilePoint> RawPoints { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RawProfilePoint
    {
        [XmlAttribute("X")]
        [DataMember]
        public double X { get; set; }

        [XmlAttribute("Z")]
        [DataMember]
        public double Z { get; set; }

        public override string ToString()
        {
            return $"{X}; {Z}";
        }
    }
}
