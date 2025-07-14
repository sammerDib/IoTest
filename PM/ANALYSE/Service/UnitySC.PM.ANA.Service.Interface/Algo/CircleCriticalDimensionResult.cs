using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CircleCriticalDimensionResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public Length Diameter { get; set; }

        [DataMember]
        public ServiceImage ResultImage { get; set; }
    }
}
