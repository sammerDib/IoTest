using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CheckPatternRecResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public bool Succeeded { get; set; }

        [DataMember]
        public double Gamma { get; set; }

        /// <summary>
        /// <para>The list of errors between the expected shift and the actual shift found.</para>
        /// <para>
        /// For example, let's assume the pattern is on (0, 0), and one verification point is on (1,
        /// 0). The expected shift is -1. If the pattern rec on that point found a ShiftX of -1.002,
        /// then the result will contain a ShiftX of -0.002.
        /// </para>
        /// </summary>
        [DataMember]
        public List<PatternRecResult> SingleResults { get; set; } = new List<PatternRecResult>();
    }
}
