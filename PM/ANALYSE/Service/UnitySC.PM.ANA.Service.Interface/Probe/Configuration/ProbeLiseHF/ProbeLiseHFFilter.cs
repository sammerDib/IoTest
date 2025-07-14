using System.Runtime.Serialization;
using System.Xml.Serialization;

using AutoMapper.Mappers;

namespace UnitySC.PM.ANA.Service.Interface.Probe.Configuration.ProbeLiseHF
{
    public enum LHFFilterType
    {
        Zero = -1, // Note : -1 to avoid to be selected as default since it could damaged wafer
        Standard = 0,
        LowIllum,
        Custom, 
    }

    [DataContract]
    public class ProbeLiseHFFilter
    {
        [DataMember]
        [XmlAttribute]
        public LHFFilterType Type { get; set; }

        [DataMember]
        [XmlAttribute]
        public int SliderIndex { get; set; }

        [DataMember]
        [XmlAttribute]
        public string Name { get; set; }

        [DataMember]
        [XmlAttribute]
        public double AttenuationdB { get; set; }

        public override string ToString()
        {
            return $"Filter {Name} idx:{SliderIndex} {Type} [{AttenuationdB:0.0##} dB]";
        }
    }
}
