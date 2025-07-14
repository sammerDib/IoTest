using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AlignmentMarksResult : IFlowResult
    {
        [DataMember]
        [XmlIgnore]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double Confidence { get; set; }

        [DataMember]
        public Length ShiftX { get; set; }

        [DataMember]
        public Length ShiftY { get; set; }

        [DataMember]
        public Angle RotationAngle { get; set; }
    }
}
