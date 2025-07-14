using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class BareWaferAlignmentResult : BareWaferAlignmentChangeInfo
    {
        /// <summary>
        /// Angle rotation against perfect 270° axis
        /// A negative value indicate a clockwise rotation, where positive value indicates anti-clockwise rotation
        /// </summary>
        [DataMember]
        public Angle Angle { get; set; }

        /// <summary>
        /// Shift X from the chuck center
        /// </summary>
        [DataMember]
        public Length ShiftX { get; set; }

        /// <summary>
        /// Shift Y from the chuck center
        /// </summary>
        [DataMember]
        public Length ShiftY { get; set; }

        /// <summary>
        /// Indicate the qualitative value of the result. 1 is better
        /// </summary>
        [DataMember]
        public double Confidence { get; set; }

        // <summary>
        /// Indicate the measured diameter of the wafer
        /// </summary>
        [DataMember]
        public Length Diameter { get; set; }

        /// <summary>
        /// Shift X from the stage home (0)
        /// </summary>
        [DataMember]
        public Length ShiftStageX { get; set; }

        /// <summary>
        /// Shift Y from the stage home (0)
        /// </summary>
        [DataMember]
        public Length ShiftStageY { get; set; }
    }
}
