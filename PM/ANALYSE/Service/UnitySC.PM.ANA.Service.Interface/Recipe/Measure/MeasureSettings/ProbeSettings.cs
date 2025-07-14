using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    [KnownType(typeof(SingleLiseSettings))]
    [KnownType(typeof(DualLiseSettings))]
    [KnownType(typeof(LiseHFSettings))]
    [XmlInclude(typeof(SingleLiseSettings))]
    [XmlInclude(typeof(DualLiseSettings))]
    [XmlInclude(typeof(LiseHFSettings))]
    public class ProbeSettings
    {
        [DataMember]
        public string ProbeId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ProbeSettings settings && ProbeId == settings.ProbeId;
        }

        public override int GetHashCode()
        {
            return (ProbeId).GetHashCode();
        }

        public virtual ProbeSettings Clone()
        {
            var settings = new ProbeSettings();
            settings.ProbeId = ProbeId;

            return settings;
        }
    }
}
