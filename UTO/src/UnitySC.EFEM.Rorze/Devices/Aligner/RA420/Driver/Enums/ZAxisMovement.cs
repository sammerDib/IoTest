using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum ZAxisMovement
    {
        [Description("The Z-axis stays at the current position.")]
        NoZAxisMove = 0,

        [Description("The Z-axis moves to the very bottom position set by DALN[x][8].")]
        MoveZAxisToVeryBottom = 1,

        [Description("The Z-axis moves to the substrate bottom surface position set by DALN[x][9].")]
        MoveZAxisToSubstrateBottom = 2,

        [Description("The Z-axis moves to the height of the spindle axis set by DALN[x][10].")]
        MoveZAxisToTheHeightOfSpindleAxis = 3,

        [Description("The Z-axis moves to the substrate lifting position set by DALN[x][11].")]
        MoveZAxisToSubstrateLiftingPosition = 4,

        [Description("The Z-axis moves to the very top position set by DALN[x][12].")]
        MoveZAxisToVeryTop = 5
    }
}
