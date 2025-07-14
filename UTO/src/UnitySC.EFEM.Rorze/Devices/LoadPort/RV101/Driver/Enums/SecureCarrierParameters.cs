using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    public enum SecureCarrierOperationCharacteristicParameter
    {
        [Description("Perform the carry-in operation of carrier.")]
        CarryIn = 0x0,

        [Description("Locks the cover.")]
        LockCover = 0x1,

        [Description("Opens the shutter.")]
        OpenShutter = 0x2,
    }
}
