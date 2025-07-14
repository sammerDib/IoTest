using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    public enum ReleaseCarrierOperationMode
    {
        [Description("Closes FOUP door.")]
        CloseFOUP = 0x0,

        [Description("Releases carrier clamp. The motion is possible even while no carrier is placed. (Door closing operation is not performed.)")]
        ReleaseCarrierClampOnly = 0x1,

        [Description("Closes FOUP door, and moves to the carry-out position. (Clamp does not perform any motion.)")]
        CloseFOUPDoorAndMoveToCarryOutPos = 0x2,

        [Description("Turns off chucking.")]
        ChuckOff = 0x3,

        [Description("Opens latch key.")]
        OpenLatchKey = 0x4,

        [Description("Prepares mapper.")]
        PrepareMapper = 0x5,

        [Description("Moves to the waiting position for FOSB.")]
        MoveToFOSBWaitingPosition = 0x7,

        [Description("Moves to the waiting position for open carrier.")]
        MoveOpenCarrierToWaitingPosition = 0x8,

        [Description("Releases the secured adapter. Note: Execute this before disconnecting the cable of the adapter.")]
        ReleaseSecuredAdapter = 0xA,

        [Description("Unlocks the cover locking mechanism of the adapter.")]
        UnlockAdapterCoverLockingMechanism = 0xC
    }

    public enum ReleaseCarrierUnclampOrMoveToYPosition
    {
        [Description("Use only when parameter should be ommited.")]
        NotSet = -1,

        [Description("Clamps open.")]
        Unclamp = 0x0,

        [Description("Undock position.")]
        MoveToUndockPosition = 0x1,

        [Description("Wafer transfer possible position.")]
        MoveToWaferTransferPossiblePosition = 0x2,

        [Description("Dock position.")]
        MoveToDockPosition = 0x3,

        [Description("Short undock position.")]
        MoveToShortUndockPosition = 0x4,

        [Description("Position for rotating the stage (For System type = 4, 6: FOSB lid check position).")]
        MoveToRotateStagePosition = 0x5,

        [Description("Position for reading the carrier ID (For System type = 4, 6: Stage rotation position).")]
        MoveToReadingCarrierIDPosition = 0x6,
    }

    public enum ReleaseCarrierEnableMapping
    {
        [Description("Use the default setting for carrier type. (settable via DPRM)")]
        NotSet = -1,

        [Description("Usable only with "
                     + nameof(ReleaseCarrierOperationMode.CloseFOUP)
                     + ", "
                     + nameof(ReleaseCarrierOperationMode.MoveToFOSBWaitingPosition)
                     + ", "
                     + nameof(ReleaseCarrierOperationMode.MoveOpenCarrierToWaitingPosition)
                     + " operation modes.")]
        PerformMapping = 0,

        [Description(
            "Usable only with CloseFOUP, MoveToFOSBWaitingPosition, MoveOpenCarrierToWaitingPosition operation modes.")]
        IgnoreMapping = 1
    }

    /// <summary>
    /// NOTE: compatible only with System Type = 4.
    /// </summary>
    public enum ReleaseCarrierRotateAtYPos1
    {
        [Description("Always use it for system types other than 4 or when operation mode is "
                     + nameof(ReleaseCarrierOperationMode.CloseFOUP)
                     + " or "
                     + nameof(ReleaseCarrierOperationMode.MoveToFOSBWaitingPosition)
                     + ", or when "
                     + nameof(ReleaseCarrierEnableMapping)
                     + " is also "
                     + nameof(ReleaseCarrierEnableMapping.NotSet)
                     + ".")]
        NotSet = -1,

        [Description("Does not rotate at \"Y - axis position = 1\".")]
        Enable = 0,

        [Description("Rotates at \"Y - axis position = 1\".")]
        Disable = 1
    }
}
