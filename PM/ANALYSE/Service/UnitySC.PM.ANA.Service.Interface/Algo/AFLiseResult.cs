using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AFLiseResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        /// <summary>
        /// Optimal Z position (based on stage referential) for objective focus.
        /// </summary>
        [DataMember]
        public double ZPosition { get; set; }

        [DataMember]
        public double QualityScore { get; set; }
    }
}
