using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum ZAxisPosition
    {
        [Description("The Z-axis is at origin.")]
        ZAxisAtOrigin = 0,

        [Description("The Z-axis is at the lowermost position.")]
        ZAxisAtVeryBottom = 1,

        [Description("The Z-axis is at the substrate bottom surface position.")]
        ZAxisAtSubstrateBottom = 2,

        [Description("The Z-axis is at the spindle axis position.")]
        ZAxisAtTheHeightOfSpindleAxis = 3,

        [Description("The Z-axis is at the substrate top surface position.")]
        ZAxisAtSubstrateLiftingPosition = 4,

        [Description("The Z-axis is at the uppermost position.")]
        ZAxisAtVeryTop = 5,

        [Description("The Z-axis is at the position other than registered position, or has not performed the origin search.")]
        ZAxisUndefinedPosition = 99
    }
}
