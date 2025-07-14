using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class DieAndStreetSizesResult : IFlowResult
    {
        [DataMember]
        [XmlIgnore]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double Confidence { get; set; }

        [DataMember]
        public DieDimensionalCharacteristic DieDimensions { get; set; }
    }
}
