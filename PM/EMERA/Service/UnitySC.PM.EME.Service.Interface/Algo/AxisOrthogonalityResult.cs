using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class AxisOrthogonalityResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Angle XAngle { get; set; }

        [DataMember]
        public Angle YAngle { get; set; }

    }
}
