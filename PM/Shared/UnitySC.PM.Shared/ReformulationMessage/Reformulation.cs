using System.Xml.Serialization;

namespace UnitySC.PM.Shared.ReformulationMessage
{
    [XmlRoot("Reformulation")]
    public class Reformulation
    {
        [XmlArray("ReformulationMessageList")]
        public ReformulationMessage[] ReformulationMessageList { get; set; }
    }
}