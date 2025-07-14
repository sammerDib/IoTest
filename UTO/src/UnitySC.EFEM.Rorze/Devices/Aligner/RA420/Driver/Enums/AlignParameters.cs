using System;
using System.ComponentModel;
using System.Globalization;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum AlignmentMode
    {
        SubstrateAlignment = 1,

        TeachingForNotchAlignment = 3,

        [Description("The parameter for designating the angle is ignored while examining the eccentric quantity, "
                     + "and the motion stops at the position at the time when the alignment operation is started. "
                     + "At this moment, an error occurs if re-holding is required.")]
        ExaminationForTheNotchDirectionAndEccentricQuantity = 5,

        SubstrateAlignment_EndInChuckingOffState = 7,

        [Description("If the substrate centering only is performed, the angle designation is ignored when the notch is not actually found.")]
        PerformsSubstrateCenteringOnly = 9
    }

    public enum AlignPostOperationActions
    {
        [Description("Ends without shifting the substrate after alignment.")]
        NoExtraMove = 0,

        [Description("Shifts the substrate after alignment.")]
        ShiftAfterAlignment = 1,

        [Description("Shifts the substrate after alignment and then picks it up with the Z-axis.")]
        ShiftAndPickUpWithZAxisAfterAlignment = 2
    }

    /// <summary>
    /// Represent on which condition to stop alignment action on RA420.
    /// Allow to transform a logical and easy readable value into its <see cref="StringRepresentation"/> to send to HW.
    /// </summary>
    public class AlignmentValue
    {
        private const uint MaxStopPos             = 50;
        private const uint MaxStopPulsePosition   = 999999;
        private const char AlignStopAtAnglePrefix = 'D';
        private const char AlignStopAtPulsePrefix = 'P';

        /// <summary>
        /// Create a <see cref="AlignmentValue"/> parameter to end at the position of notch 0 degrees set by DALN[x][14].
        /// </summary>
        public static AlignmentValue Default()
        {
            return new AlignmentValue();
        }

        /// <summary>
        /// Create a <see cref="AlignmentValue"/> parameter to end at the notch stopping position No.<paramref name="pos"/> (position set by "DROT").
        /// </summary>
        /// <param name="pos">The position to be checked before to be converted.</param>
        public static AlignmentValue FromPos(uint pos)
        {
            if (0 < pos && pos < MaxStopPos)
            {
                return new AlignmentValue(pos.ToString(CultureInfo.InvariantCulture));
            }

            throw new InvalidOperationException(
                "Could not convert given value into HW stopping position. "
                + $"Given value={pos} whereas it should be greater than 0 and lower than {MaxStopPos}");
        }

        /// <summary>
        /// Create a <see cref="AlignmentValue"/> parameter to designate the notch stop angle position.
        /// </summary>
        public static AlignmentValue FromAngle(double angle)
        {
            if (0 <= angle && angle < 360)
            {
                return new AlignmentValue(AlignStopAtAnglePrefix + angle.ToString("F2", CultureInfo.InvariantCulture));
            }

            throw new InvalidOperationException(
                "Could not convert given angle into HW stopping position. "
                + $"Given value={angle} whereas it should be greater or equal than 0° and lower than 360°.");
        }

        /// <summary>
        /// Create a <see cref="AlignmentValue"/> parameter to designate the notch stop pulse position [pulse].
        /// </summary>
        /// <remarks>Moves to the position rounded by the spindle axis resolution.</remarks>
        public static AlignmentValue FromStopPulsePosition(uint value)
        {
            if (value <= MaxStopPulsePosition)
            {
                return new AlignmentValue(AlignStopAtPulsePrefix + value.ToString());
            }

            throw new InvalidOperationException(
                "Could not convert given value into HW stop pulse position. "
                + $"Given value={value} whereas it should be lower than {MaxStopPulsePosition}");
        }

        private AlignmentValue()
        {
            StringRepresentation = "0";
        }

        private AlignmentValue(string stringRepresentation)
        {
            StringRepresentation = stringRepresentation;
        }

        public string StringRepresentation { get; }
    }
}
