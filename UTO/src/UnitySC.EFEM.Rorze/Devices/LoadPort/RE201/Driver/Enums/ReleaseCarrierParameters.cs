using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums
{
    public enum ReleaseCarrierOperationMode
    {
        [Description("Closes the carrier.")] ClosesTheCarrier = 0x0,

        [Description("Unclamps the carrier.)")]
        UnclampsTheCarrier = 0x1
    }

    public enum ReleaseCarrierCloseOperation
    {
        [Description("Not set.")] NotSet = -1,

        [Description("Closes the carrier to unclamp.")]
        ClosesTheCarrierToUnclamp = 0x0,

        [Description("Closes the carrier (not to unclamp).")]
        ClosesTheCarrierNotToUnclamp = 0x1,

        [Description("Move the Z-axis to origin.")]
        MoveTheZAxisToOrigin = 0x2
    }
}
