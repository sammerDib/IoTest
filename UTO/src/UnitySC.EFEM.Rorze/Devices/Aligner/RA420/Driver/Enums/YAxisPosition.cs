using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum YAxisPosition
    {
        [Description("The Y-axis is at origin.")]
        YAxisAtOrigin = 0,

        [Description("The Y-axis is at the waiting position set by \"DALN[x][4]\"")]
        YAxisAtWaitingPosition = 1,

        [Description("The Y-axis is at the position set by \"DALN[x][5]\"")]
        YAxisAtFirstDefinedPosition = 2,

        [Description("The Y-axis is at the position set by \"DALN[x][6]\"")]
        YAxisAtSecondDefinedPosition = 3,

        [Description("The Y-axis is at the position set by \"DALN[x][7]\"")]
        YAxisAtThirdDefinedPosition = 4,

        [Description("The Y-axis is at the position other than registered position, or has not performed the origin search.")]
        YAxisUndefinedPosition = 99
    }
}
