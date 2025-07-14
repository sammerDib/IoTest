using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    public enum ReleaseCarrierOperationMode
    {
        Close = 0x0,

        [Description("Unlocks the cover lock. (id cannot be designated)")]
        UnlocksCoverLock = 0x1,

        [Description("Closes the shutter (id can not be designated)")]
        ClosesShutter = 0x2,
    }

    public enum ReleaseCarrierOption
    {
        [Description("Use only when parameter should be ommited.")]
        NotSet = -1,

        [Description("Unlocks the cover lock. (Parameter n,id can be omitted)")]
        UnlocksCoverLock = 0x0,

        [Description("Cover lock is not unlocked.")]
        KeepCoverLocked = 0x1
    }
}
