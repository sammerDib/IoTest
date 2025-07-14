using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum ChuckSubstrateZAxisMovement
    {
        [Description("The Z-axis stays at the current position, and chucking of the spindle turns on.")]
        NoZAxisMove = 0,

        [Description("The Z-axis moves back to the very bottom position, and chucking of the spindle turns on.")]
        MoveZAxisToVeryBottom = 1,

        [Description("The Z-axis moves back to the lower position of the spindle, and chucking of the spindle turns on.")]
        MoveZAxisToTheLowerSpindlePosition= 2,

        [Description("The Z-axis moves back to the same height of the spindle, and chucking of the spindle turns on.")]
        MoveZAxisToTheSameHeightOfSpindle = 3
    }
}
