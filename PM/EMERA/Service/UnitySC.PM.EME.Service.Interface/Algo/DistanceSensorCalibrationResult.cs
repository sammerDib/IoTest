using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class DistanceSensorCalibrationResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }
        [DataMember]
        public Length OffsetX { get; set; }
        [DataMember]
        public Length OffsetY { get; set; }

    }
}
