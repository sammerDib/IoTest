using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class PatternRecResult : IFlowResult
    {
        public PatternRecResult()
        {
        }

        public PatternRecResult(FlowStatus status, double confidence, Length shiftX, Length shiftY, ServiceImage controlImage)
        {
            Status = status;
            Confidence = confidence;
            ShiftX = shiftX;
            ShiftY = shiftY;
            ControlImage = controlImage;
        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double Confidence { get; set; }

        /// <summary>
        /// <para>
        /// The signed distance on the X-axis to go from the current position to the pattern.
        /// </para>
        /// <para>
        /// For example, let's say that the current position is on (-5, -6), and the pattern
        /// is actually on (0, 0), then ShiftX will be +5.
        /// </para>
        /// </summary>
        [DataMember]
        public Length ShiftX { get; set; }

        /// <summary>
        /// <para>
        /// The signed distance on the Y-axis to go from the current position to the pattern.
        /// </para>
        /// <para>
        /// For example, let's say that the current position is on (-5, -6), and the pattern
        /// is actually on (0, 0), then ShiftY will be +6.
        /// </para>
        /// </summary>
        [DataMember]
        public Length ShiftY { get; set; }

        [DataMember]
        public ServiceImage ControlImage { get; set; }
    }
}
