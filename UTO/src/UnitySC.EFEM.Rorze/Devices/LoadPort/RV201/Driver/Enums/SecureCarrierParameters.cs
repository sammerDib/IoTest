using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    public enum SecureCarrierOperationCharacteristicParameter
    {
        [Description("Opens FOUP door.")]
        OpenFOUP = 0x0,

        [Description("Closes carrier clamp. Use with \"PerformMapping\" to check mis-clamp and rotate to the LPU side for the turn type.")]
        CarrierClampOnly = 0x1,

        [Description("Moves to Y-axis position 3 after securing the carrier.")]
        YPos3 = 0x2,

        [Description("Turns on chucking.")]
        ChuckOn = 0x3,

        [Description("Closes latch key.")]
        CloseLatchKey = 0x4,

        [Description("Contains mapper.")]
        ContainsMapper = 0x5,

        [Description("Performs preparing operation for FOSB.")]
        PrepareFOSB = 0x7,

        [Description("Performs preparing operation for 25-slot open carrier.")]
        Prepare25SlotOpenCarrier = 0x8,

        [Description("Performs preparing operation for 13-slot open carrier.")]
        Prepare13SlotOpenCarrier = 0x9,

        [Description("Opens 1-slot FOUP door. Chucking and cover are not checked.")]
        Open1SlotFOUP = 0x10,

        [Description("Opens FOSB door. Cover is not checked.")]
        OpenFOSBDoor = 0x11,

        [Description(
            "Secures the adapter using the clamp mechanism for carrier. Note: Execute this while the adapter cable is connected. Note:  Release the secured adapter by UCLM(A).")]
        SecureAdapter = 0xA,

        [Description("Locks the cover locking mechanism of the adapter.")]
        LockAdapterCoverLockingMechanism = 0xC
    }

    public enum SecureCarrierEnableMappingParameter
    {
        PerformMapping = 0,
        IgnoreMapping  = 1,
        NotSet         = -1
    }
}
