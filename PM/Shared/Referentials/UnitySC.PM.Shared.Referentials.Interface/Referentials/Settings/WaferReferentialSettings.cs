using System.Runtime.Serialization;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    [DataContract]
    public class WaferReferentialSettings : ReferentialSettingsBase
    {
        public WaferReferentialSettings() : base(ReferentialTag.Wafer)
        {
            WaferAngle = new Angle(0, AngleUnit.Degree);
            ShiftX = new Length(0, LengthUnit.Millimeter);
            ShiftY = new Length(0, LengthUnit.Millimeter);
        }

        /// <summary>
        /// Angle rotation against perfect 270° axis
        /// A negative value indicate a clockwise rotation, where positive value indicates anti-clockwise rotation
        /// </summary>
        [DataMember]
        public Angle WaferAngle { get; set; }

        /// <summary>
        /// Shift X from stage home(0) (ie. Shift BWA + ChuckCneter offset)
        /// </summary>
        [DataMember]
        public Length ShiftX { get; set; }

        /// <summary>
        /// Shift Y from stage home(0) (ie. Shift BWA + ChuckCneter offset)
        /// </summary>
        [DataMember]
        public Length ShiftY { get; set; }

        /// <summary>
        /// Z top focus with the main objective
        /// </summary>
        [DataMember]
        public Length ZTopFocus { get; set; }

        /// <summary>
        /// Objective id used for top focus
        /// </summary>
        [DataMember]
        public string ObjectiveIdForTopFocus { get; set; }
    }
}
