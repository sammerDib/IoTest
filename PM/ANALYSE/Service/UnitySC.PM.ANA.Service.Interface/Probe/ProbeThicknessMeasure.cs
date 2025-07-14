using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Probe
{
    [DataContract]
    public class ProbeThicknessMeasure
    {
        public ProbeThicknessMeasure()
        { }

        public ProbeThicknessMeasure(Length thickness, double quality)
        {
            Thickness = thickness;
            Quality = quality;
        }

        public ProbeThicknessMeasure(Length thickness, double quality, bool isMandatory, string name)
        {
            Thickness = thickness;
            Quality = quality;
            IsMandatory = isMandatory;
            Name = name;
        }

        [DataMember]
        public Length Thickness { get; set; }

        [DataMember]
        public double Quality { get; set; }

        [DataMember]
        public bool IsMandatory { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
