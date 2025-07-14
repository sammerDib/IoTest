using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data
{
    [DataContract]
    [Serializable]
    public class MaterialCharacteristic
    {
        [DataMember]
        [XmlArrayItem("RI")]
        public List<ComplexRefractiveIndex> RefractiveIndexList { get; set; }
    }

    [DataContract]
    public class ComplexRefractiveIndex
    {
        [DataMember]
        [XmlAttribute("w")]
        public double WavelengthUm { get; set; }

        [DataMember]
        [XmlAttribute("n")]
        public double RefractiveIndex { get; set; }

        [DataMember]
        [XmlAttribute("k")]
        public double ExtinctionCoefficient { get; set; }
    }
}