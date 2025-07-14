using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class MultiSizeChuckResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }       

        [DataMember]
        public Length ShiftX { get; set; }
        [DataMember]
        public Length ShiftY { get; set; }

        [DataMember]
        public Angle WaferAngle { get; set; }
    }

}
