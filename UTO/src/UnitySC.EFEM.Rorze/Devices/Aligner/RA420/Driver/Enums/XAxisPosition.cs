using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum XAxisPosition
    {
        [Description("The X-axis is at origin.")]
        XAxisAtOrigin = 0,

        [Description("The X-axis is at the waiting position set by \"DALN[x][0]\"")]
        XAxisAtWaitingPosition = 1,

        [Description("The X-axis is at the position set by \"DALN[x][1]\"")]
        XAxisAtFirstDefinedPosition = 2,

        [Description("The X-axis is at the position set by \"DALN[x][2]\"")]
        XAxisAtSecondDefinedPosition = 3,

        [Description("The X-axis is at the position set by \"DALN[x][3]\"")]
        XAxisAtThirdDefinedPosition = 4,

        [Description("The X-axis is at the position other than registered position, or has not performed the origin search.")]
        XAxisUndefinedPosition = 99
    }
}
